using System;
using System.Collections.Generic;
using System.Linq;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that represents an in-game creature, player or NPC.
    /// </summary>
    public class Creature
    {
        public Creature(Objects.Client c, int battleListAddress)
        {
            this.Client = c;
            this.BattleListAddress = battleListAddress;
            this.UseCaching = true;
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }

        #region properties
        /// <summary>
        /// Gets the client associated with this creature.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets or sets this creature's memory address.
        /// </summary>
        public int BattleListAddress { get; set; }
        /// <summary>
        /// Gets or sets whether to cache properties that are unlikely to change very quickly.
        /// </summary>
        public bool UseCaching { get; set; }

        /// <summary>
        /// Gets or sets this creature's ID.
        /// </summary>
        public uint ID
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.ID); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.ID, value); }
        }
        /// <summary>
        /// Gets this creature's type.
        /// </summary>
        public Types Type
        {
            get { return (Types)this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Type); }
        }
        /// <summary>
        /// Gets or sets this creature's name.
        /// </summary>
        public string Name
        {
            get
            {
                if (!this.UseCaching) return this.Client.Memory.ReadString(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Name);

                int tick = Environment.TickCount;
                uint id = this.ID;
                if (tick < this.CachedNameTime + 1000 && id == this.OldID) return this.CachedName;

                this.OldID = id;
                this.CachedName = this.Client.Memory.ReadString(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Name);
                this.CachedNameTime = tick;
                return this.CachedName;
            }
            set
            {
                if (this.UseCaching)
                {
                    this.CachedName = value;
                    this.CachedNameTime = Environment.TickCount;
                }
                this.Client.Memory.WriteString(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Name, value);
            }
        }
        private string CachedName { get; set; }
        private int CachedNameTime { get; set; }
        private uint OldID { get; set; }
        /// <summary>
        /// Gets this creature's world location.
        /// </summary>
        public Objects.Location Location
        {
            get { return new Objects.Location(this.X, this.Y, this.Z); }
        }
        /// <summary>
        /// Gets this creature's X position.
        /// </summary>
        public ushort X
        {
            get { return this.Client.Memory.ReadUInt16(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.X); }
        }
        /// <summary>
        /// Gets this creature's Y position.
        /// </summary>
        public ushort Y
        {
            get { return this.Client.Memory.ReadUInt16(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Y); }
        }
        /// <summary>
        /// Gets this creature's Z position.
        /// </summary>
        public byte Z
        {
            get { return this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Z); }
        }
        /// <summary>
        /// Gets this creature's direction.
        /// </summary>
        public Enums.Direction Direction
        {
            get { return (Enums.Direction)this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Direction); }
        }
        /// <summary>
        /// Gets or sets whether this creature is walking.
        /// </summary>
        public bool IsWalking
        {
            get { return this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.IsWalking) != 0; }
            set { this.Client.Memory.WriteByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.IsWalking, value ? (byte)1 : (byte)0); }
        }
        /// <summary>
        /// Gets or sets whether this creature is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.IsVisible) != 0; }
            set { this.Client.Memory.WriteByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.IsVisible, Convert.ToByte(value)); }
        }
        /// <summary>
        /// Gets the time of this creature having attacked the player. Time is based on time since the client was started in milliseconds.
        /// </summary>
        public uint BlackSquare
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.BlackSquare); }
        }
        /// <summary>
        /// Gets whether this creature has attacked the player within the last 60 seconds.
        /// </summary>
        public bool HasAttackedMe
        {
            get { return this.HasAttackedMeRecently(1000 * 8); }
        }
        /// <summary>
        /// Returns true if this creature has attacked the player within a set timeframe.
        /// </summary>
        /// <param name="timeframe">The amount of milliseconds that will serve as a timeframe.</param>
        /// <returns></returns>
        public bool HasAttackedMeRecently(int timeframe)
        {
            if (this.Client.Memory.ReadUInt32(this.Client.Addresses.Misc.Time) - this.BlackSquare < timeframe) return true;
            return false;
        }
        /// <summary>
        /// Gets the health percent of this creature.
        /// </summary>
        public byte HealthPercent
        {
            get { return this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.HealthPercent); }
        }
        /// <summary>
        /// Gets the walking speed of this creaure.
        /// </summary>
        public uint WalkSpeed
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.WalkingSpeed); }
        }
        /// <summary>
        /// Gets the party icon of this creature.
        /// </summary>
        public Enums.PartyShield Party
        {
            get { return (Enums.PartyShield)this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Party); }
        }
        /// <summary>
        /// Gets the skull of this creature.
        /// </summary>
        public Enums.Skull Skull
        {
            get { return (Enums.Skull)this.Client.Memory.ReadByte(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Skull); }
        }

        public uint OutfitType
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitType); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitType, value); }
        }

        public uint OutfitHead
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitHead); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitHead, value); }
        }

        public uint OutfitBody
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitTorso); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitTorso, value); }
        }

        public uint OutfitLegs
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitLegs); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitLegs, value); }
        }

        public uint OutfitFeet
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitFeet); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitFeet, value); }
        }
        /// <summary>
        /// Gets or sets the amount of light this creature is omitting.
        /// </summary>
        public uint Light
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Light); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.Light, value); }
        }
        /// <summary>
        /// Gets or sets the color of the light this creature is omitting.
        /// </summary>
        public uint LightColor
        {
            get { return this.Client.Memory.ReadUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.LightColor); }
            set { this.Client.Memory.WriteUInt32(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.LightColor, value); }
        }
        /// <summary>
        /// Gets whether this creature is dead.
        /// </summary>
        public bool IsDead
        {
            get { return this.HealthPercent == 0 && !this.IsVisible; }
        }
        #endregion

        public enum Types
        {
            Player = 16, // realots 0
            Creature = 64
        }

        /// <summary>
        /// Attempts to attack this creature.
        /// </summary>
        public void Attack() { this.Client.Packets.Attack(this); }
        /// <summary>
        /// Attempts to follow this creature.
        /// </summary>
        public void Follow() { this.Client.Packets.Follow(this.ID); }
        /// <summary>
        /// Attempts to use an item on this creature.
        /// </summary>
        /// <param name="itemLocation">The item to use.</param>
        public void UseItemOn(Objects.ItemLocation itemLocation)
        {
            if (itemLocation != null) this.Client.Packets.UseItemOnLocation(itemLocation, this.Location);
        }
        /// <summary>
        /// Attempts to send a private message to this creature.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void PrivateMessage(string message) { this.Client.Packets.PrivateMessage(this.Name, message); }
        /// <summary>
        /// Checks whether this creature is reachable.
        /// </summary>
        /// <returns></returns>
        public bool IsReachable() { return this.IsReachable(this.Client.Map.GetTilesOnScreen()); }
        /// <summary>
        /// Checks whether this creature is reachable.
        /// </summary>
        /// <param name="tileCollection">The tiles on screen to use for pathfinding.</param>
        /// <returns></returns>
        public bool IsReachable(Map.TileCollection tileCollection) { return this.IsReachable(tileCollection, this.Client.PathFinder); }
        /// <summary>
        /// Checks whether this creature is reachable.
        /// </summary>
        /// <param name="tileCollection">The tiles on screen to use for pathfinding.</param>
        /// <param name="pathFinder">The pathfinder to use.</param>
        /// <returns></returns>
        public bool IsReachable(Map.TileCollection tileCollection, Objects.PathFinder pathFinder)
        {
            return this.GetTilesToCreature(tileCollection, pathFinder).ToList<Objects.PathFinder.Node>().Count > 0;
        }
        /// <summary>
        /// Gets a list of nodes to this creature. Includes start node and end node. Returns null if no path found.
        /// </summary>
        /// <param name="tileCollection">The list of tiles on screen to use for pathfinding.</param>
        /// <returns></returns>
        public IEnumerable<Objects.PathFinder.Node> GetTilesToCreature(Map.TileCollection tileCollection)
        {
            return this.GetTilesToCreature(tileCollection, this.Client.PathFinder);
        }
        /// <summary>
        /// Gets a collection of pathfinder nodes to this creature. Returns an empty IEnumerable if unreachable.
        /// </summary>
        /// <param name="tileCollection">The list of tiles on screen to use for pathfinding.</param>
        /// <param name="pathFinder">The PathFinder object to use.</param>
        /// <returns></returns>
        public IEnumerable<Objects.PathFinder.Node> GetTilesToCreature(Map.TileCollection tileCollection,
            Objects.PathFinder pathFinder)
        {
            if (pathFinder == null) return Enumerable.Empty<Objects.PathFinder.Node>();

            if (this.Location.IsAdjacentTo(this.Client.Player.Location))
            {
                return new List<Objects.PathFinder.Node>()
                {
                    new Objects.PathFinder.Node() { X = 8, Y = 6 }
                };
            }
            if (!this.Location.IsOnScreen(this.Client.Player.Location)) return Enumerable.Empty<Objects.PathFinder.Node>();
            uint playerZ = this.Client.Player.Z;
            if (tileCollection.IsEmpty()) return Enumerable.Empty<Objects.PathFinder.Node>();
            Map.Tile playerTile = tileCollection.GetTile(count: this.Client.Player.ID);
            Map.Tile creatureTile = tileCollection.GetTile(count: this.ID);
            if (playerTile == null || creatureTile == null) return Enumerable.Empty<Objects.PathFinder.Node>();
            lock (pathFinder)
            {
                pathFinder.ResetGrid();
                foreach (Map.Tile tile in tileCollection.GetTiles())
                {
                    if (tile == null) continue;

                    if (!tile.IsWalkable()) pathFinder.Grid[tile.MemoryLocation.X, tile.MemoryLocation.Y] = (byte)Enums.MiniMapSpeedValues.Unwalkable;
                    else pathFinder.Grid[tile.MemoryLocation.X, tile.MemoryLocation.Y] = 1;
                }
                pathFinder.Grid[playerTile.MemoryLocation.X, playerTile.MemoryLocation.Y] = 1;
                pathFinder.Grid[creatureTile.MemoryLocation.X, creatureTile.MemoryLocation.Y] = 1;
                return pathFinder.FindPath(playerTile.MemoryLocation, creatureTile.MemoryLocation, true);
            }
        }
        /// <summary>
        /// Checks whether this creature is shootable.
        /// </summary>
        /// <returns></returns>
        public bool IsShootable() { return this.IsShootable(this.Client.Map.GetTilesOnScreen()); }
        /// <summary>
        /// Checks whether this creature is shootable.
        /// </summary>
        /// <param name="tileCollection">The tile collection to use for pathfinding.</param>
        /// <returns></returns>
        public bool IsShootable(Map.TileCollection tileCollection)
        {
            return this.Client.Player.Location.CanShootLocation(this.Client, this.Location, tileCollection);
        }
    }
}
