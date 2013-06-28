using System;
using System.Text;

namespace KarelazisBot.Objects
{
    public class Memory
    {
        public Memory(Objects.Client c)
        {
            this.Client = c;
        }

        public Objects.Client Client { get; private set; }

        public byte[] ReadBytes(long address, uint bytesToRead)
        {
            try
            {
                IntPtr ptrBytesRead;
                byte[] buffer = new byte[bytesToRead];
                WinAPI.ReadProcessMemory(this.Client.TibiaHandle, new IntPtr(address), buffer, bytesToRead, out ptrBytesRead);
                return buffer;
            }
            catch (Exception ex)
            {
                if (this.Client.TibiaProcess.HasExited) System.Windows.Forms.Application.Exit();
                else System.Windows.Forms.MessageBox.Show(ex.Message);
                return new byte[bytesToRead];
            }
        }
        public byte[] ReadBytes(long address, int bytesToRead)
        {
            return this.ReadBytes(address, (uint)bytesToRead);
        }
        public bool WriteBytes(long address, byte[] bytes, uint length)
        {
            IntPtr bytesWritten;
            int result = WinAPI.WriteProcessMemory(this.Client.TibiaHandle, new IntPtr(address), bytes, length, out bytesWritten);
            return result != 0;
        }
        public bool WriteBytes(long address, byte[] bytes)
        {
            return this.WriteBytes(address, bytes, (uint)bytes.Length);
        }
        public bool WriteInt32(long address, int value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value), 4);
        }
        public bool WriteUInt16(long address, ushort value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value), 2);
        }
        public bool WriteDouble(long address, double value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value), 8);
        }
        public bool WriteUInt32(long address, uint value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value), 4);
        }
        public bool WriteByte(long address, byte value)
        {
            return this.WriteBytes(address, new byte[] { value }, 1);
        }
        public bool WriteBool(long address, bool value)
        {
            return this.WriteByte(address, (byte)(value ? 1 : 0));
        }

        public int ReadInt32(long address)
        {
            return BitConverter.ToInt32(this.ReadBytes(address, 4), 0);
        }

        public long ReadInt64(long address)
        {
            return BitConverter.ToInt64(this.ReadBytes(address, 8), 0);
        }

        public uint ReadUInt32(long address)
        {
            return BitConverter.ToUInt32(this.ReadBytes(address, 4), 0);
        }

        public short ReadInt16(long address)
        {
            return BitConverter.ToInt16(this.ReadBytes(address, 2), 0);
        }

        public ushort ReadUInt16(long address)
        {
            return BitConverter.ToUInt16(this.ReadBytes(address, 2), 0);
        }
        public bool ReadBool(long address)
        {
            return this.ReadByte(address) != 0;
        }
        public byte ReadByte(long address)
        {
            return this.ReadBytes(address, 1)[0];
        }
        public ushort[] ReadUInt16Array(long address, int length, int size = 4)
        {
            ushort[] array = new ushort[length];
            int position = 0;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadUInt16(address + position);
                position += size;
            }
            return array;
        }
        public uint[] ReadUInt32Array(long address, int length, int size = 4)
        {
            uint[] array = new uint[length];
            int position = 0;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadUInt32(address + position);
                position += size;
            }
            return array;
        }
        public bool WriteString(long address, string text)
        {
            return this.WriteBytes(address, ASCIIEncoding.Default.GetBytes(text + '\0'), (uint)text.Length + 1);
        }
        public string ReadString(long address)
        {
            string stringRead = ASCIIEncoding.Default.GetString(this.ReadBytes(address, 32));
            int index = stringRead.IndexOf('\0');
            return index >= 0 ? stringRead.Substring(0, index) : stringRead;
        }
        public string ReadString(long address, int length, bool untilTerminator)
        {
            string stringRead = ASCIIEncoding.Default.GetString(this.ReadBytes(address, (uint)length));
            if (untilTerminator)
            {
                int index = stringRead.IndexOf('\0');
                if (index > 0) return stringRead.Substring(0, index);
            }
            return stringRead;
        }
        public double ReadDouble(long address)
        {
            return BitConverter.ToDouble(this.ReadBytes(address, 8), 0);
        }
    }
}
