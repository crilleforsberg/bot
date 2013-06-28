// NOTE: All code regarding the minimap is not finished, and will contain bugs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        public MiniMap(Objects.Client client)
        {
            this.Client = client;
            /*this.CachedChunksInMemory = new CachedChunk[this.Client.Addresses.MiniMap.MaxEntries];
            // also cache the same amount of chunks on file
            this.CachedChunksOnFile = new CachedChunk[this.Client.Addresses.MiniMap.MaxEntries];

            // load chunks from memory for caching, as rapidly creating and deleting objects of this size is taxing
            int index = 0;
            for (int i = this.Client.Addresses.MiniMap.Start;
                i < this.Client.Addresses.MiniMap.End;
                i += this.Client.Addresses.MiniMap.Step)
            {
                this.CachedChunksInMemory[index] = new CachedChunk(this.Client, i);
                index++;
            }*/
        }

        public Objects.Client Client { get; private set; }
        //private CachedChunk[] CachedChunksInMemory { get; set; }
        //private CachedChunk[] CachedChunksOnFile { get; set; }

        public enum ChunkTypes
        {
            Fast,
            Cached,
            Merged
        }

        public IEnumerable<Objects.Location> GetCurrentlyLoadedChunkLocations()
        {
            for (int i = this.Client.Addresses.MiniMap.Start;
                i < this.Client.Addresses.MiniMap.End;
                i += this.Client.Addresses.MiniMap.Step)
            {
                var loc = new Objects.Location(this.Client.Memory.ReadUInt16(i + this.Client.Addresses.MiniMap.Distances.X),
                    this.Client.Memory.ReadUInt16(i + this.Client.Addresses.MiniMap.Distances.Y),
                    this.Client.Memory.ReadByte(i + this.Client.Addresses.MiniMap.Distances.Z));
                if (loc.X == this.Client.Addresses.MiniMap.NotYetLoadedValue) continue; // chunk not loaded yet
                yield return loc;
            }
        }
        public bool ExistsInMemory(Objects.Location miniMapLocation, ref int address)
        {
            for (int i = this.Client.Addresses.MiniMap.Start;
                i < this.Client.Addresses.MiniMap.End;
                i += this.Client.Addresses.MiniMap.Step)
            {
                var loc = new Objects.Location(this.Client.Memory.ReadUInt16(i + this.Client.Addresses.MiniMap.Distances.X),
                    this.Client.Memory.ReadUInt16(i + this.Client.Addresses.MiniMap.Distances.Y),
                    this.Client.Memory.ReadByte(i + this.Client.Addresses.MiniMap.Distances.Z));
                if (loc.X == this.Client.Addresses.MiniMap.NotYetLoadedValue) continue; // chunk not loaded yet
                if (loc != miniMapLocation) continue;
                address = i;
                return true;
            }
            return false;
        }
        public IEnumerable<string> GetCurrentlyLoadedChunkNames()
        {
            foreach (Objects.Location loc in this.GetCurrentlyLoadedChunkLocations())
            {
                yield return string.Concat(string.Format("{0:D3}{1:D3}{2:D2}", loc.X, loc.Y, loc.Z), ".map");
            }
        }
        public Objects.Location GetAlignedMiniMapLocation(Objects.Location worldLocation)
        {
            int multiplier = this.Client.Addresses.MiniMap.WorldLocationMultiplier;
            return new Objects.Location((ushort)(worldLocation.X / multiplier),
                (ushort)(worldLocation.Y / multiplier), worldLocation.Z);
        }
        public FileInfo GetMapFile(Objects.Location minimapLocation)
        {
            return this.GetMapFile(string.Concat(string.Format("{0:D3}{1:D3}{2:D2}",
                minimapLocation.X, minimapLocation.Y, minimapLocation.Z),
                ".map"));
        }
        public FileInfo GetMapFile(string mapFileName)
        {
            FileInfo fi = new FileInfo(Path.Combine(this.Client.AutomapDirectory.FullName, mapFileName));
            if (!fi.Exists) return null;
            return fi;
        }
        public IEnumerable<FileInfo> GetMapFiles(bool considerOpenTibiaFiles = false)
        {
            ushort minX = 100;
            foreach (FileInfo fi in this.Client.AutomapDirectory.GetFiles("*.map"))
            {
                ushort x = 0;
                if (!ushort.TryParse(fi.Name.Substring(0, 3), out x)) continue;
                if (!considerOpenTibiaFiles && x < minX) continue;
                yield return fi;
            }
        }
        public IEnumerable<FileInfo> GetMapFiles(Objects.Location worldStart, Objects.Location worldEnd)
        {
            Objects.Location minimapStart = this.GetAlignedMiniMapLocation(
                new Objects.Location(Math.Min(worldStart.X, worldEnd.X),
                    Math.Min(worldStart.Y, worldEnd.Y),
                    Math.Min(worldStart.Z, worldEnd.Z)));
            Objects.Location minimapEnd = this.GetAlignedMiniMapLocation(
                new Objects.Location(Math.Max(worldStart.X, worldEnd.X),
                    Math.Max(worldStart.Y, worldEnd.Y),
                    Math.Max(worldStart.Z, worldEnd.Z)));

            for (int z = 0; z <= minimapEnd.Z - minimapStart.Z; z++)
            {
                for (int y = 0; y <= minimapEnd.Y - minimapStart.Y; y++)
                {
                    for (int x = 0; x <= minimapEnd.X - minimapStart.X; x++)
                    {
                        FileInfo fi = this.GetMapFile(minimapStart.Offset(x, y, z));
                        if (fi != null) yield return fi;
                    }
                }
            }
        }
        public ChunkCollection GetChunksInMemory(ChunkTypes desiredType, bool considerUnloadedChunks = false)
        {
            if (desiredType == ChunkTypes.Merged) throw new Exception("desiredType can not be Merged");

            var chunks = new List<IChunk>(this.Client.Addresses.MiniMap.MaxEntries);
            int index = -1;
            for (int i = this.Client.Addresses.MiniMap.Start;
                i < this.Client.Addresses.MiniMap.End;
                i += this.Client.Addresses.MiniMap.Step)
            {
                index++;
                if (!considerUnloadedChunks)
                {
                    ushort x = this.Client.Memory.ReadUInt16(i + this.Client.Addresses.MiniMap.Distances.X);
                    if (x == this.Client.Addresses.MiniMap.NotYetLoadedValue) continue; // chunk not loaded yet
                }
                switch (desiredType)
                {
                    case ChunkTypes.Cached:
                        chunks.Add(new CachedChunk(this.Client, i));
                        break;
                    case ChunkTypes.Fast:
                        chunks.Add(new FastChunk(this.Client, i));
                        break;
                }
                //this.CachedChunksInMemory[index].UpdateData();
                //chunks.Add(this.CachedChunksInMemory[index]);
            }
            return new ChunkCollection(this.Client, chunks);
        }
        public ChunkCollection GetChunksInMemory(ChunkTypes desiredType, Objects.Location start, Objects.Location end)
        {
            if (desiredType == ChunkTypes.Merged) throw new Exception("desiredType can not be Merged");

            return this.GetChunksInMemory(desiredType).GetChunkCollection(start, end);
        }
        public ChunkCollection GetChunksFromFiles(ChunkTypes desiredType, Objects.Location start, Objects.Location end)
        {
            if (desiredType == ChunkTypes.Merged) throw new Exception("desiredType can not be Merged");

            var chunks = new List<IChunk>();
            foreach (FileInfo fi in this.GetMapFiles(start, end))
            {
                switch (desiredType)
                {
                    case ChunkTypes.Cached:
                        chunks.Add(new CachedChunk(this.Client, fi));
                        break;
                    case ChunkTypes.Fast:
                        chunks.Add(new FastChunk(this.Client, fi));
                        break;
                }
            }
            return new ChunkCollection(this.Client, chunks);
        }
        public ChunkCollection GetCombinedChunks(ChunkTypes desiredType, Objects.Location start, Objects.Location end)
        {
            if (desiredType == ChunkTypes.Merged) throw new Exception("desiredType can not be Merged");

            ChunkCollection chunks = this.GetChunksInMemory(desiredType, start, end);
            if (chunks.IsEmpty()) return this.GetChunksFromFiles(desiredType, start, end);

            Objects.Location minimapStart = this.GetAlignedMiniMapLocation(new Objects.Location(Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z)));
            Objects.Location minimapEnd = this.GetAlignedMiniMapLocation(new Objects.Location(Math.Max(start.X, end.X),
                Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z)));

            // add chunks from files if necessary
            for (int z = 0; z <= minimapEnd.Z - minimapStart.Z; z++)
            {
                for (int y = 0; y <= minimapEnd.Y - minimapStart.Y; y++)
                {
                    for (int x = 0; x <= minimapEnd.X - minimapStart.X; x++)
                    {
                        Objects.Location loc = minimapStart.Offset(x, y, z);
                        if (chunks.GetChunkFromMiniMapLocation(loc) != null) continue; // chunk found, skip
                        FileInfo fi = this.GetMapFile(loc);
                        if (!fi.Exists) continue;
                        switch (desiredType)
                        {
                            case ChunkTypes.Cached:
                                chunks.AddChunk(new CachedChunk(this.Client, fi));
                                break;
                            case ChunkTypes.Fast:
                                chunks.AddChunk(new FastChunk(this.Client, fi));
                                break;
                        }
                    }
                }
            }

            return chunks;
        }
        public MergedChunk GetMergedChunk(Objects.Location start, Objects.Location end)
        {
            Objects.Location minimapStart = this.GetAlignedMiniMapLocation(new Objects.Location(Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z)));
            Objects.Location minimapEnd = this.GetAlignedMiniMapLocation(new Objects.Location(Math.Max(start.X, end.X),
                Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z)));

            // check if only one chunk if necessary
            if (minimapStart == minimapEnd)
            {
                int address = 0;
                if (this.ExistsInMemory(minimapStart, ref address))
                {
                    return new MergedChunk(this.Client, start, new FastChunk(this.Client, address).GetNodes(start, end));
                }
                FileInfo fi = null;
                if ((fi = this.GetMapFile(minimapStart)) != null)
                {
                    return new MergedChunk(this.Client, start, new FastChunk(this.Client, fi).GetNodes(start, end));
                }

                return null;
            }

            // get several chunks
            return this.GetCombinedChunks(ChunkTypes.Fast, start, end).GetMergedChunk(start, end);
        }
        public bool IsTileWalkable(byte color, byte speed, bool isDestination = false, bool considerUnexplored = false)
        {
            if (considerUnexplored && color == (byte)Enums.MiniMapColorValues.Unexplored) return true;
            if (speed == (byte)Enums.MiniMapSpeedValues.Unwalkable)
            {
                if (isDestination && color == (byte)Enums.MiniMapColorValues.FloorChange) return true;
                return false;
            }
            return true;
        }
        public Color GetAutomapColor(byte color)
        {
            switch ((Enums.MiniMapColorValues)color)
            {
                case Enums.MiniMapColorValues.Foliage: // dark green
                    return Color.FromArgb(0, 0x66, 0);
                case Enums.MiniMapColorValues.Grass: // green
                    return Color.FromArgb(0, 0xcc, 0);
                case Enums.MiniMapColorValues.Swamp: // bright green
                    return Color.FromArgb(0, 0xFF, 0);
                case Enums.MiniMapColorValues.Water: // blue
                    return Color.FromArgb(0x33, 0, 0xcc);
                case Enums.MiniMapColorValues.StoneWall: // dark grey
                    return Color.FromArgb(0x66, 0x66, 0x66);
                case Enums.MiniMapColorValues.UnknownUnwalkable: // maroon
                    return Color.FromArgb(0x99, 0x33, 0);
                case Enums.MiniMapColorValues.Dirt: // brown
                    return Color.FromArgb(0x99, 0x66, 0x33);
                case Enums.MiniMapColorValues.Path: // Paths, tile floors, other floors
                    return Color.FromArgb(0x99, 0x99, 0x99);
                case Enums.MiniMapColorValues.Ice: // light blue
                    return Color.FromArgb(0xcc, 0xff, 0xff);
                case Enums.MiniMapColorValues.Wall: // red
                    return Color.FromArgb(0xff, 0x33, 0);
                case Enums.MiniMapColorValues.Lava: // orange
                    return Color.FromArgb(0xff, 0x66, 0);
                case Enums.MiniMapColorValues.Sand: // tan
                    return Color.FromArgb(0xff, 0xcc, 0x99);
                case Enums.MiniMapColorValues.FloorChange: // yellow
                    return Color.FromArgb(0xff, 0xff, 0);
                case Enums.MiniMapColorValues.Unexplored: // black
                    return Color.Black;
                default: // Unknown, white
                    return Color.White;
            }
        }
    }
}
