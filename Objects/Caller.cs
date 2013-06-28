using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace KarelazisBot.Objects
{
    public class InternalCaller
    {
        public InternalCaller(Client client)
        {
            this.Client = client;
            this.Process = this.Client.TibiaProcess;
            this.MainThreadHandle = (IntPtr)this.Process.Threads[0].Id;
            this.Parameters = new BinaryWriter(new MemoryStream());
            this.Handles = new List<IntPtr>();
            this.StackParams = 0;
            this.SyncObject = new object();
        }

        public Client Client { get; private set; }
        Process Process;
        IntPtr MainThreadHandle;
        BinaryWriter Parameters;
        List<IntPtr> Handles;
        List<IntPtr> MemoryToRelease = new List<IntPtr>();
        UInt32 StackParams;
        object SyncObject;

        /// <summary>
        /// Pushes value on the stack (remember about function calling convention).
        /// </summary>
        private void PushValue(UInt32 value)
        {
            this.Parameters.Write((byte)0x68);
            this.Parameters.Write(value);
            this.StackParams++;
        }

        /// <summary>
        /// Reserves memory, copies string and pushes it's address on the stack.
        /// </summary>
        private void PushString(string str)
        {
            this.PushValue(GetPointer(ASCIIEncoding.ASCII.GetBytes(str + "\0")));
        }

        /// <summary>
        /// Reserves memory, copies data and pushes it's address on the stack.
        /// </summary>
        private void PushPointer(byte[] data)
        {
            this.PushValue(GetPointer(data));
        }

        /// <summary>
        /// Clears call stack.
        /// </summary>
        private void ClearStack()
        {
            this.Parameters = new BinaryWriter(new MemoryStream());
        }

        internal UInt32 Call(IntPtr address, CallConvention convention, params object[] parameters)
        {
            try
            {
                Monitor.Enter(this.Process);
                Array.Reverse(parameters);
                foreach (object o in parameters)
                {
                    Type paramt = o.GetType();
                    if (paramt == typeof(System.String)) this.PushString(Convert.ToString(o));
                    else if (paramt == typeof(System.Byte) && paramt.IsArray) this.PushPointer((Byte[])o);
                    else this.PushValue(Convert.ToUInt32(o));
                }
                return this.Call(address, convention);
            }
            catch (Exception ex) { System.IO.File.AppendAllText("errors-caller.txt", ex.Message + "\n" + ex.StackTrace + "\n\n"); }
            finally { Monitor.Exit(this.Process); }
            return 0;
        }

        /// <summary>
        /// Calls the function, return its exit code.
        /// </summary>
        private UInt32 Call(IntPtr address, CallConvention convention)
        {
            uint lpExitCode = 0;
            BinaryWriter opcodes = new BinaryWriter(new MemoryStream());
            this.Parameters.BaseStream.Position = 0;
            opcodes.Write(new BinaryReader(this.Parameters.BaseStream).ReadBytes((int)this.Parameters.BaseStream.Length));
            opcodes.Write((byte)0xB8); //mov eax, -
            opcodes.Write((Int32)address);
            opcodes.Write((UInt16)0xD0FF); //call eax

            if (convention == CallConvention.Cdecl)
            {
                opcodes.Write((UInt16)0xC481); //add esp, -
                opcodes.Write((UInt32)StackParams * 4);
            }

            opcodes.Write((byte)0xC2);
            opcodes.Write((UInt16)0x04); //retn 4
            opcodes.BaseStream.Position = 0;

            BinaryReader reader = new BinaryReader(opcodes.BaseStream);
            UInt32 ops = this.GetPointer(reader.ReadBytes((int)opcodes.BaseStream.Length));
            uint threadId = 0;
            IntPtr hThread = CreateRemoteThread(this.Process.Handle, IntPtr.Zero, 0, (IntPtr)ops, IntPtr.Zero, 0, out threadId);
            if (hThread != null && hThread != IntPtr.Zero)
            {
                uint waitCode = WaitForSingleObject(hThread, 2000);
                switch (waitCode)
                {
                    case WAIT_ABANDONED:
                        this.OutputError("WaitForSingleObject: unexpected return code (abandoned)", Marshal.GetLastWin32Error());
                        break;
                    case WAIT_FAILED:
                        this.OutputError("WaitForSingleObject: unexpected return code (failed)", Marshal.GetLastWin32Error());
                        break;
                    case WAIT_SIGNALED:
                        break;
                    case WAIT_TIMEOUT:
                        this.OutputError("WaitForSingleObject: unexpected return code (timeout)", Marshal.GetLastWin32Error());
                        break;
                }

                bool exitSuccess = GetExitCodeThread(hThread, out lpExitCode);
                if (!exitSuccess) this.OutputError("GetExitCodeThread: unexpected return code (false)", Marshal.GetLastWin32Error());

                if (!CloseHandle(hThread))
                {
                    this.OutputError("CloseHandle (hThread): unexpected return code (false)", Marshal.GetLastWin32Error());
                }
                //this.Handles.Add(hThread);
            }
            else this.OutputError("CreateRemoteThread: unexpected return code (null/IntPtr.Zero)", Marshal.GetLastWin32Error());

            foreach (IntPtr pointer in this.MemoryToRelease)
            {
                if (!VirtualFreeEx(this.Process.Handle, pointer, 0, AllocationType.Release))
                {
                    this.OutputError("VirtualFreeEx: unexpected return code (false)", Marshal.GetLastWin32Error());
                }
            }
            this.MemoryToRelease = new List<IntPtr>();
            foreach (IntPtr handle in this.Handles)
            {
                if (handle == IntPtr.Zero) continue;
                bool closeHandleSuccess = CloseHandle(handle);
                if (!closeHandleSuccess) this.OutputError("CloseHandle: unexpected return code (false), value: " + handle.ToInt32(), Marshal.GetLastWin32Error());
            }
            this.Handles = new List<IntPtr>();

            return lpExitCode;
        }

        private void OutputError(string message, int errorCode)
        {
            File.AppendAllText(Path.Combine(this.Client.BotDirectory.FullName, "pinvoke-debug.txt"),
                "Error code: " + errorCode + "\nMessage: " + message + "\n\n");
        }

        private UInt32 GetPointer(byte[] data)
        {
            IntPtr ret = VirtualAllocEx(this.Process.Handle, IntPtr.Zero, (uint)data.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
            if (ret != IntPtr.Zero)
            {
                //this.Handles.Add(ret);
                int bytesWritten = 0;
                if (!WriteProcessMemory(this.Process.Handle, ret, data, (uint)data.Length, out bytesWritten))
                {
                    this.OutputError("WriteProcessMemory: unexpected return code (false)", Marshal.GetLastWin32Error());
                }
                this.MemoryToRelease.Add(ret);
                return (UInt32)ret;
            }
            else throw new Exception("Could not reserve memory!");
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, AllocationType dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
           lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        const uint WAIT_ABANDONED = (uint)0x00000080L,
            WAIT_SIGNALED = (uint)0x00000000L,
            WAIT_TIMEOUT = (uint)0x00000102L,
            WAIT_FAILED = 0xFFFFFFFF;

        [Flags]
        private enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        private enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
    }
    public enum CallConvention
    {
        StdCall,
        Cdecl
    }
    public class InternalFunction
    {
        public InternalCaller Caller { get; set; }
        public IntPtr Address { get; set; }
        public CallConvention CallingConvention { get; set; }
        public uint Call(params object[] parameters)
        {
            return Caller.Call(Address, CallingConvention, parameters);
        }
    }
}