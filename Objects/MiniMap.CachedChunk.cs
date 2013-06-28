using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        /// <summary>
        /// A class for a chunk of minimap data, which caches all content available.
        /// Useful for i.e. global pathfinding or displaying a minimap.
        /// </summary>
        public class CachedChunk : IChunk
        {
            /// <summary>
            /// Constructor for a minimap chunk in memory.
            /// </summary>
            /// <param name="client"></param>
            /// <param name="address"></param>
            public CachedChunk(Objects.Client client, int address)
            {
                this.Client = client;
                this.Address = address;
                this.IsInMemory = true;
                this.SyncObject = new object();
                this.Nodes = new Node[this.Client.Addresses.MiniMap.AxisLength, this.Client.Addresses.MiniMap.AxisLength];
                this.Nodes.Initialize();
                this.OldMiniMapLocation = new Objects.Location();
                this.Type = ChunkTypes.Cached;
            }
            /// <summary>
            /// Constructor for a minimap chunk on file.
            /// </summary>
            /// <param name="client"></param>
            /// <param name="fi"></param>
            public CachedChunk(Objects.Client client, FileInfo fi)
            {
                this.Client = client;
                this.MapFile = fi;
                string name = this.MapFile.Name.Remove(this.MapFile.Name.LastIndexOf(".map"));
                this.MiniMapLocation = new Objects.Location(ushort.Parse(name.Substring(0, 3)),
                    ushort.Parse(name.Substring(3, 3)), ushort.Parse(name.Substring(6, 2)));
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                this.WorldLocation = new Objects.Location(this.MiniMapLocation.X * multiplier,
                    this.MiniMapLocation.Y * multiplier, this.MiniMapLocation.Z);
                this.IsInMemory = false;
                this.SyncObject = new object();
                this.Nodes = new Node[this.Client.Addresses.MiniMap.AxisLength, this.Client.Addresses.MiniMap.AxisLength];
                this.Nodes.Initialize();
                this.OldMiniMapLocation = new Objects.Location();
                this.Type = ChunkTypes.Cached;
            }

            public Objects.Client Client { get; private set; }
            public int Address { get; private set; }
            public FileInfo MapFile { get; private set; }
            public bool IsInMemory { get; private set; }
            public Objects.Location MiniMapLocation { get; private set; }
            public Objects.Location WorldLocation { get; private set; }
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
                    int length = this.Nodes.GetLength(0);
                    return this.WorldLocation.X + (length > 0 ? length - 1 : 0);
                }
            }
            public int Bottom
            {
                get
                {
                    int length = this.Nodes.GetLength(1);
                    return this.WorldLocation.Y + (length > 0 ? length - 1 : 0);
                }
            }
            public Node[,] Nodes { get; private set; }
            private object SyncObject { get; set; }
            /// <summary>
            /// Used to determine whether this chunk needs to update.
            /// </summary>
            private Objects.Location OldMiniMapLocation { get; set; }
            private bool FirstRun { get; set; }

            public Node GetNode(int x, int y)
            {
                return this.Nodes[x, y];
            }
            public Node[,] GetNodes()
            {
                this.UpdateData();
                return this.Nodes;
            }
            public Node[,] GetNodes(Location start, Location end)
            {
                if (!this.ContainsLocation(start)) throw new IndexOutOfRangeException("start is out of range");
                if (!this.ContainsLocation(end)) throw new IndexOutOfRangeException("end is out of range");

                this.UpdateData();
                Point indexStart = new Point(Math.Min(start.X - this.WorldLocation.X, end.X - this.WorldLocation.X),
                    Math.Min(start.Y - this.WorldLocation.Y, end.Y - this.WorldLocation.Y));
                Point indexEnd = new Point(Math.Max(start.X - this.WorldLocation.X, end.X - this.WorldLocation.X),
                    Math.Max(start.Y - this.WorldLocation.Y, end.Y - this.WorldLocation.Y));
                int lengthX = indexEnd.X - indexStart.X + 1,
                    lengthY = indexEnd.Y - indexStart.Y + 1;
                var nodes = new Node[lengthX, lengthY];
                for (int x = 0; x < lengthX; x++)
                {
                    for (int y = 0; y < lengthY; y++)
                    {
                        nodes[x, y] = this.Nodes[indexStart.X + x, indexStart.Y + y];
                    }
                }
                return nodes;
            }
            public void UpdateData(bool updateSeenPartsOnly = true)
            {
                lock (this.SyncObject)
                {
                    if (this.IsInMemory)
                    {
                        this.MiniMapLocation = new Objects.Location(this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.MiniMap.Distances.X),
                            this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.MiniMap.Distances.Y),
                            this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.MiniMap.Distances.Z));

                        this.WorldLocation = new Objects.Location(this.MiniMapLocation.X * this.Client.Addresses.MiniMap.WorldLocationMultiplier,
                            this.MiniMapLocation.Y * this.Client.Addresses.MiniMap.WorldLocationMultiplier,
                            this.MiniMapLocation.Z);
                    }

                    if (!this.IsValid()) return;

                    byte[] colors = null;
                    byte[] speeds = null;

                    if (this.IsInMemory)
                    {
                        // read entire chunks at once instead of byte-per-byte
                        // colors are very unlikely to change, so skip if already loaded
                        if (this.MiniMapLocation != this.OldMiniMapLocation)
                        {
                            colors = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.Colors,
                                this.Client.Addresses.MiniMap.DataLength);
                        }
                        speeds = this.Client.Memory.ReadBytes(this.Address + this.Client.Addresses.MiniMap.Distances.TileSpeeds,
                            this.Client.Addresses.MiniMap.DataLength);
                    }
                    else if (this.MiniMapLocation != this.OldMiniMapLocation)
                    {
                        this.MapFile.Refresh();
                        if (!this.MapFile.Exists) return;

                        using (BinaryReader reader = new BinaryReader(this.MapFile.OpenRead()))
                        {
                            // read colors
                            colors = reader.ReadBytes(this.Client.Addresses.MiniMap.DataLength);
                            // read speeds
                            speeds = reader.ReadBytes(this.Client.Addresses.MiniMap.DataLength);
                        }
                    }
                    else return;

                    // parse the chunk
                    int lengthX = this.Nodes.GetLength(0),
                        lengthY = this.Nodes.GetLength(1);
                    int startX = 0, startY = 0;

                    if (updateSeenPartsOnly && !this.FirstRun)
                    {
                        Objects.Location playerLoc = this.Client.Player.Location;
                        int maxX = Constants.Map.MaxX,
                            maxY = Constants.Map.MaxY;
                        Rectangle playerViewport = new Rectangle(playerLoc.X - Constants.Map.MemoryLocationCenterX,
                            playerLoc.Y - Constants.Map.MemoryLocationCenterY, maxX, maxY);
                        Objects.Location topLeft = new Objects.Location(playerViewport.Left, playerViewport.Top, playerLoc.Z),
                            topRight = new Objects.Location(playerViewport.Right, playerViewport.Top, playerLoc.Z),
                            bottomRight = new Objects.Location(playerViewport.Right, playerViewport.Bottom, playerLoc.Z),
                            bottomLeft = new Objects.Location(playerViewport.Left, playerViewport.Bottom, playerLoc.Z);

                        if (this.ContainsLocation(topLeft))
                        {
                            startX = this.Right - topLeft.X;
                            startY = this.Bottom - topLeft.Y;
                        }
                        else if (this.ContainsLocation(topRight))
                        {
                            startX = topRight.X - this.Left;
                            startY = this.Bottom - topRight.Y;
                        }
                        else if (this.ContainsLocation(bottomRight))
                        {
                            startX = bottomRight.X - this.Left;
                            startY = this.Top - bottomRight.Y;
                        }
                        else if (this.ContainsLocation(bottomLeft))
                        {
                            startX = this.Right - bottomLeft.X;
                            startY = this.Bottom - bottomLeft.Y;
                        }
                        else return;
                    }

                    int endX = updateSeenPartsOnly ? startX + Constants.Map.MaxX : lengthX,
                        endY = updateSeenPartsOnly ? startY + Constants.Map.MaxY : lengthY;
                    if (endX > lengthX) endX = lengthX;
                    if (endY > lengthY) endY = lengthY;
                    int index = startY > 0 ? startX * lengthX + startY : 0;
                    bool parseColors = colors != null,
                        parseSpeeds = speeds != null;
                    // segregate to improve performance for big chunks
                    if (updateSeenPartsOnly)
                    {
                        for (int x = startX; x < endX; x++)
                        {
                            for (int y = startY; y < endY; y++)
                            {
                                if (parseColors) this.Nodes[x, y].Color = colors[index];
                                if (parseSpeeds) this.Nodes[x, y].Speed = speeds[index];
                                index++;
                            }
                            if (startY != 0 || endY != lengthY) index += lengthY - endY + startY;
                        }
                    }
                    else
                    {
                        for (int x = startX; x < endX; x++)
                        {
                            for (int y = startY; y < endY; y++)
                            {
                                this.Nodes[x, y].Color = colors[index];
                                this.Nodes[x, y].Speed = speeds[index];
                                index++;
                            }
                        }
                    }

                    this.OldMiniMapLocation = this.MiniMapLocation;
                    this.FirstRun = false;
                }
            }
            public bool ContainsLocation(Objects.Location worldLocation)
            {
                if (worldLocation.Z != this.WorldLocation.Z) return false;
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                return this.MiniMapLocation == new Objects.Location(worldLocation.X / multiplier,
                    worldLocation.Y / multiplier, worldLocation.Z);
            }
            public bool IsAdjacentTo(CachedChunk chunk)
            {
                if (chunk == null) return false;
                return this.IsAdjacentTo(chunk.MiniMapLocation);
            }
            public bool IsAdjacentTo(Objects.Location miniMapLocation)
            {
                return this.MiniMapLocation.IsAdjacentNonDiagonalOnly(miniMapLocation);
            }
            public bool IsValid()
            {
                if (this.MiniMapLocation.X == this.Client.Addresses.MiniMap.NotYetLoadedValue) return false;
                if (!this.IsInMemory)
                {
                    if (this.MapFile == null) return false;
                    this.MapFile.Refresh();
                    if (!this.MapFile.Exists) return false;
                }
                return true;
            }
        }
    }
}
