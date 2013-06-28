using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KarelazisBot.Objects;

namespace KarelazisBot.Modules
{
	public partial class Cavebot
	{
        /// <summary>
        /// A class that represents an in-game target.
        /// </summary>
        public class Target
        {
            #region constructors
            /// <summary>
            /// Constructor for an empty target.
            /// </summary>
            /// <param name="parent">The Cavebot module that will hosts this object.</param>
            public Target(Cavebot parent)
                : this(parent, "New monster", new List<Setting>()) { }
            /// <summary>
            /// Constructor for a target.
            /// </summary>
            /// <param name="parent">The Cavebot module that will hosts this object.</param>
            /// <param name="name">The name of the creature to look for.</param>
            /// <param name="settings">A collection of Target.Setting objects, used as criterias when finding a new target.</param>
            public Target(Cavebot parent, string name, List<Target.Setting> settings)
            {
                this.Parent = parent;
                this.Name = name;
                this.Settings = settings;
                this.DoLoot = true;
                while (this.Settings.Count < this.SettingsMaxCount) this.Settings.Add(Target.Setting.GetDefaults(this));
            }
            #endregion

            #region properties
            /// <summary>
            /// The Cavebot module associated with this Target object. Objects.Client objects can be accessed through this member.
            /// </summary>
            public Cavebot Parent { get; private set; }
            /// <summary>
            /// The name to look for in the client.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// A collection of settings, used as criterias when finding a new target.
            /// </summary>
            private List<Target.Setting> Settings { get; set; }
            /// <summary>
            /// The index last regarded as the best setting.
            /// <para></para>
            /// This is set when calling GetBestSetting.
            /// </summary>
            public int CurrentSettingIndex { get; private set; }
            /// <summary>
            /// The amount of settings this target can hold and use.
            /// </summary>
            public readonly int SettingsMaxCount = 4;
            /// <summary>
            /// Storage for a Creature object, has no function in the class.
            /// </summary>
            public Objects.Creature Creature { get; set; }
            /// <summary>
            /// Gets or sets whether to loot this target.
            /// </summary>
            public bool DoLoot { get; set; }
            #endregion

            #region methods
            /// <summary>
            /// Gets a Target.Setting object by index. Returns null if no setting is found.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Target.Setting GetSettingByIndex(int index)
            {
                Target.Setting[] settings = this.GetSettings().ToArray();
                if (index >= settings.Length || index < 0) return null;
                return settings[index];
            }
            /// <summary>
            /// Gets an array of Target.Setting objects that this target currently holds.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Target.Setting> GetSettings()
            {
                return this.Settings.ToArray();
            }
            /// <summary>
            /// Gets the last setting that was returned by GetBestSetting. Returns null if not set.
            /// </summary>
            /// <returns></returns>
            public Target.Setting GetCurrentSetting()
            {
                return this.GetSettingByIndex(this.CurrentSettingIndex);
            }
            /// <summary>
            /// Attempts to get the best setting for this target. Also sets CurrentSettingIndex. Returns null if unsuccessful.
            /// </summary>
            /// <param name="creaturesOnScreen">A collection of creatures visible on screen.</param>
            /// <param name="playersOnScreen">A collection of players visible on screen.</param>
            /// <param name="tileCollection">A collection of tiles visible on screen.</param>
            /// <param name="setCreature">Whether to set this target's Creature.</param>
            /// <returns></returns>
            public Target.Setting GetBestSetting(IEnumerable<Objects.Creature> creaturesOnScreen,
                IEnumerable<Objects.Creature> playersOnScreen, Map.TileCollection tileCollection,
                bool setCreature)
            {
                // check if there are any settings to use
                bool found = false;
                foreach (Target.Setting s in this.GetSettings())
                {
                    if (s.UseThisSetting)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) return null;

                // set up the player's tile and other variables
                Map.Tile playerTile = tileCollection.GetTile(count: this.Parent.Client.Player.ID);
                if (playerTile == null) return null;
                List<Objects.Creature> creatures = new List<Objects.Creature>(),
                    players = new List<Objects.Creature>();
                foreach (Objects.Creature c in creaturesOnScreen)
                {
                    if (c.Name.ToLower() == this.Name.ToLower()) creatures.Add(c);
                }
                foreach (Objects.Creature p in playersOnScreen.ToArray())
                {
                    if (p.ID != this.Parent.Client.Player.ID) players.Add(p);
                }

                // calculate best setting
                int bestCount = 0, bestIndex = 0, index = 0;
                Target.Setting bestSetting = null;
                Objects.Creature bestCreature = null;
                foreach (Target.Setting setting in this.GetSettings())
                {
                    if (!setting.UseThisSetting) continue;
                    int count = 0,
                        bestCreatureDistance = setting.Range + 1;
                    Objects.Creature tempCreature = null;
                    foreach (Objects.Creature c in creatures)
                    {
                        if (!c.IsVisible) continue;
                        Map.Tile creatureTile = tileCollection.GetTile(count: c.ID);
                        if (creatureTile == null) continue;
                        if (!playerTile.WorldLocation.IsOnScreen(creatureTile.WorldLocation)) continue;
                        if (this.Parent.CurrentSettings.FriendlyMode && players.Count > 0 && !c.HasAttackedMeRecently(4000)) continue;
                        if (setting.MustBeShootable && !c.IsShootable(tileCollection)) continue;
                        var pfNodes = c.GetTilesToCreature(tileCollection, this.Parent.PathFinder)
                            .ToList<Objects.PathFinder.Node>();
                        if (setting.MustBeReachable && pfNodes.Count == 0) continue;
                        if ((pfNodes.Count > 0 ? pfNodes.Count : playerTile.WorldLocation.DistanceTo(creatureTile.WorldLocation)) > setting.Range) continue;
                        count++;

                        if (setCreature)
                        {
                            int distance = pfNodes.Count > 0 ?
                                pfNodes.Count :
                                (int)playerTile.WorldLocation.DistanceTo(creatureTile.WorldLocation);
                            if (distance < bestCreatureDistance)
                            {
                                bestCreatureDistance = distance;
                                tempCreature = c;
                            }
                        }
                    }

                    if (count == 0 || count < setting.Count) continue;
                    if (count > bestCount)
                    {
                        bestCount = count;
                        bestSetting = setting;
                        bestIndex = index;
                        bestCreature = tempCreature;
                    }

                    index++;
                }
                this.CurrentSettingIndex = bestSetting != null ? bestIndex : -1;
                if (bestSetting != null && bestCreature != null) this.Creature = bestCreature;
                return bestSetting;
            }
            /// <summary>
            /// Sets this target's settings.
            /// </summary>
            /// <param name="settings">A collection of Target.Setting objects that this target will use.</param>
            public void SetSettings(IEnumerable<Target.Setting> settings)
            {
                if (settings != null) this.Settings = settings.ToList<Target.Setting>();
                while (this.Settings.Count < this.SettingsMaxCount) this.Settings.Add(Target.Setting.GetDefaults(this));
            }
            /// <summary>
            /// Attempts to get the best location to move to. Returns Objects.Location.Invalid if unsuccessful.
            /// </summary>
            /// <param name="setting">The setting to use.</param>
            /// <param name="tileCollection">The tiles that are visible on screen.</param>
            /// <param name="creaturesOnScreen">The creatures that are visible on screen.</param>
            /// <returns></returns>
            public Objects.Location GetBestLocation(Target.Setting setting, Map.TileCollection tileCollection,
                IEnumerable<Objects.Creature> creaturesOnScreen, Stack<Objects.Location> backtrackedLocations = null)
            {
                if (this.Creature == null) return null;
                Map.Tile playerTile = tileCollection.GetTile(count: this.Parent.Client.Player.ID),
                    targetTile = tileCollection.GetTile(count: this.Creature.ID);
                if (playerTile == null || targetTile == null) return null;
                Map.TileCollection adjacentTiles = tileCollection.GetAdjacentTileCollection(targetTile);
                List<Objects.PathFinder.Node> pfNodes = null;
                int closest = 15;
                Objects.Location bestLocation = Objects.Location.Invalid;
                switch (setting.FightStance)
                {
                    case Enums.FightStance.FollowDiagonalOnly:
                        if (playerTile.WorldLocation.IsAdjacentDiagonalOnly(targetTile.WorldLocation)) break;
                        int closestNonDiagonal = 15;
                        Objects.Location bestNonDiagonalLocation = Objects.Location.Invalid;
                        foreach (Map.Tile tile in adjacentTiles.GetTiles())
                        {
                            if (!tile.IsWalkable()) continue;
                            //if (!tile.WorldLocation.IsAdjacentDiagonalOnly(targetTile.WorldLocation)) continue;
                            pfNodes = playerTile.WorldLocation.GetTilesToLocation(this.Parent.Client,
                                tile.WorldLocation, tileCollection, this.Parent.PathFinder, true).ToList<Objects.PathFinder.Node>();
                            if (pfNodes.Count > 0)
                            {
                                if (this.Parent.CurrentSettings.AllowDiagonalMovement &&
                                    playerTile.WorldLocation.IsAdjacentDiagonalOnly(targetTile.WorldLocation))
                                {
                                    continue;
                                }

                                if (pfNodes.Count - 1 < closest &&
                                    tile.WorldLocation.IsAdjacentDiagonalOnly(targetTile.WorldLocation))
                                {
                                    closest = pfNodes.Count - 1;
                                    bestLocation = tile.WorldLocation;
                                }
                                else if (pfNodes.Count - 1 < closestNonDiagonal &&
                                    tile.WorldLocation.IsAdjacentNonDiagonalOnly(targetTile.WorldLocation))
                                {
                                    closestNonDiagonal = pfNodes.Count - 1;
                                    bestNonDiagonalLocation = tile.WorldLocation;
                                }
                            }
                        }
                        if (!bestLocation.IsValid()) bestLocation = bestNonDiagonalLocation;
                        break;
                    case Enums.FightStance.FollowStrike:
                        if (playerTile.WorldLocation.IsAdjacentNonDiagonalOnly(targetTile.WorldLocation)) break;
                        foreach (Map.Tile tile in adjacentTiles.GetTiles())
                        {
                            if (!tile.WorldLocation.IsAdjacentNonDiagonalOnly(targetTile.WorldLocation)) continue;
                            pfNodes = playerTile.WorldLocation.GetTilesToLocation(this.Parent.Client,
                                tile.WorldLocation, tileCollection, this.Parent.PathFinder, true).ToList<Objects.PathFinder.Node>();
                            if (pfNodes.Count > 0 && pfNodes.Count - 1 < closest &&
                                (!this.Parent.CurrentSettings.AllowDiagonalMovement ||
                                 playerTile.WorldLocation.IsAdjacentDiagonalOnly(targetTile.WorldLocation)))
                            {
                                closest = pfNodes.Count - 1;
                                bestLocation = tile.WorldLocation;
                            }
                        }
                        break;
                    case Enums.FightStance.DistanceFollow:
                    case Enums.FightStance.DistanceWait:
                        // map creature tiles and the path nodes to them from the player
                        Dictionary<Map.Tile, List<Objects.PathFinder.Node>> creatureTiles =
                            new Dictionary<Map.Tile, List<Objects.PathFinder.Node>>();
                        // add the current target
                        creatureTiles.Add(targetTile,
                            this.Creature.GetTilesToCreature(tileCollection, this.Parent.PathFinder).ToList<Objects.PathFinder.Node>());
                        // check whether to add other monsters as well
                        if (this.Parent.CurrentSettings.ConsiderAllMonstersWhenKeepingAway)
                        {
                            foreach (Objects.Creature c in creaturesOnScreen)
                            {
                                Map.Tile t = tileCollection.GetTile(count: c.ID);
                                if (t != null && !creatureTiles.ContainsKey(t))
                                {
                                    creatureTiles.Add(t,
                                        c.GetTilesToCreature(tileCollection, this.Parent.PathFinder).ToList<Objects.PathFinder.Node>());
                                }
                            }
                        }

                        // check if the player needs to move
                        // also set the player's location as default location to return
                        bool needToMove = false;
                        bestLocation = playerTile.WorldLocation;
                        foreach (var keypair in creatureTiles)
                        {
                            // check if creature can reach the player
                            if (keypair.Value.Count == 0) continue;

                            if (keypair.Value.Count < setting.DistanceRange ||
                                (setting.FightStance == Enums.FightStance.DistanceFollow && keypair.Value.Count > setting.DistanceRange))
                            {
                                needToMove = true;
                            }
                        }
                        if (!needToMove) break;

                        int bestRange = 1;
                        Map.Tile bestTile = playerTile;
                        // calculate distance to the player's tile
                        foreach (var keypair in creatureTiles)
                        {
                            pfNodes = keypair.Value;
                            if (pfNodes.Count == 0) continue;
                            int count = pfNodes.Count;
                            bestRange += Math.Abs(setting.DistanceRange - count) * (count < setting.DistanceRange ? 2 : 1);
                        }

                        if (backtrackedLocations != null && backtrackedLocations.Count > 0)
                        {
                            Objects.Location peekLoc = backtrackedLocations.Peek();
                            if (peekLoc.IsOnScreen(playerTile.WorldLocation))
                            {
                                bestLocation = peekLoc;
                                break;
                            }
                        }
                        // calculate and set 
                        foreach (Map.Tile t in tileCollection.GetAdjacentTileCollection(playerTile).GetTiles())
                        {
                            if (t == null || !t.IsWalkable()) continue;
                            if (this.Parent.CurrentSettings.AllowDiagonalMovement &&
                                playerTile.WorldLocation.IsAdjacentDiagonalOnly(t.WorldLocation))
                            {
                                continue;
                            }

                            // calculate distance for creatures
                            int distance = 0;
                            foreach (var keypair in creatureTiles)
                            {
                                pfNodes = keypair.Key.WorldLocation.GetTilesToLocation(this.Parent.Client, t.WorldLocation,
                                    tileCollection, this.Parent.PathFinder, true).ToList<Objects.PathFinder.Node>();
                                if (pfNodes.Count == 0) continue;
                                int count = pfNodes.Count - 1;
                                distance += Math.Abs(setting.DistanceRange - count) * (count < setting.DistanceRange ? 2 : 1);
                            }

                            // get next tile in same general direction
                            bool good = false;
                            int directionX = playerTile.WorldLocation.X - t.WorldLocation.X,
                                directionY = playerTile.WorldLocation.Y - t.WorldLocation.Y;
                            foreach (Map.Tile nextTile in tileCollection.GetAdjacentTileCollection(t).GetTiles())
                            {
                                if (nextTile == null || !nextTile.IsWalkable()) continue;
                                if (t.WorldLocation.X - nextTile.WorldLocation.X != directionX &&
                                    t.WorldLocation.Y - nextTile.WorldLocation.Y != directionY)
                                {
                                    continue;
                                }
                                if (nextTile == playerTile) continue;

                                good = true;
                                break;
                            }
                            if (!good) continue;

                            // check of tile is better than previous tile
                            if (bestRange > distance)
                            {
                                bestRange = distance;
                                bestLocation = t.WorldLocation;
                            }
                        }
                        break;
                    case Enums.FightStance.FollowEconomic:
                        break;
                }
                return bestLocation;
            }
            #endregion

            public override string ToString()
            {
                return this.Name;
            }

            /// <summary>
            /// A class that encapsulates several settings for a target.
            /// </summary>
            public class Setting
            {
                #region constructors
                /// <summary>
                /// Constructor for this class.
                /// </summary>
                /// <param name="target">The target that will use these settings.</param>
                /// <param name="count">Minimum count for creatures.</param>
                /// <param name="rune">Name of the rune to use.</param>
                /// <param name="spell">Name of the spell to use.</param>
                /// <param name="fightMode">The fight mode to use.</param>
                /// <param name="fightStance">The fight stance to use.</param>
                /// <param name="range">Maxmimum range for this target.</param>
                /// <param name="distanceRange">Minimum range to keep distance at.</param>
                /// <param name="dangerLevel">Represents how dangerous this target is.</param>
                /// <param name="mustBeReachable">Will only choose this setting if the target is reachable.</param>
                /// <param name="mustBeShootable">Will only choose this setting if the target is shootable.</param>
                /// <param name="useThisSetting">Whether to use this setting or not.</param>
                public Setting(Target target, byte count, string rune = "", string spell = "",
                    Enums.FightMode fightMode = Enums.FightMode.Offensive, Enums.FightStance fightStance = Enums.FightStance.Follow,
                    byte range = 5, byte distanceRange = 3, byte dangerLevel = 0,
                    bool mustBeReachable = true, bool mustBeShootable = false, bool useThisSetting = true)
                {
                    Runes r = Runes.None;
                    switch (rune.ToLower())
                    {
                        case "none":
                        default:
                            r = Runes.None;
                            break;
                        case "sd":
                        case "sudden death":
                            r = Runes.SuddenDeath;
                            break;
                        case "explosion":
                        case "explo":
                            r = Runes.Explosion;
                            break;
                        case "hmm":
                        case "heavy magic missile":
                            r = Runes.HeavyMagicMissile;
                            break;
                    }

                    this.Target = target;
                    this.Count = count;
                    this.Rune = r;
                    this.Spell = spell;
                    this.FightMode = fightMode;
                    this.FightStance = fightStance;
                    this.Range = range;
                    this.DistanceRange = distanceRange;
                    this.DangerLevel = dangerLevel;
                    this.MustBeReachable = mustBeReachable;
                    this.MustBeShootable = mustBeShootable;
                    this.UseThisSetting = useThisSetting;
                }
                /// <summary>
                /// Constructor for this class.
                /// </summary>
                /// <param name="target">The target that will use these settings.</param>
                /// <param name="count">Minimum count for creatures.</param>
                /// <param name="rune">The rune to use.</param>
                /// <param name="spell">Name of the spell to use.</param>
                /// <param name="fightMode">The fight mode to use.</param>
                /// <param name="fightStance">The fight stance to use.</param>
                /// <param name="range">Maxmimum range for this target.</param>
                /// <param name="distanceRange">Minimum range to keep distance at.</param>
                /// <param name="dangerLevel">Represents how dangerous this target is.</param>
                /// <param name="mustBeReachable">Will only choose this setting if the target is reachable.</param>
                /// <param name="mustBeShootable">Will only choose this setting if the target is shootable.</param>
                /// <param name="useThisSetting">Whether to use this setting or not.</param>
                public Setting(Target target, byte count = 0, Runes rune = Runes.None, string spell = "",
                    Enums.FightMode fightMode = Enums.FightMode.Offensive, Enums.FightStance fightStance = Enums.FightStance.Follow,
                    byte range = 5, byte distanceRange = 3, byte dangerLevel = 0,
                    bool mustBeReachable = true, bool mustBeShootable = false, bool useThisSetting = true)
                {
                    this.Target = target;
                    this.Count = count;
                    this.Rune = rune;
                    this.Spell = spell;
                    this.FightMode = fightMode;
                    this.FightStance = fightStance;
                    this.Range = range;
                    this.DistanceRange = distanceRange;
                    this.DangerLevel = dangerLevel;
                    this.MustBeReachable = mustBeReachable;
                    this.MustBeShootable = mustBeShootable;
                    this.UseThisSetting = useThisSetting;
                }
                #endregion

                #region properties
                /// <summary>
                /// The parent target that will hold these settings.
                /// </summary>
                public Target Target { get; private set; }
                /// <summary>
                /// 0 = any,
                /// 1-* = minimum count
                /// </summary>
                public byte Count { get; set; }
                public Runes Rune { get; set; }
                public string Spell { get; set; }
                public Enums.FightMode FightMode { get; set; }
                public Enums.FightStance FightStance { get; set; }
                public byte Range { get; set; }
                public byte DistanceRange { get; set; }
                public byte DangerLevel { get; set; }
                public bool MustBeReachable { get; set; }
                public bool MustBeShootable { get; set; }
                public bool UseThisSetting { get; set; }
                #endregion

                #region enums
                public enum Runes
                {
                    None,
                    SuddenDeath,
                    HeavyMagicMissile,
                    GreatFireball,
                    Explosion
                }
                #endregion

                #region methods
                /// <summary>
                /// Gets the ID for this setting's rune. Returns 0 if invalid.
                /// </summary>
                /// <returns></returns>
                public ushort GetRuneID()
                {
                    switch (this.Rune)
                    {
                        case Runes.None:
                        default:
                            return 0;
                        case Runes.Explosion:
                            return this.Target.Parent.Client.ItemList.Runes.Explosion;
                        case Runes.GreatFireball:
                            return this.Target.Parent.Client.ItemList.Runes.GreatFireball;
                        case Runes.HeavyMagicMissile:
                            return this.Target.Parent.Client.ItemList.Runes.HeavyMagicMissile;
                        case Runes.SuddenDeath:
                            return this.Target.Parent.Client.ItemList.Runes.SuddenDeath;
                    }
                }
                /// <summary>
                /// Checks whether this setting's rune is AoE (i.e. great fireball).
                /// </summary>
                /// <returns></returns>
                public bool RuneIsAoE()
                {
                    switch (this.Rune)
                    {
                        case Runes.Explosion:
                        case Runes.GreatFireball:
                            return true;
                        default:
                            return false;
                    }
                }
                /// <summary>
                /// Gets an Objects.AreaEffect for this setting's rune. Returns null if unsuccessful.
                /// </summary>
                /// <returns></returns>
                public Objects.AreaEffect GetAreaEffect()
                {
                    switch (this.Rune)
                    {
                        case Runes.None:
                        default:
                            return null;
                        case Runes.Explosion:
                            return this.Target.Parent.Client.AreaEffects.Explosion;
                        case Runes.GreatFireball:
                            return this.Target.Parent.Client.AreaEffects.GreatFireball;
                    }
                }

                /// <summary>
                /// Gets a default Setting object.
                /// </summary>
                /// <param name="t">The target that will use this setting.</param>
                /// <returns></returns>
                public static Setting GetDefaults(Target t)
                {
                    Setting s = new Setting(target: t, rune: Runes.None, useThisSetting: false);
                    return s;
                }
                #endregion
            }
        }
	}
}
