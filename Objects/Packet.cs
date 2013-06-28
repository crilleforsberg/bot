using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace KarelazisBot.Objects
{
    public class Packet
    {
        /// <summary>
        /// Constructor for already existing data.
        /// </summary>
        /// <param name="packet">The array of data to load.</param>
        public Packet(byte[] packet)
            : this(packet, packet.Length, 0) { }
        /// <summary>
        /// Constructor for already existing data.
        /// </summary>
        /// <param name="packet">The array of data to load.</param>
        /// <param name="length">How many bytes of data to load.</param>
        public Packet(byte[] packet, int length)
            : this(packet, length, 0) { }
        /// <summary>
        /// Constructor for already existing data.
        /// </summary>
        /// <param name="packet">The array of data to load.</param>
        /// <param name="length">How many bytes of data to load.</param>
        /// <param name="index">At what index in the array to start loading the data.</param>
        public Packet(byte[] packet, int length, int index)
        {
            if (length > packet.Length || index >= length) throw new IndexOutOfRangeException();
            else
            {
                this.Data = new List<byte>();
                byte[] packetShortened = new byte[length];
                Array.Copy(packet, index, packetShortened, 0, length);
                this.Data.AddRange(packetShortened);
            }
        }
        /// <summary>
        /// Constructor for new data.
        /// </summary>
        public Packet() { this.Data = new List<byte>(); }

        /// <summary>
        /// Gets or sets the list of bytes that constructs a network packet.
        /// </summary>
        private List<byte> Data { get; set; }

        /// <summary>
        /// Returns a byte array containing this packet's data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes() { return this.Data.ToArray(); }
        /// <summary>
        /// Returns a byte array containing this packet's data.
        /// </summary>
        /// <param name="index">At what index to return data from.</param>
        /// <returns></returns>
        public byte[] ToBytes(int index)
        {
            List<byte> templist = new List<byte>();
            templist.AddRange(this.Data.ToArray());
            templist.RemoveRange(0, index);
            return templist.ToArray();
        }
        public override string ToString()
        {
            return BitConverter.ToString(ToBytes());
        }
        /// <summary>
        /// Send this packet to a TcpClient object.
        /// </summary>
        /// <param name="tcpclient">The TcpClient to send this packet to.</param>
        /// <returns></returns>
        public bool Send(TcpClient tcpclient)
        {
            return this.Send(tcpclient, 0);
        }
        /// <summary>
        /// Send this packet to a TcpClient object.
        /// </summary>
        /// <param name="tcpclient">The TcpClient to send this packet to.</param>
        /// <param name="index">At what index to send data.</param>
        /// <returns></returns>
        public bool Send(TcpClient tcpclient, int index)
        {
            if (this.Data.Count > index && tcpclient != null && tcpclient.Connected)
            {
                byte[] buffer = this.ToBytes(index);
                tcpclient.GetStream().Write(buffer, 0, buffer.Length);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the amount of bytes this packet contains.
        /// </summary>
        public int Length { get { return this.Data.Count; } }

        /// <summary>
        /// Adds a byet to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddByte(byte value) { this.Data.Add(value); }
        /// <summary>
        /// Adds a 16-bit unsigned integer to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddUInt16(ushort value) { this.Data.AddRange(BitConverter.GetBytes(value)); }
        /// <summary>
        /// Adds a 32-bit unsigned integer to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddUInt32(uint value) { this.Data.AddRange(BitConverter.GetBytes(value)); }
        /// <summary>
        /// Adds a 64-bit unsigned integer to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddUInt64(ulong value) { this.Data.AddRange(BitConverter.GetBytes(value)); }
        /// <summary>
        /// Adds a byte array to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddBytes(byte[] value) { this.Data.AddRange(value); }
        /// <summary>
        /// Adds an ASCII string to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddString(string value)
        { 
            AddUInt16((ushort)value.Length);
            this.Data.AddRange(ASCIIEncoding.Default.GetBytes(value));
        }
        /// <summary>
        /// Adds a boolean to this packet.
        /// </summary>
        /// <param name="value"></param>
        public void AddBool(bool value) { this.AddByte(value ? (byte)1 : (byte)0); }
        /// <summary>
        /// Inserts this packet's length (UInt16) to the beginning of the packet.
        /// </summary>
        public void AddLength() { this.Data.InsertRange(0, BitConverter.GetBytes((ushort)this.Length)); }

        /// <summary>
        /// Inserts a byte at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertByte(int index, byte value) { this.Data.Insert(index, value); }
        /// <summary>
        /// Inserts a 16-bit unsigned integer at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertUInt16(int index, ushort value) { this.Data.InsertRange(index, BitConverter.GetBytes(value)); }
        /// <summary>
        /// Inserts a 32-bit unsigned integer at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertUInt32(int index, uint value) { this.Data.InsertRange(index, BitConverter.GetBytes(value)); }
        /// <summary>
        /// Inserts a 64-bit unsigned integer at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertUInt64(int index, ulong value) { this.Data.InsertRange(index, BitConverter.GetBytes(value)); }
        /// <summary>
        /// Inserts a byte array at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertBytes(int index, byte[] value) { this.Data.InsertRange(index, value); }
        /// <summary>
        /// Inserts an ASCII string at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertString(int index, string value)
        {
            this.InsertUInt16(index, (ushort)value.Length);
            this.InsertBytes(index + 2, ASCIIEncoding.Default.GetBytes(value));
        }
        /// <summary>
        /// Inserts a boolean at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void InsertBool(int index, bool value) { this.InsertByte(index, value ? (byte)1 : (byte)0); }

        /// <summary>
        /// Gets or sets the position in the packet.
        /// </summary>
        public int GetPosition = 0;
        /// <summary>
        /// Gets a byte from the packet and advances the position by 1.
        /// </summary>
        /// <returns></returns>
        public byte GetByte()
        {
            byte val = this.Data[this.GetPosition];
            this.GetPosition++;
            return val;
        }
        /// <summary>
        /// Gets a 16-bit unsigned integer from the packet and advances the position by 2.
        /// </summary>
        /// <returns></returns>
        public ushort GetUInt16()
        {
            ushort val = BitConverter.ToUInt16(this.Data.ToArray(), this.GetPosition);
            this.GetPosition += 2;
            return val;
        }
        /// <summary>
        /// Gets a 32-bit unsigned integer from the packet and advances the position by 4.
        /// </summary>
        /// <returns></returns>
        public uint GetUInt32()
        {
            uint val = BitConverter.ToUInt32(this.Data.ToArray(), this.GetPosition);
            this.GetPosition += 4;
            return val;
        }
        /// <summary>
        /// Gets a 64-bit unsigned integer from the packet and advances the position by 8.
        /// </summary>
        /// <returns></returns>
        public ulong GetUInt64()
        {
            ulong val = BitConverter.ToUInt64(this.Data.ToArray(), this.GetPosition);
            this.GetPosition += 8;
            return val;
        }
        /// <summary>
        /// Gets a byte array with a given length from the packet and advances the position by the given length.
        /// </summary>
        /// <param name="length">The amount of bytes to get.</param>
        /// <returns></returns>
        public byte[] GetBytes(int length)
        {
            byte[] b = new byte[length];
            Array.Copy(this.Data.ToArray(), this.GetPosition, b, 0, length);
            this.GetPosition += length;
            return b;
        }
        /// <summary>
        /// Gets a boolean from the packet and advances the position by 1.
        /// </summary>
        /// <returns></returns>
        public bool GetBool()
        {
            return this.GetByte() == 1;
        }
        /// <summary>
        /// Gets an ASCII string from the packet and advances the position.
        /// </summary>
        /// <param name="length">Maximum bytes to read.</param>
        /// <param name="readUntilNullTerminator">If this is true, will stop when null termintor ('\0') is found.</param>
        /// <param name="disregardLengthWhenTerminatorFound">If set to true, disregards the length parameter when a null terminator is found.</param>
        /// <returns></returns>
        public string GetString(int length, bool readUntilNullTerminator,
            bool disregardLengthWhenTerminatorFound)
        {
            string s = ASCIIEncoding.Default.GetString(this.Data.ToArray(), this.GetPosition, length);
            if (readUntilNullTerminator && s.Contains("\0"))
            {
                s = s.Substring(0, s.IndexOf('\0'));
            }
            if (s.Length != length && disregardLengthWhenTerminatorFound) this.GetPosition += s.Length;
            else this.GetPosition += length;
            return s;
        }
        /// <summary>
        /// Gets an ASCII string and advances the position by 2 + string length.
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            return GetString(this.GetUInt16(), false, false);
        }

        /// <summary>
        /// Encrypts the packet with Xtea. Packet length should be re-added after encrypting.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public unsafe bool XteaEncrypt(uint[] key)
        {
            byte[] tempbuffer = this.ToBytes();
            this.Data.Clear();
            if (key == null) return false;

            int msgSize = tempbuffer.Length;

            int pad = msgSize % 8;
            if (pad > 0) msgSize += (8 - pad);

            // add filler junk data
            byte[] buffer = new byte[msgSize];
            new Random().NextBytes(buffer);
            Array.Copy(tempbuffer, buffer, tempbuffer.Length);

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr);

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                    while (x_count-- > 0)
                    {
                        words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                            + key[x_sum & 3];
                        x_sum += x_delta;
                        words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                            + key[x_sum >> 11 & 3];
                    }
                }
            }

            this.Data.AddRange(buffer);
            return true;
        }

        public void Clear()
        {
            this.Data.Clear();
            this.GetPosition = 0;
        }

        /// <summary>
        /// Creates a packet from reading a file, much like a BinaryReader.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Packet FromFile(string path)
        {
            if (!System.IO.File.Exists(path)) { return null; }
            byte[] buffer = new byte[1];
            using (System.IO.FileStream fstream = System.IO.File.OpenRead(path))
            {
                buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
            }
            return new Packet(buffer);
        }
        /// <summary>
        /// Gets the next packet in a given NetworkStream. Returns an empty packet if unsuccessful.
        /// </summary>
        /// <param name="nstream">The NetworkStream object to read from.</param>
        /// <param name="length">How many bytes to read.</param>
        /// <returns></returns>
        public static Packet GetNextPacket(NetworkStream nstream, ushort length)
        {
            Packet p = new Packet();
            try
            {
                int bytesRead = 0, bytesReadTotal = 0;
                byte[] buffer = new byte[length];
                while (bytesReadTotal < length)
                {
                    try { bytesRead = nstream.Read(buffer, 0, length - bytesReadTotal); }
                    catch { return new Packet(); }
                    if (bytesRead == 0) break;
                    byte[] tempBytes = new byte[bytesRead];
                    Array.Copy(buffer, tempBytes, bytesRead);
                    p.AddBytes(tempBytes);
                    bytesReadTotal += bytesRead;
                }
                p.AddLength();
            }
            catch { }
            return p;
        }
        /// <summary>
        /// Gets the next packet in a given NetworkStream. Returns an empty packet if unsuccessful.
        /// </summary>
        /// <param name="nstream">The NetworkStream to read from.</param>
        /// <returns></returns>
        public static Packet GetNextPacket(NetworkStream nstream)
        {
            try
            {
                int bytesRead = 0;
                ushort length = 0;
                byte[] buffer = new byte[8192];

                // read first 2 bytes (packet length)
                try { bytesRead = nstream.Read(buffer, 0, 2); }
                catch { return new Packet(); }
                if (bytesRead == 0) return new Packet();
                length = BitConverter.ToUInt16(buffer, 0);

                return GetNextPacket(nstream, length);
            }
            catch { }
            return new Packet();
        }
    }
}
