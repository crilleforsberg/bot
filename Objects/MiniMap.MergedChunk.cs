using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        public class MergedChunk : IChunk
        {
            public MergedChunk(Objects.Client client, Objects.Location worldLocation, Node[,] nodes)
            {
                this.Client = client;
                this.WorldLocation = worldLocation;
                this.MiniMapLocation = new Location(this.WorldLocation.X / this.Client.Addresses.MiniMap.WorldLocationMultiplier,
                    this.WorldLocation.Y / this.Client.Addresses.MiniMap.WorldLocationMultiplier, this.WorldLocation.Z);
                this.Nodes = nodes;
                this.Type = ChunkTypes.Merged;
            }

            public Objects.Client Client { get; private set; }
            public Objects.Location WorldLocation { get; private set; }
            public Location MiniMapLocation { get; private set; }
            public Node[,] Nodes { get; private set; }
            public ChunkTypes Type { get; private set; }
            public int Address { get; private set; }
            public FileInfo MapFile { get; private set; }
            public bool IsInMemory { get; private set; }
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
                    int length = this.Nodes.GetLength(0);
                    return this.WorldLocation.Y + (length > 0 ? length - 1 : 0);
                }
            }

            public Node GetNode(int x, int y)
            {
                if (x < 0 || x >= this.Nodes.GetLength(0)) throw new IndexOutOfRangeException("x is out of range");
                if (y < 0 || y >= this.Nodes.GetLength(1)) throw new IndexOutOfRangeException("y is out of range");

                return this.Nodes[x, y];
            }
            public Node[,] GetNodes()
            {
                return this.Nodes;
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
                for (int x = 0; x < lengthX; x++)
                {
                    for (int y = 0; y < lengthY; y++)
                    {
                        nodes[x, y] = this.Nodes[indexStart.X + x, indexStart.Y + y];
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
        }
    }
}
