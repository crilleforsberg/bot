using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        public class ChunkCollection
        {
            public ChunkCollection(Objects.Client client, IEnumerable<IChunk> chunks)
            {
                this.Client = client;
                this.Chunks = chunks.ToList();
            }

            private List<IChunk> Chunks { get; set; }
            public Objects.Client Client { get; private set; }
            public int Count
            {
                get { return this.Chunks.Count; }
            }
            public IChunk this[int index]
            {
                get { return this.Chunks[index]; }
            }

            public IEnumerable<IChunk> GetChunks()
            {
                return this.Chunks.ToArray();
            }
            public bool IsEmpty()
            {
                return this.Count == 0;
            }
            public void AddChunk(IChunk chunk)
            {
                this.Chunks.Add(chunk);
            }
            public IChunk GetPlayerChunk()
            {
                return this.GetChunk(this.Client.Player.Location);
            }
            public IChunk GetChunk(Objects.Location worldLocation)
            {
                foreach (var chunk in this.GetChunks())
                {
                    if (chunk.ContainsLocation(worldLocation)) return chunk;
                }
                return null;
            }
            public IChunk GetChunkFromMiniMapLocation(Objects.Location miniMapLocation)
            {
                foreach (var chunk in this.GetChunks())
                {
                    if (chunk.MiniMapLocation == miniMapLocation) return chunk;
                }
                return null;
            }
            public IChunk GetAdjacentChunk(IChunk chunk, Enums.Direction direction)
            {
                if (chunk == null) return null;

                Objects.Location loc = null;
                int length = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                switch (direction)
                {
                    case Enums.Direction.Down:
                        loc = chunk.MiniMapLocation.Offset(y: length);
                        break;
                    case Enums.Direction.Left:
                        loc = chunk.MiniMapLocation.Offset(x: -length);
                        break;
                    case Enums.Direction.Right:
                        loc = chunk.MiniMapLocation.Offset(x: length);
                        break;
                    case Enums.Direction.Up:
                        loc = chunk.MiniMapLocation.Offset(y: -length);
                        break;
                }
                if (loc == null) return null;

                foreach (IChunk c in this.GetChunks())
                {
                    if (c.MiniMapLocation == loc) return c;
                }
                return null;
            }
            public ChunkCollection GetChunkCollection(Objects.Location start, Objects.Location end)
            {
                // get start and end chunks
                IChunk startChunk = null, endChunk = null;
                foreach (IChunk chunk in this.GetChunks())
                {
                    if (startChunk == null && chunk.ContainsLocation(start)) startChunk = chunk;
                    if (endChunk == null && chunk.ContainsLocation(end)) endChunk = chunk;

                    if (startChunk != null && endChunk != null) break;
                }

                // sanity checks
                if (startChunk == null || endChunk == null) return new ChunkCollection(this.Client, Enumerable.Empty<IChunk>());

                // check if both locations are in the same chunk
                if (startChunk == endChunk) return new ChunkCollection(this.Client, new IChunk[] { startChunk });

                // check if start and end chunks are adjacent
                if (startChunk.MiniMapLocation.IsAdjacentNonDiagonalOnly(endChunk.MiniMapLocation)) return new ChunkCollection(this.Client, new IChunk[] { startChunk, endChunk });

                // get connecting chunks
                List<IChunk> chunks = new List<IChunk>();
                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
                Location minimapStart = new Location(Math.Min(start.X / multiplier, end.X / multiplier),
                    Math.Min(start.Y / multiplier, end.Y / multiplier), Math.Min(start.Z, end.Z));
                Location minimapEnd = new Location(Math.Max(start.X / multiplier, end.X / multiplier),
                    Math.Max(start.Y / multiplier, end.Y / multiplier), Math.Max(start.Z, end.Z));
                for (int z = 0; z <= minimapEnd.Z - minimapStart.Z; z++)
                {
                    for (int y = 0; y <= minimapEnd.Y - minimapStart.Y; y++)
                    {
                        for (int x = 0; x <= minimapEnd.X - minimapStart.X; x++)
                        {
                            foreach (IChunk chunk in this.GetChunks())
                            {
                                if (chunk.MiniMapLocation != minimapStart.Offset(x, y, z)) continue;
                                chunks.Add(chunk);
                                break;
                            }
                        }
                    }
                }

                return new ChunkCollection(this.Client, chunks);
            }
            public MergedChunk GetMergedChunk(Objects.Location start, Objects.Location end)
            {
                if (start.Z != end.Z) throw new Exception("start.Z must be the same as end.Z");

                var chunkCollection = this.GetChunkCollection(start, end);

                int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier,
                    axisLength = this.Client.Addresses.MiniMap.AxisLength;
                Location minimapStart = new Location(Math.Min(start.X / multiplier, end.X / multiplier),
                    Math.Min(start.Y / multiplier, end.Y / multiplier), Math.Min(start.Z, end.Z));
                Location minimapEnd = new Location(Math.Max(start.X / multiplier, end.X / multiplier),
                    Math.Max(start.Y / multiplier, end.Y / multiplier), Math.Max(start.Z, end.Z));
                Location worldStart = new Location(minimapStart.X * multiplier, minimapStart.Y * multiplier, minimapStart.Z);
                Location worldEnd = new Location(minimapEnd.X * multiplier, minimapEnd.Y * multiplier, minimapEnd.Z);
                Location diffStart = start - worldStart;
                Location diffEnd = end - worldEnd;
                var nodes = new Node[Math.Abs(end.X - start.X) + 1, Math.Abs(end.Y - start.Y) + 1];

                var chunks = chunkCollection.GetChunks();
                for (int miniMapX = minimapStart.X; miniMapX <= minimapEnd.X; miniMapX++)
                {
                    for (int miniMapY = minimapStart.Y; miniMapY <= minimapEnd.Y; miniMapY++)
                    {
                        Location loc = minimapStart.Offset(miniMapX, miniMapY);
                        Location diff = loc - minimapStart;
                        int nodeIndexX = diff.X * multiplier,
                            nodeIndexY = diff.Y * multiplier,
                            chunkIndexX = 0, chunkIndexY = 0,
                            endX = axisLength, endY = axisLength;
                        if (loc == minimapStart)
                        {
                            chunkIndexX += diffStart.X;
                            chunkIndexY += diffStart.Y;
                        }
                        if (loc == minimapEnd)
                        {
                            endX -= diffEnd.X + chunkIndexX;
                            endY -= diffEnd.Y + chunkIndexY;
                        }

                        bool found = false;
                        foreach (IChunk chunk in chunks)
                        {
                            if (chunk.MiniMapLocation != loc) continue;

                            var chunkNodes = chunk.GetNodes();
                            for (int x = chunkIndexX; x < endX; x++)
                            {
                                for (int y = chunkIndexY; y < endY; y++)
                                {
                                    nodes[nodeIndexX, nodeIndexY] = chunkNodes[x, y];
                                    nodeIndexY++;
                                }
                                nodeIndexX++;
                            }
                        }

                        if (!found)
                        {
                            // create unexplored nodes
                            for (int x = chunkIndexX; x < endX; x++)
                            {
                                for (int y = chunkIndexY; y < endY; y++)
                                {
                                    nodes[nodeIndexX, nodeIndexY] = new Node()
                                    {
                                        Color = (byte)Enums.MiniMapColorValues.Unexplored,
                                        Speed = (byte)Enums.MiniMapSpeedValues.Unexplored
                                    };
                                    nodeIndexY++;
                                }
                                nodeIndexX++;
                            }
                        }
                    }
                }

                return new MergedChunk(this.Client, start, nodes);
            }
        }
    }
}
