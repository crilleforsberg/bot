using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that handles local pathfinding, as in the limits of the game window.
    /// </summary>
    public class LocalPathFinder : Objects.PathFinder
    {
        public LocalPathFinder(Objects.Client client, byte heuristicEstimate = 2, bool allowDiagonals = true,
            bool penalizeDiagonals = true, float diagonalPenaltyMultiplier = 3, bool considerUnexploredAsUnwalkable = true)
            : base((ushort)Constants.Map.MaxX, (ushort)Constants.Map.MaxY, heuristicEstimate,
            allowDiagonals, penalizeDiagonals, diagonalPenaltyMultiplier, considerUnexploredAsUnwalkable)
        {
            this.Client = client;
            this.SyncObject = new object();
        }

        public Objects.Client Client { get; private set; }
        private object SyncObject { get; set; }

        public IEnumerable<Node> FindPath(Objects.Location start, Objects.Location end,
            Map.TileCollection tiles, Objects.MiniMap.MergedChunk mergedChunk,
            bool considerPlayerWalkable = false, bool considerCreatureOnLocationWalkable = false)
        {
            if (start == null || end == null || tiles == null || mergedChunk == null) return Enumerable.Empty<Node>();

            Objects.Location playerLoc = this.Client.Player.Location;
            Objects.Location topLeft = playerLoc.Offset(-Constants.Map.MemoryLocationCenterX, -Constants.Map.MemoryLocationCenterY),
                bottomRight = playerLoc.Offset(Constants.Map.MemoryLocationCenterX, Constants.Map.MemoryLocationCenterY);
            if (start.Z != end.Z ||
                start.X < topLeft.X || start.Y < topLeft.Y || start.X > bottomRight.X || start.Y > bottomRight.Y ||
                end.X < topLeft.X || end.Y < topLeft.Y || end.X > bottomRight.X || end.Y > bottomRight.Y)
            {
                return Enumerable.Empty<Node>();
            }

            var nodes = mergedChunk.GetNodes();
            Point gridLength = new Point(this.Grid.GetLength(0), this.Grid.GetLength(1)),
                nodeStartIndex = new Point(Math.Max(0, mergedChunk.WorldLocation.X - topLeft.X),
                    Math.Max(0, mergedChunk.WorldLocation.Y - topLeft.Y)),
                nodeEndIndex = new Point(Math.Min(gridLength.X, nodeStartIndex.X + nodes.GetLength(0)),
                    Math.Min(gridLength.Y, nodeStartIndex.Y + nodes.GetLength(1))),
                alignedStart = new Point(start.X - topLeft.X, start.Y - topLeft.Y),
                alignedEnd = new Point(end.X - topLeft.X, end.Y - topLeft.Y);

            lock (this.SyncObject)
            {
                // assign node values to pathfinder grid
                for (int x = 0; x < gridLength.X; x++)
                {
                    for (int y = 0; y < gridLength.Y; y++)
                    {
                        if (considerPlayerWalkable)
                        {
                            var tile = tiles.GetTile(topLeft.Offset(x, y));
                            if (tile != null && tile.ContainsCreature(this.Client.Player.ID))
                            {
                                this.Grid[x, y] = 1;
                                continue;
                            }
                        }

                        if (nodeStartIndex.X <= x && nodeEndIndex.X > x &&
                            nodeStartIndex.Y <= y && nodeEndIndex.Y > y)
                        {
                            Objects.MiniMap.Node node = nodes[x - nodeStartIndex.X, y - nodeStartIndex.Y];
                            this.Grid[x, y] = this.Client.MiniMap.IsTileWalkable(node.Color, node.Speed,
                                alignedEnd.X == x && alignedEnd.Y == y, this.ConsiderUnexploredAsUnwalkable) ?
                                node.Speed :
                                (byte)Enums.MiniMapSpeedValues.Unwalkable;
                        }
                        else
                        {
                            this.Grid[x, y] = (byte)Enums.MiniMapSpeedValues.Unwalkable;
                        }
                    }
                }

                return this.FindPath(alignedStart, alignedEnd);
            }
        }
    }
}
