using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        /// <summary>
        /// A class for a chunk of minimap data, which reads only specified parts.
        /// Useful for i.e. local pathfinding, where only a small part of the data is required.
        /// </summary>
        public class FastChunk : IChunk
        {
            public FastChunk(Client client, int address)
            {
                this.Client = client;
                this.Address = address;
                this.IsInMemory = true;
                this.UpdateLocations();
                this.Type = ChunkTypes.Fast;
            }
            public FastChunk(Objects.Client client, FileInfo fi)
            {
                this.Client = client;
                this.MapFile = fi;
                this.IsInMemory = false;
                string name = fi.Name.Remove(fi.Name.LastIndexOf(".map"));
                this.MiniMapLocation = new Location(byte.Parse(name.Substring(0, 3)),
                    byte.Parse(name.Substring(3, 3)), byte.Parse(name.Substring(6, 2)));
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                this.WorldLocation = new Location(this.MiniMapLocation.X * multiplier,
                    this.MiniMapLocation.Y * multiplier, this.MiniMapLocation.Z);
                this.Type = ChunkTypes.Fast;
            }

            public Client Client { get; private set; }
            public int Address { get; private set; }
            public FileInfo MapFile { get; private set; }
            public bool IsInMemory { get; private set; }
            public Location WorldLocation { get; private set; }
            public Location MiniMapLocation { get; private set; }
            public ChunkTypes Type { get; private set; }
            public int Left
            {
                get { return this.WorldLocation.X; }
            }
            public int Top
            {
                get { return this.WorldLocation.Y; }
            }
            public int Right
            {
                get
                {
                    int length = this.Client.Addresses.MiniMap.AxisLength;
                    return this.WorldLocation.X + (length > 0 ? length - 1 : 0);
                }
            }
            public int Bottom
            {
                get
                {
                    int length = this.Client.Addresses.MiniMap.AxisLength;
                    return this.WorldLocation.Y + (length > 0 ? length - 1 : 0);
                }
            }

            public void UpdateLocations()
            {
                if (!this.IsInMemory) return;
                this.MiniMapLocation = new Location(this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.MiniMap.Distances.X),
                    this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.MiniMap.Distances.Y),
                    this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.MiniMap.Distances.Z));
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                this.WorldLocation = new Location(this.MiniMapLocation.X * multiplier, this.MiniMapLocation.Y * multiplier,
                    this.MiniMapLocation.Z);
            }
            public Node GetNode(int x, int y)
            {
                int index = this.GetIndex(x, y);
                if (this.IsInMemory)
                {
                    return new Node()
                    {
                        Color = this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.MiniMap.Distances.Colors + index),
                        Speed = this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.MiniMap.Distances.TileSpeeds + index)
                    };
                }

                using (BinaryReader reader = new BinaryReader(this.MapFile.OpenRead()))
                {
                    byte color, speed;
                    // set stream position to color index
                    reader.BaseStream.Position = index;
                    color = reader.ReadByte();
                    // set stream position to speed index
                    reader.BaseStream.Position = this.Client.Addresses.MiniMap.DataLength + index;
                    speed = reader.ReadByte();
                    return new Node()
                    {
                        Color = color,
                        Speed = speed
                    };
                }
            }
            /// <summary>
            /// Gets the entirety of this chunk. NOT recommended for this chunk type.
            /// </summary>
            /// <returns></returns>
            public Node[,] GetNodes()
            {
                byte[] colors = null;
                byte[] speeds = null;

                if (this.IsInMemory)
                {
                    colors = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.Colors,
                        this.Client.Addresses.MiniMap.DataLength);
                    speeds = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.TileSpeeds,
                        this.Client.Addresses.MiniMap.DataLength);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(this.MapFile.OpenRead()))
                    {
                        colors = reader.ReadBytes(this.Client.Addresses.MiniMap.DataLength);
                        speeds = reader.ReadBytes(this.Client.Addresses.MiniMap.DataLength);
                    }
                }

                var nodes = new Node[this.Client.Addresses.MiniMap.AxisLength, this.Client.Addresses.MiniMap.AxisLength];
                int index = 0;
                for (int x = 0; x < this.Client.Addresses.MiniMap.AxisLength; x++)
                {
                    for (int y = 0; y < this.Client.Addresses.MiniMap.AxisLength; y++)
                    {
                        nodes[x, y] = new Node()
                        {
                            Color = colors[index],
                            Speed = speeds[index]
                        };
                        index++;
                    }
                }
                return nodes;
            }
            public Node[,] GetNodes(Location start, Location end)
            {
                if (!this.ContainsLocation(start)) throw new IndexOutOfRangeException("start is out of range");
                if (!this.ContainsLocation(end)) throw new IndexOutOfRangeException("end is out of range");

                Point indexStart = new Point(Math.Min(start.X - this.WorldLocation.X, end.X - this.WorldLocation.X),
                    Math.Min(start.Y - this.WorldLocation.Y, end.Y - this.WorldLocation.Y));
                Point indexEnd = new Point(Math.Max(start.X - this.WorldLocation.X, end.X - this.WorldLocation.X),
                    Math.Max(start.Y - this.WorldLocation.Y, end.Y - this.WorldLocation.Y));
                int lengthX = indexEnd.X - indexStart.X + 1,
                    lengthY = indexEnd.Y - indexStart.Y + 1;
                var nodes = new Node[lengthX, lengthY];
                int x, y;
                byte[] colors = null, speeds = null;

                if (this.IsInMemory)
                {
                    for (x = 0; x < lengthX; x++)
                    {
                        // read the entire y axis in one go
                        colors = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.Colors + indexStart.X + x,
                            lengthY);
                        speeds = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.TileSpeeds + indexStart.X + x,
                            lengthY);

                        for (y = 0; y < lengthY; y++)
                        {
                            nodes[x, y] = new Node()
                            {
                                Color = colors[y],
                                Speed = speeds[y]
                            };
                        }
                    }
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(this.MapFile.OpenRead()))
                    {
                        for (x = 0; x < lengthX; x++)
                        {
                            // read the entire y axis in one go
                            int index = this.GetIndex(indexStart.X + x, indexStart.Y);
                            // set stream position to color index
                            reader.BaseStream.Position = index;
                            colors = reader.ReadBytes(lengthY);
                            // set stream position to speed index
                            reader.BaseStream.Position = this.Client.Addresses.MiniMap.DataLength + index;
                            speeds = reader.ReadBytes(lengthY);

                            for (y = 0; y < lengthY; y++)
                            {
                                nodes[x, y] = new Node()
                                {
                                    Color = colors[y],
                                    Speed = speeds[y]
                                };
                            }
                        }
                    }
                }

                return nodes;
            }
            public bool ContainsLocation(Location worldLocation)
            {
                if (worldLocation.Z != this.WorldLocation.Z) return false;
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                return this.MiniMapLocation == new Objects.Location(worldLocation.X / multiplier,
                    worldLocation.Y / multiplier, worldLocation.Z);
            }

            private int GetIndex(int x, int y)
            {
                return y + x * this.Client.Addresses.MiniMap.AxisLength;
            }
        }
    }
}
