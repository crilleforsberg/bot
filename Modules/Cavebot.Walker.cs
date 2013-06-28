using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KarelazisBot.Objects;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A class that simplifies the handling of waypoints.
        /// </summary>
        public class Walker
        {
            #region constructors
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            public Walker(Cavebot parent)
            {
                this.Parent = parent;
                this.ResetEvent = new ManualResetEventSlim();
                this.ResetEventTilesUpdated = new AutoResetEvent(false);
            }
            #endregion

            #region properties
            /// <summary>
            /// Gets the Cavebot module that holds this walker.
            /// </summary>
            public Cavebot Parent { get; private set; }
            /// <summary>
            /// Gets whether this walker is currently executing a waypoint.
            /// </summary>
            public bool IsRunning
            {
                get { return this.ResetEvent.IsSet; }
            }

            /// <summary>
            /// Gets or sets the thread used for executing the waypoint.
            /// </summary>
            private Thread DedicatedThread { get; set; }
            private ManualResetEventSlim ResetEvent { get; set; }
            private AutoResetEvent ResetEventTilesUpdated { get; set; }
            private Waypoint CurrentWaypoint { get; set; }
            private Map.TileCollection CachedTiles { get; set; }
            private bool Cancel { get; set; }
            #endregion

            #region events
            public delegate void WaypointHandler(Waypoint wp, bool success = false);
            /// <summary>
            /// An event that gets raised when the execution of a waypoint begins.
            /// </summary>
            public event WaypointHandler WaypointExecutedBegin;
            /// <summary>
            /// An event that gets raised when the execution of a waypoint ends.
            /// </summary>
            public event WaypointHandler WaypointExecutedEnd;
            public delegate void ErrorHandler(Exception ex);
            /// <summary>
            /// An event that gets raised when an error occurrs.
            /// </summary>
            public event ErrorHandler ErrorOccurred;
            #endregion

            #region methods
            /// <summary>
            /// Starts executing a waypoint.
            /// </summary>
            /// <param name="wp">The waypoint to execute.</param>
            /// <param name="cachedTiles">The cached tiles to use for pathfinding.</param>
            /// <returns></returns>
            public bool ExecuteWaypoint(Waypoint wp, Map.TileCollection cachedTiles)
            {
                // check if thread is busy
                if (this.ResetEvent.IsSet) return false;

                this.CurrentWaypoint = wp;

                if (this.DedicatedThread == null || !this.DedicatedThread.IsAlive)
                {
                    this.DedicatedThread = new Thread(this.Run);
                    this.DedicatedThread.Start();
                }

                this.UpdateCache(cachedTiles);
                this.ResetEvent.Set();
                return true;
            }
            /// <summary>
            /// Cancels the execution.
            /// </summary>
            public void CancelExecution()
            {
                if (!this.ResetEvent.IsSet) return;
                this.Cancel = true;
                this.ResetEventTilesUpdated.Set();
            }
            /// <summary>
            /// Updates this walker's cache.
            /// </summary>
            /// <param name="tileCollection">A collection of tiles to use for pathfinding.</param>
            public void UpdateCache(Map.TileCollection tileCollection)
            {
                this.CachedTiles = tileCollection;
                this.ResetEventTilesUpdated.Set();
            }

            private void Run()
            {
                while (true)
                {
                    try
                    {
                        this.ResetEvent.Wait();
                        if (this.WaypointExecutedBegin != null) this.WaypointExecutedBegin(this.CurrentWaypoint);

                        bool success = false,
                            firstRun = true;
                        Objects.Location currentSubNode = Objects.Location.Invalid;

                        while (!this.Cancel)
                        {
                            this.ResetEventTilesUpdated.WaitOne();

                            if (this.Cancel) break;

                            Objects.Player player = this.Parent.Client.Player;
                            Objects.Location playerLoc = player.Location;

                            if (player.Z != this.CurrentWaypoint.Location.Z) break;

                            if (firstRun) firstRun = false;
                            else
                            {
                                if (this.Parent.Client.Window.StatusBar.GetText() == Enums.StatusBar.ThereIsNoWay)
                                {
                                    this.Parent.Client.Window.StatusBar.SetText(string.Empty);
                                    success = false;
                                    break;
                                }
                            }

                            bool isOnScreen = playerLoc.IsOnScreen(this.CurrentWaypoint.Location),
                                doBreak = false;
                            var tilesToLocation = isOnScreen ?
                                playerLoc.GetTilesToLocation(this.Parent.Client,
                                this.CurrentWaypoint.Location, this.CachedTiles, this.Parent.PathFinder)
                                .ToList<Objects.PathFinder.Node>() :
                                new List<Objects.PathFinder.Node>();

                            switch (this.CurrentWaypoint.Type)
                            {
                                case Waypoint.Types.Walk:
                                    #region walk
                                    if (playerLoc == this.CurrentWaypoint.Location)
                                    {
                                        doBreak = true;
                                        success = true;
                                        break;
                                    }
                                    if (isOnScreen && tilesToLocation.Count == 0)
                                    {
                                        doBreak = true;
                                        success = false;
                                        break;
                                    }
                                    if (!player.IsWalking || player.GoTo != this.CurrentWaypoint.Location)
                                    {
                                        player.GoTo = this.CurrentWaypoint.Location;
                                    }
                                    #endregion
                                    break;
                                case Waypoint.Types.Node:
                                    #region node
                                    if (playerLoc == this.CurrentWaypoint.Location)
                                    {
                                        doBreak = true;
                                        success = true;
                                        break;
                                    }
                                    if (isOnScreen && tilesToLocation.Count == 0)
                                    {
                                        doBreak = true;
                                        success = false;
                                        break;
                                    }

                                    // check if the player is already walking to a node
                                    if (player.IsWalking)
                                    {
                                        int range = this.Parent.CurrentSettings.NodeRadius;
                                        Objects.Location currentGoTo = player.GoTo;
                                        bool found = false;
                                        for (int x = -range; x <= range; x++)
                                        {
                                            for (int y = -range; y <= range; y++)
                                            {
                                                Objects.Location subNode = this.CurrentWaypoint.Location.Offset(x, y);
                                                if (currentGoTo != subNode) continue;
                                                // check distance to node
                                                if (isOnScreen)
                                                {
                                                    var tilesToSubNode = playerLoc.GetTilesToLocation(this.Parent.Client,
                                                        subNode, this.CachedTiles, this.Parent.PathFinder, true).ToArray();
                                                    if (tilesToSubNode.Length <= this.Parent.CurrentSettings.NodeSkipRange)
                                                    {
                                                        success = true;
                                                        doBreak = true;
                                                        break;
                                                    }
                                                }
                                                found = true;
                                                break;
                                            }
                                            if (found) break;
                                        }
                                        if (found) break;
                                    }
                                    else if (playerLoc.DistanceTo(this.CurrentWaypoint.Location) <= this.Parent.CurrentSettings.NodeSkipRange)
                                    {
                                        success = true;
                                        doBreak = true;
                                        break;
                                    }

                                    // find new node to walk to
                                    if (isOnScreen)
                                    {
                                        Map.Tile tile = this.CachedTiles.GetTile(this.CurrentWaypoint.Location);
                                        if (tile == null)
                                        {
                                            doBreak = true;
                                            success = false;
                                            break;
                                        }
                                        Map.TileCollection nearbyTiles = this.CachedTiles.GetNearbyTileCollection(tile,
                                            this.Parent.CurrentSettings.NodeRadius);
                                        Objects.Location loc = tilesToLocation.Count != 0 ?
                                            tile.WorldLocation :
                                            Objects.Location.Invalid;
                                        List<Map.Tile> goodTiles = new List<Map.Tile>();
                                        foreach (Map.Tile nearbyTile in nearbyTiles.GetTiles())
                                        {
                                            bool isReachable = playerLoc.CanReachLocation(this.Parent.Client,
                                                nearbyTile.WorldLocation, this.CachedTiles, this.Parent.PathFinder);
                                            if (isReachable) goodTiles.Add(nearbyTile);
                                        }
                                        if (goodTiles.Count > 0)
                                        {
                                            loc = goodTiles[new Random().Next(goodTiles.Count)].WorldLocation;
                                        }

                                        if (loc.IsValid()) player.GoTo = loc;
                                        else
                                        {
                                            doBreak = true;
                                            success = false;
                                            break;
                                        }
                                    }
                                    else if (this.Parent.CurrentSettings.UseAlternateNodeFinder)
                                    {
                                        int range = this.Parent.CurrentSettings.NodeRadius;
                                        Random rand = new Random();
                                        Objects.Location loc = this.CurrentWaypoint.Location;
                                        for (int x = -range; x <= range; x++)
                                        {
                                            for (int y = -range; y <= range; y++)
                                            {
                                                Objects.Location subNode = this.CurrentWaypoint.Location.Offset(
                                                    rand.Next(-range, range + 1), rand.Next(-range, range + 1));
                                                if (!this.Parent.Client.Modules.MapViewer.IsWalkable(subNode)) continue;
                                                loc = subNode;
                                                break;
                                            }
                                            if (loc != this.CurrentWaypoint.Location) break;
                                        }
                                        player.GoTo = loc;
                                    }
                                    else // use stored subnodes
                                    {
                                        Objects.Location loc = this.CurrentWaypoint.Location;
                                        if (this.CurrentWaypoint.NodeLocations.Count > 0)
                                        {
                                            int index = -1, newIndex = new Random().Next(-1, this.CurrentWaypoint.NodeLocations.Count);
                                            if (newIndex != index) loc = this.CurrentWaypoint.NodeLocations[newIndex];
                                        }
                                        player.GoTo = loc;
                                    }
                                    #endregion
                                    break;
                                case Waypoint.Types.Machete:
                                case Waypoint.Types.Pick:
                                case Waypoint.Types.Rope:
                                case Waypoint.Types.Shovel:
                                    #region tools
                                    // check if we're adjacent to the waypoint
                                    if (playerLoc.IsAdjacentTo(this.CurrentWaypoint.Location))
                                    {
                                        Objects.Item tool = null;
                                        switch (this.CurrentWaypoint.Type)
                                        {
                                            case Waypoint.Types.Machete:
                                                tool = this.Parent.Client.Inventory.GetItem(this.Parent.Client.ItemList.Tools.Machete);
                                                break;
                                            case Waypoint.Types.Pick:
                                                tool = this.Parent.Client.Inventory.GetItem(this.Parent.Client.ItemList.Tools.Pick);
                                                break;
                                            case Waypoint.Types.Rope:
                                                tool = this.Parent.Client.Inventory.GetItem(this.Parent.Client.ItemList.Tools.Rope);
                                                break;
                                            case Waypoint.Types.Shovel:
                                                tool = this.Parent.Client.Inventory.GetItem(this.Parent.Client.ItemList.Tools.Shovel);
                                                break;
                                        }
                                        if (tool == null)
                                        {
                                            doBreak = true;
                                            success = false;
                                            break;
                                        }
                                        Map.Tile tile = this.CachedTiles.GetTile(this.CurrentWaypoint.Location);
                                        if (tile == null)
                                        {
                                            success = false;
                                            doBreak = true;
                                            break;
                                        }
                                        Map.TileObject topItem = tile.GetTopUseItem(true);
                                        if (this.CurrentWaypoint.Type != Waypoint.Types.Rope)
                                        {
                                            tool.UseOnTileObject(topItem);
                                            success = true;
                                            doBreak = true;
                                        }
                                        else
                                        {
                                            if (topItem.StackIndex != 0)
                                            {
                                                // find a non-blocking adjacent tile
                                                var adjacentTiles = this.CachedTiles.GetAdjacentTileCollection(tile).GetTiles()
                                                    .ToList<Map.Tile>();
                                                Map.Tile bestTile = tile;
                                                foreach (Map.Tile t in adjacentTiles.ToArray())
                                                {
                                                    if (!t.IsWalkable()) adjacentTiles.Remove(t);
                                                }
                                                if (adjacentTiles.Count > 0)
                                                {
                                                    bestTile = adjacentTiles[new Random().Next(adjacentTiles.Count)];
                                                }
                                                topItem.Move(bestTile.ToItemLocation());
                                            }
                                            else
                                            {
                                                tool.UseOnTileObject(topItem);
                                                for (int i = 0; i < 3; i++)
                                                {
                                                    Thread.Sleep(100);
                                                    if (player.Z != this.CurrentWaypoint.Location.Z)
                                                    {
                                                        doBreak = true;
                                                        success = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                    else if (isOnScreen)
                                    {
                                        Map.Tile tile = this.CachedTiles.GetTile(this.CurrentWaypoint.Location);
                                        if (tile == null)
                                        {
                                            success = false;
                                            doBreak = true;
                                            break;
                                        }
                                        Map.TileCollection adjacentTiles = this.CachedTiles.GetAdjacentTileCollection(tile);
                                        Objects.Location bestLoc = Objects.Location.Invalid;
                                        int distance = int.MaxValue;
                                        foreach (Map.Tile adjTile in adjacentTiles.GetTiles())
                                        {
                                            if (!adjTile.IsWalkable()) continue;

                                            var tilesToAdjTile = playerLoc.GetTilesToLocation(this.Parent.Client,
                                                adjTile.WorldLocation, this.CachedTiles, this.Parent.PathFinder, true)
                                                .ToArray<Objects.PathFinder.Node>();

                                            if (tilesToAdjTile.Length == 0) continue;
                                            if (tilesToAdjTile.Length < distance)
                                            {
                                                bestLoc = adjTile.WorldLocation;
                                                distance = tilesToAdjTile.Length;
                                            }
                                        }
                                        if (bestLoc.IsValid()) player.GoTo = bestLoc;
                                        else
                                        {
                                            doBreak = true;
                                            success = false;
                                            break;
                                        }
                                    }
                                    else if (!player.IsWalking)
                                    {
                                        player.GoTo = this.CurrentWaypoint.Location;
                                    }
                                    #endregion
                                    break;
                                case Waypoint.Types.Ladder:
                                    #region ladder
                                    if (player.IsWalking) break;

                                    if (isOnScreen)
                                    {
                                        Map.Tile tile = this.CachedTiles.GetTile(this.CurrentWaypoint.Location);
                                        if (tile == null)
                                        {
                                            doBreak = true;
                                            success = false;
                                            break;
                                        }

                                        if (playerLoc.DistanceTo(this.CurrentWaypoint.Location) < 2)
                                        {
                                            Map.TileObject topItem = tile.GetTopUseItem(false);
                                            if (topItem == null)
                                            {
                                                doBreak = true;
                                                success = false;
                                                break;
                                            }
                                            for (int i = 0; i < 3; i++)
                                            {
                                                topItem.Use();
                                                Thread.Sleep(300);
                                                if (player.Z != this.CurrentWaypoint.Location.Z)
                                                {
                                                    doBreak = true;
                                                    success = true;
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                        // find suitable loc to walk to
                                        int distance = int.MaxValue;
                                        Objects.Location bestLoc = Objects.Location.Invalid;
                                        foreach (Map.Tile adjTile in this.CachedTiles.GetAdjacentTileCollection(tile).GetTiles())
                                        {
                                            if (!adjTile.IsWalkable()) continue;
                                            var tilesToAdjTile = playerLoc.GetTilesToLocation(this.Parent.Client,
                                                adjTile.WorldLocation, this.CachedTiles, this.Parent.PathFinder, true)
                                                .ToArray<Objects.PathFinder.Node>();
                                            if (tilesToAdjTile.Length == 0) continue;
                                            if (tilesToAdjTile.Length < distance)
                                            {
                                                distance = tilesToAdjTile.Length;
                                                bestLoc = adjTile.WorldLocation;
                                            }
                                        }
                                        if (bestLoc.IsValid())
                                        {
                                            player.GoTo = bestLoc;
                                        }
                                        else
                                        {
                                            doBreak = true;
                                            success = false;
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        player.GoTo = this.CurrentWaypoint.Location;
                                    }
                                    #endregion
                                    break;
                            }

                            if (doBreak) break;
                        }

                        if (this.WaypointExecutedEnd != null) this.WaypointExecutedEnd(this.CurrentWaypoint, success);
                        this.ResetEvent.Reset();
                    }
                    catch (Exception ex)
                    {
                        if (this.ErrorOccurred != null) this.ErrorOccurred(ex);
                    }
                }
            }
            #endregion
        }
    }
}
