using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace KarelazisBot.Objects
{
    public partial class PathFinder
    {
        public PathFinder(ushort width, ushort height, byte heuristicEstimate = 2, bool allowDiagonals = true,
            bool penalizeDiagonals = true, float diagonalPenaltyMultiplier = 3, bool considerUnexploredAsUnwalkable = true)
        {
            this.SyncObject = new object();
            this.Grid = new byte[width, height];

            this.DiagonalPenaltyMultiplier = diagonalPenaltyMultiplier;
            this.HeuristicEstimate = heuristicEstimate;
            this.AllowDiagonals = allowDiagonals;
            this.PenalizeDiagonals = penalizeDiagonals;
            this.ConsiderUnexploredAsUnwalkable = considerUnexploredAsUnwalkable;
        }

        public byte[,] Grid { get; private set; }
        public bool ConsiderUnexploredAsUnwalkable { get; set; }
        public bool AllowDiagonals { get; set; }
        public bool PenalizeDiagonals { get; set; }
        public float DiagonalPenaltyMultiplier { get; set; }
        public byte HeuristicEstimate { get; set; }
        public readonly ushort MaxGridLength = 0xFFFF;

        protected bool DoStop { get; set; }
        private object SyncObject { get; set; }
        private readonly sbyte[,] Directions = new sbyte[8, 2]
        {
            { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 },
            { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 }
        };

        /// <summary>
        /// A structure to find a path between two points. Contains information like XY position and travel cost.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct Node
        {
            /// <summary>
            /// Position of X axis.
            /// </summary>
            public ushort X;
            /// <summary>
            /// Position of Y axis.
            /// </summary>
            public ushort Y;
            /// <summary>
            /// The total travel cost thus far. Is normally referenced to as G.
            /// </summary>
            public ushort TotalTravelCost;
            /// <summary>
            /// The heuristic cost for this node. Is normally referenced to as H.
            /// </summary>
            public ushort HeuristicCost;
            /// <summary>
            /// The combined travel cost for this node (F=G+H). Is normally referenced to as F.
            /// </summary>
            public ushort CombinedTravelCost;
        }

        public void ResetGrid()
        {
            lock (this.SyncObject)
            {
                for (int y = 0; y < this.Grid.GetUpperBound(1); y++)
                {
                    for (int x = 0; x < this.Grid.GetUpperBound(0); x++)
                    {
                        this.Grid[x, y] = (byte)Enums.MiniMapSpeedValues.Unwalkable;
                    }
                }
            }
        }
        public void Stop()
        {
            this.DoStop = true;
        }
        public IEnumerable<Node> FindPath(Point start, Point end, bool stopWhenAdjacent = false)
        {
            if (start.X > this.MaxGridLength || start.X < 0 || start.Y > this.MaxGridLength || start.Y < 0 ||
                end.X > this.MaxGridLength || end.X < 0 || end.Y > this.MaxGridLength || end.Y < 0)
            {
                return Enumerable.Empty<Node>();
            }
            return this.FindPath((byte)start.X, (byte)start.Y, (byte)end.X, (byte)end.Y, stopWhenAdjacent);
        }
        public IEnumerable<Node> FindPath(Objects.Location start, Objects.Location end, bool stopWhenAdjacent = false)
        {
            if (start.Z != end.Z) throw new Exception("start.Z must be the same as end.Z");

            return this.FindPath(new Point(start.X, start.Y), new Point(end.X, end.Y), stopWhenAdjacent);
        }

        protected IEnumerable<Node> FindPath(ushort startX, ushort startY, ushort endX, ushort endY, bool stopWhenAdjacent = false)
        {
            lock (this.SyncObject)
            {
                this.DoStop = false;

                int gridWidth = this.Grid.GetUpperBound(0),
                    gridHeight = this.Grid.GetUpperBound(1);
                byte unexploredValue = (byte)Enums.MiniMapSpeedValues.Unexplored,
                    unwalkableValue = (byte)Enums.MiniMapSpeedValues.Unwalkable;
                PriorityQueue<Node> openNodes = new PriorityQueue<Node>();
                List<Node> closedNodes = new List<Node>();
                bool success = false;

                Node parentNode = new Node();
                parentNode.X = startX;
                parentNode.Y = startY;
                parentNode.TotalTravelCost = 0;
                parentNode.HeuristicCost = this.HeuristicEstimate;
                parentNode.CombinedTravelCost = (ushort)(parentNode.TotalTravelCost + parentNode.HeuristicCost);

                openNodes.Push(parentNode);

                while (openNodes.Count > 0 && !this.DoStop)
                {
                    parentNode = openNodes.Pop();
                    if ((parentNode.X == endX && parentNode.Y == endY) ||
                        (stopWhenAdjacent && Math.Max(Math.Abs(parentNode.X - endX), Math.Abs(parentNode.Y - endY)) <= 1))
                    {
                        closedNodes.Add(parentNode);
                        success = true;
                        break;
                    }

                    // check adjacent nodes
                    for (byte i = 0; i < (this.AllowDiagonals ? 8 : 4); i++)
                    {
                        Node newNode = new Node()
                        {
                            X = (ushort)(parentNode.X + this.Directions[i, 0]),
                            Y = (ushort)(parentNode.Y + this.Directions[i, 1])
                        };

                        // check if node is within bounds
                        if (newNode.X < 0 || newNode.X >= gridWidth || newNode.Y < 0 || newNode.Y >= gridHeight) continue;

                        byte speed = this.Grid[newNode.X, newNode.Y];
                        if ((this.ConsiderUnexploredAsUnwalkable && speed == unexploredValue) || speed == unwalkableValue) continue;

                        int totalCost = parentNode.TotalTravelCost + (i > 3 ? (ushort)(speed * (this.PenalizeDiagonals ? this.DiagonalPenaltyMultiplier : 1)) : speed);
                        if (totalCost == parentNode.TotalTravelCost) continue;

                        // check if node already exists in open list
                        int index = -1;
                        for (int j = 0; j < openNodes.Count; j++)
                        {
                            if (openNodes[j].X == newNode.X && openNodes[j].Y == newNode.Y)
                            {
                                index = j;
                                break;
                            }
                        }
                        if (index != -1 && openNodes[index].TotalTravelCost <= totalCost) continue;

                        // check if node already exists in closed list
                        index = -1;
                        for (int j = 0; j < closedNodes.Count; j++)
                        {
                            if (closedNodes[j].X == newNode.X && closedNodes[j].Y == newNode.Y)
                            {
                                index = j;
                                break;
                            }
                        }
                        if (index != -1 && closedNodes[index].TotalTravelCost <= totalCost) continue;

                        newNode.TotalTravelCost = (ushort)(totalCost > ushort.MaxValue ? ushort.MaxValue : totalCost);
                        newNode.HeuristicCost = (ushort)(this.HeuristicEstimate * (Math.Abs(newNode.X - endX) + Math.Abs(newNode.Y - endY)));
                        newNode.CombinedTravelCost = (ushort)(newNode.TotalTravelCost + newNode.HeuristicCost);

                        openNodes.Push(newNode);
                    }

                    closedNodes.Add(parentNode);
                }

                this.DoStop = false;

                if (success) return closedNodes.ToArray();
                else return Enumerable.Empty<Node>();
            }
        }
    }
}
