using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using KarelazisBot.Objects;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A class that simplifies the handling of targets.
        /// </summary>
        public class Targeting
        {
            #region constructors
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            public Targeting(Cavebot parent)
            {
                this.Parent = parent;
                this.ResetEvent = new ManualResetEventSlim();
                this.ResetEventCacheUpdated = new AutoResetEvent(false);
                this.StopwatchExhaust = new Stopwatch();
                this.CreatureTimestamps = new Dictionary<uint, long>();
                this.CreatureWatchlist = new Dictionary<uint, Objects.Location>();

                
            }
            #endregion

            #region properties
            /// <summary>
            /// Gets the Cavebot module that holds this targeter.
            /// </summary>
            public Cavebot Parent { get; private set; }
            /// <summary>
            /// Gets whether this targeter is currently running.
            /// </summary>
            public bool IsRunning
            {
                get { return this.ResetEvent.IsSet; }
            }

            private Thread DedicatedThread { get; set; }
            private ManualResetEventSlim ResetEvent { get; set; }
            private AutoResetEvent ResetEventCacheUpdated { get; set; }
            private Target CurrentTarget { get; set; }
            private Map.TileCollection CachedTiles { get; set; }
            private IEnumerable<Objects.Creature> CachedCreatures { get; set; }
            private IEnumerable<Objects.Creature> CachedPlayers { get; set; }
            private bool Cancel { get; set; }
            private Stopwatch StopwatchExhaust { get; set; }
            /// <summary>
            /// Key = creature ID, value = creature location.
            /// </summary>
            private Dictionary<uint, Objects.Location> CreatureWatchlist { get; set; }
            /// <summary>
            /// Key = creature id, value = time of last seen alive
            /// </summary>
            private Dictionary<uint, long> CreatureTimestamps { get; set; }
            #endregion

            #region events
            public delegate void TargetHandler(Target t);
            /// <summary>
            /// An event that gets raised when the execution of a target begins.
            /// </summary>
            public event TargetHandler TargetExecutedBegin;
            /// <summary>
            /// An event that gets raised when the execution of a target ends.
            /// </summary>
            public event TargetHandler TargetExecutedEnd;
            public delegate void CreatureDiedHandler(Objects.Creature c);
            /// <summary>
            /// An event that gets raised when a creature dies.
            /// </summary>
            public event CreatureDiedHandler CreatureDied;
            public delegate void ErrorHandler(Exception ex);
            /// <summary>
            /// An event that gets raised when an error occurrs.
            /// </summary>
            public event ErrorHandler ErrorOccurred;
            #endregion

            #region methods
            /// <summary>
            /// Starts executing a target.
            /// </summary>
            /// <param name="t">The target to execute.</param>
            /// <param name="cachedTiles">A collection of tiles to use for pathfinding.</param>
            /// <param name="visibleCreatures">A collection of currently visible creatures.</param>
            /// <param name="visiblePlayers">A collection of currently visible players.</param>
            /// <returns></returns>
            public bool ExecuteTarget(Target t, Map.TileCollection cachedTiles,
                IEnumerable<Objects.Creature> visibleCreatures, IEnumerable<Objects.Creature> visiblePlayers)
            {
                // check if thread is busy
                if (this.ResetEvent.IsSet) return false;

                this.CurrentTarget = t;

                if (this.DedicatedThread == null || !this.DedicatedThread.IsAlive)
                {
                    this.DedicatedThread = new Thread(this.Run);
                    this.DedicatedThread.Start();
                }

                this.UpdateCache(cachedTiles, visibleCreatures, visiblePlayers);
                this.ResetEvent.Set();
                return true;
            }
            public IEnumerable<Objects.Creature> GetKilledCreatures(IEnumerable<Objects.Creature> creaturesOnSameFloor)
            {
                foreach (var c in creaturesOnSameFloor)
                {
                    uint id = c.ID;

                    if (c.IsVisible)
                    {
                        if (!this.Parent.CurrentSettings.KillBeforeLooting &&
                            this.Parent.Client.Player.Target != id)
                        {
                            continue;
                        }

                        if (!this.CreatureWatchlist.ContainsKey(id)) this.CreatureWatchlist.Add(id, c.Location);
                        if (!this.CreatureTimestamps.ContainsKey(id)) this.CreatureTimestamps.Add(id, Environment.TickCount);
                        else this.CreatureTimestamps[id] = Environment.TickCount;
                        continue;
                    }

                    if (this.CreatureTimestamps.ContainsKey(id))
                    {
                        if (Environment.TickCount - this.CreatureTimestamps[id] > 1000 * 5)
                        {
                            this.CreatureWatchlist.Remove(id);
                            this.CreatureTimestamps.Remove(id);
                            continue;
                        }
                    }
                    else
                    {
                        this.CreatureWatchlist.Remove(id);
                        continue;
                    }

                    if (c.IsDead && this.CreatureWatchlist.ContainsKey(id))
                    {
                        this.CreatureWatchlist[id] = c.Location;
                        this.CreatureWatchlist.Remove(id);
                        yield return c;
                    }
                }
            }
            /// <summary>
            /// Cancels execution.
            /// </summary>
            public void CancelExecution()
            {
                if (!this.ResetEvent.IsSet) return;
                this.Cancel = true;
                this.ResetEventCacheUpdated.Set();
            }
            /// <summary>
            /// Updates this targeter's cache.
            /// </summary>
            /// <param name="tileCollection">A collection of tiles to use for pathfinding.</param>
            /// <param name="visibleCreatures">A collection of currently visible creatures.</param>
            /// <param name="visiblePlayers">A collection of currently visible players.</param>
            public void UpdateCache(Map.TileCollection tileCollection, IEnumerable<Objects.Creature> visibleCreatures,
                IEnumerable<Objects.Creature> visiblePlayers)
            {
                this.CachedTiles = tileCollection;
                this.CachedCreatures = visibleCreatures;
                this.CachedPlayers = visiblePlayers;
                this.ResetEventCacheUpdated.Set();
            }

            private void Run()
            {
                while (true)
                {
                    try
                    {
                        this.ResetEvent.Wait();
                        if (this.TargetExecutedBegin != null) this.TargetExecutedBegin(this.CurrentTarget);

                        Objects.Client client = this.Parent.Client;
                        Objects.Creature oldCreature = null;

                        while (!this.Cancel)
                        {
                            this.ResetEventCacheUpdated.WaitOne();

                            if (this.Cancel) break;

                            // get best setting
                            Target.Setting setting = this.CurrentTarget.GetBestSetting(this.CachedCreatures,
                                this.CachedPlayers, this.CachedTiles, true);
                            if (setting == null) break;

                            // check if we should revert to older creature if sticky is on
                            // or if new creature is in no better position
                            if (oldCreature != null &&
                                oldCreature != this.CurrentTarget.Creature &&
                                oldCreature.IsVisible &&
                                client.Player.Location.IsOnScreen(oldCreature.Location))
                            {
                                if (this.Parent.CurrentSettings.StickToCreature) this.CurrentTarget.Creature = oldCreature;
                                else
                                {
                                    int oldDist = (int)client.Player.Location.DistanceTo(oldCreature.Location),
                                        newDist = this.CurrentTarget.Creature != null ?
                                            (int)client.Player.Location.DistanceTo(this.CurrentTarget.Creature.Location) :
                                            100;
                                    if (oldDist < newDist + 2) this.CurrentTarget.Creature = oldCreature;
                                }
                            }
                            oldCreature = this.CurrentTarget.Creature;

                            Objects.Location playerLoc = client.Player.Location,
                                targetLoc = this.CurrentTarget.Creature.Location;

                            #region targeting checks and whatnot
                            // check if the creature is dead
                            if (this.CurrentTarget.Creature.IsDead) break;

                            // check if creature is about to become a corpse
                            if (this.CurrentTarget.Creature.IsVisible &&
                                this.CurrentTarget.Creature.HealthPercent == 0)
                            {
                                continue;
                            }

                            // check if creature is still on screen, stop attacking and looping if so
                            if (!playerLoc.IsOnScreen(targetLoc))
                            {
                                this.CancelAttack();
                                break;
                            }

                            // check if creature if reachable and/or shootable
                            // stop attacking and break if settings says they must be
                            if ((setting.MustBeReachable && !this.CurrentTarget.Creature.IsReachable(this.CachedTiles, this.Parent.PathFinder)) ||
                                (setting.MustBeShootable && !this.CurrentTarget.Creature.IsShootable(this.CachedTiles)))
                            {
                                this.CancelAttack();
                                break;
                            }

                            // check if player is attacking the wrong target
                            if (client.Player.Target != this.CurrentTarget.Creature.ID)
                            {
                                this.CurrentTarget.Creature.Attack();
                                Thread.Sleep(100);
                            }

                            // set fight stance+mode and move accordingly
                            Map.Tile playerTile = this.CachedTiles.GetTile(count: client.Player.ID),
                                creatureTile = this.CachedTiles.GetTile(count: this.CurrentTarget.Creature.ID);
                            if (playerTile == null || creatureTile == null)
                            {
                                this.CancelAttack();
                                break;
                            }
                            switch (setting.FightStance)
                            {
                                case Enums.FightStance.Stand:
                                    if (client.Player.FightStance != Enums.FightStance.Stand) client.Player.FightStance = Enums.FightStance.Stand;
                                    break;
                                case Enums.FightStance.Follow:
                                    if (client.Player.FightStance != Enums.FightStance.Follow) client.Player.FightStance = Enums.FightStance.Follow;
                                    break;
                                case Enums.FightStance.FollowDiagonalOnly:
                                case Enums.FightStance.DistanceFollow:
                                case Enums.FightStance.DistanceWait:
                                case Enums.FightStance.FollowStrike:
                                    if (client.Player.FightStance != Enums.FightStance.Stand) client.Player.FightStance = Enums.FightStance.Stand;
                                    if (client.Player.IsWalking) break;
                                    Objects.Location bestLoc = this.CurrentTarget.GetBestLocation(setting, this.CachedTiles, this.CachedCreatures);
                                    if (bestLoc.IsValid() && client.Player.GoTo != bestLoc) client.Player.GoTo = bestLoc;
                                    break;
                            }

                            // shoot rune or spell
                            if (client.Player.HealthPercent >= this.Parent.CurrentSettings.MinimumHealthToShoot &&
                                this.CurrentTarget.Creature.HealthPercent > 0)
                            {
                                ushort runeID = 0;
                                if ((runeID = setting.GetRuneID()) != 0)
                                {
                                    Objects.Item rune = client.Inventory.GetItem(runeID);
                                    if (rune != null)
                                    {
                                        if (!this.StopwatchExhaust.IsRunning ||
                                            this.StopwatchExhaust.ElapsedMilliseconds >= this.Parent.CurrentSettings.Exhaust)
                                        {
                                            if (setting.RuneIsAoE()) // todo: test this
                                            {
                                                Map.Tile aoeTile = this.Parent.GetAreaEffectTile(setting.GetAreaEffect(),
                                                    this.CachedTiles, this.CurrentTarget.Creature);
                                                if (aoeTile != null)
                                                {
                                                    rune.UseOnTile(aoeTile);
                                                    this.StopwatchExhaust.Restart();
                                                }
                                            }
                                            else if (this.CurrentTarget.Creature.IsShootable(this.CachedTiles))
                                            {
                                                rune.UseOnBattleList(this.CurrentTarget.Creature);
                                                this.StopwatchExhaust.Restart();
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(setting.Spell))
                                {
                                    // todo: add aoe spells
                                    if (playerLoc.IsAdjacentNonDiagonalOnly(targetLoc))
                                    {
                                        Enums.Direction direction = Enums.Direction.Down;
                                        int diffX = playerLoc.X - targetLoc.X,
                                            diffY = playerLoc.Y - targetLoc.Y;
                                        if (diffX > 0) direction = Enums.Direction.Left;
                                        else if (diffX < 0) direction = Enums.Direction.Right;
                                        else if (diffY > 0) direction = Enums.Direction.Up;
                                        else if (diffY < 0) direction = Enums.Direction.Down;
                                        if (client.Player.Direction != direction)
                                        {
                                            client.Packets.Turn(direction);
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Thread.Sleep(50);
                                                if ((Enums.Direction)client.Player.Direction == direction) break;
                                            }
                                        }
                                        if (client.Player.Direction == direction &&
                                            (!this.StopwatchExhaust.IsRunning || this.StopwatchExhaust.ElapsedMilliseconds > this.Parent.CurrentSettings.Exhaust))
                                        {
                                            client.Packets.Say(setting.Spell);
                                            this.StopwatchExhaust.Restart();
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        if (this.TargetExecutedEnd != null) this.TargetExecutedEnd(this.CurrentTarget);
                        this.ResetEvent.Reset();
                        this.Cancel = false;
                    }
                    catch (Exception ex)
                    {
                        if (this.ErrorOccurred != null) this.ErrorOccurred(ex);
                    }
                }
            }
            private void CancelAttack()
            {
                if (this.Parent.Client.Player.Target != 0) this.Parent.Client.Packets.Attack(0);
                this.CurrentTarget.Creature = null;
            }
            #endregion
        }
    }
}
