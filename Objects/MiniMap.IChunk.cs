using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        /// <summary>
        /// An interface that represents a chunk of minimap data.
        /// </summary>
        public interface IChunk
        {
            Objects.Client Client { get; }
            int Address { get; }
            FileInfo MapFile { get; }
            bool IsInMemory { get; }
            Objects.Location WorldLocation { get; }
            Objects.Location MiniMapLocation { get; }
            int Left { get; }
            int Right { get; }
            int Bottom { get; }
            int Top { get; }
            ChunkTypes Type { get; }

            Node GetNode(int x, int y);
            Node[,] GetNodes();
            Node[,] GetNodes(Objects.Location start, Objects.Location end);
            bool ContainsLocation(Location worldLocation);
        }
    }
}
