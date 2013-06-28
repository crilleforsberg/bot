using System;
using System.Collections.Generic;
using System.Linq;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that primarily serves to describe a world location,
    /// but is also useful to describe any integer-based 3D locations.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Constructor for a string-based location, using ',' as delimeter.
        /// </summary>
        /// <param name="loc">The location to parse.</param>
        public Location(string loc)
        {
            string[] split = loc.Split(',');
            this.X = int.Parse(split[0]);
            this.Y = int.Parse(split[1]);
            this.Z = int.Parse(split[2]);
        }
        /// <summary>
        /// Constructor for a regular location.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        public Location(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Gets or sets the Y position;
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Gets or sets the Z position.
        /// </summary>
        public int Z { get; set; }

        public override string ToString()
        { 
            return this.X + "," + this.Y + "," + this.Z;
        }

        public override bool Equals(object other)
        {
            if ((object)this == null && other == null) return true;
            return this is Location && other is Location && this.Equals((Location)other);
        }
        public bool Equals(Location other)
        {
            if ((object)this == null && (object)other == null) return true;
            return other.X == this.X &&
                other.Y == this.Y &&
                other.Z == this.Z;
        }
        public static bool operator ==(Location me, Location other)
        {
            if (object.ReferenceEquals(me, other)) return true;
            if ((object)me == null ^ (object)other == null) return false;
            if ((object)me == null && (object)other == null) return true;

            return me.Equals(other);
        }
        public static bool operator !=(Location me, Location other)
        {
            return !(me == other);
        }
        public static Location operator -(Location me, Location other)
        {
            return new Location(me.X - other.X, me.Y - other.Y, me.Z - other.Z);
        }
        public static Location operator +(Location me, Location other)
        {
            return new Location(me.X + other.Y, me.Y + other.Y, me.Z + other.Z);
        }
        public override int GetHashCode()
        {
            if (this.Equals(Invalid))
                return (-1).GetHashCode();
            ushort shortX = (ushort)this.X;
            ushort shortY = (ushort)this.Y;
            byte byteZ = (byte)this.Z;
            return ((shortX << 3) + (shortY << 1) + byteZ).GetHashCode();
        }

        public static readonly Location Invalid = new Location(-1, -1, -1);

        /// <summary>
        /// Checks whether this location is a valid one.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return this != null && this != Invalid;
        }
        /// <summary>
        /// Returns the distance between two locations. Does not count tiles, only calculates a straight line.
        /// </summary>
        /// <param name="loc">The location to compare to.</param>
        /// <returns></returns>
        public double DistanceTo(Location loc)
        {
            int diffX = this.X - loc.X;
            int diffY = this.Y - loc.Y;
            return Math.Sqrt(diffX * diffX + diffY * diffY);
        }
        /// <summary>
        /// Checks whether another location is visible from this location.
        /// </summary>
        /// <param name="loc">The other location to check.</param>
        /// <returns></returns>
        public bool IsOnScreen(Location loc)
        {
            if (this.Z != loc.Z) return false;
            return Math.Abs(this.X - loc.X) <= 7 && Math.Abs(this.Y - loc.Y) <= 5;
        }
        /// <summary>
        /// Checks whether this location is adjacent (1 sqm away, regardless of direction) to another location.
        /// </summary>
        /// <param name="loc">The location to compare to.</param>
        /// <returns></returns>
        public bool IsAdjacentTo(Location loc)
        {
            return this.Z == loc.Z && Math.Max(Math.Abs(this.X - loc.X), Math.Abs(this.Y - loc.Y)) <= 1;
        }
        /// <summary>
        /// Checks whether this location is diagonally adjacent to another location.
        /// </summary>
        /// <param name="loc">The location to compare to.</param>
        /// <returns></returns>
        public bool IsAdjacentDiagonalOnly(Location loc)
        {
            return this.Z == loc.Z && Math.Abs(this.X - loc.X) == 1 && Math.Abs(this.Y - loc.Y) == 1;
        }
        /// <summary>
        /// Checks whether this location is non-diagonally adjacent to another location.
        /// </summary>
        /// <param name="loc">The location to compare to.</param>
        /// <returns></returns>
        public bool IsAdjacentNonDiagonalOnly(Location loc)
        {
            int x = Math.Abs(this.X - loc.X),
                y = Math.Abs(this.Y - loc.Y);
            return this.Z == loc.Z && (x == 1 ^ y == 1) && (x == 0 ^ y == 0);
        }
        /// <summary>
        /// Gets whether this location is in walkable range.
        /// </summary>
        /// <param name="loc">The location to compare to.</param>
        /// <returns></returns>
        public bool IsInRange(Location loc)
        {
            if (this.Z != loc.Z) return false;
            if (Math.Abs(this.X - loc.X) > 110 || Math.Abs(this.Y - loc.Y) > 110) return false;
            return true;
        }
        /// <summary>
        /// Offsets this location's X, Y and Z positions.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        public void SetOffset(int x = 0, int y = 0, int z = 0)
        {
            this.X += x;
            this.Y += y;
            this.Z += z;
        }
        /// <summary>
        /// Returns a new location based on offsetted positions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Location Offset(int x = 0, int y = 0, int z = 0)
        {
            return new Location(this.X + x, this.Y + y, this.Z + z);
        }
        /// <summary>
        /// Returns a new location based on offsetted positions.
        /// </summary>
        /// <param name="loc">The location to use as offset.</param>
        /// <returns></returns>
        public Location Offset(Location loc)
        {
            return Offset(loc.X, loc.Y, loc.Z);
        }
        /// <summary>
        /// Creates an array containing the X, Y and Z positions.
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            return new int[] { this.X, this.Y, this.Z };
        }
        public byte[] ToByteArray()
        {
            if (this.X > ushort.MaxValue || this.X < ushort.MinValue ||
                this.Y > ushort.MaxValue || this.Y < ushort.MinValue ||
                this.Z > byte.MaxValue || this.Z < ushort.MinValue)
            {
                throw new Exception("X, Y or Z are out of range.");
            }

            List<byte> data = new List<byte>(5); ; // 2 bytes X, 2 bytes Y, 1 byte Z
            data.AddRange(BitConverter.GetBytes((ushort)this.X));
            data.AddRange(BitConverter.GetBytes((ushort)this.Y));
            data.Add((byte)this.Z);
            return data.ToArray();
        }
        /// <summary>
        /// Creates an item location for this location.
        /// </summary>
        /// <returns></returns>
        public Objects.ItemLocation ToItemLocation()
        {
            return new Objects.ItemLocation(this);
        }
        /// <summary>
        /// Checks whether this location can reach another location.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to reach.</param>
        /// <returns></returns>
        public bool CanReachLocation(Objects.Client c, Location loc)
        {
            return this.CanReachLocation(c, loc, c.Map.GetTilesOnScreen());
        }
        /// <summary>
        /// Checks whether this location can reach another location.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to reach.</param>
        /// <param name="tiles">A list of tiles to use for pathfinding.</param>
        /// <returns></returns>
        public bool CanReachLocation(Objects.Client c, Location loc, Map.TileCollection tiles)
        {
            return this.CanReachLocation(c, loc, tiles, c.LocalPathFinder);
        }
        /// <summary>
        /// Checks whether this location can reach another location.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to reach.</param>
        /// <param name="tiles">A list of tiles to use for pathfinding.</param>
        /// <param name="pathFinder">The pathfinder to use.</param>
        /// <returns></returns>
        public bool CanReachLocation(Objects.Client c, Location loc,
            Map.TileCollection tiles, Objects.PathFinder pathFinder)
        {
            return this.GetTilesToLocation(c, loc, tiles, pathFinder, false).ToArray<Objects.PathFinder.Node>().Length > 0;
        }
        /// <summary>
        /// Gets a collection of pathfinder nodes to a given location. Returns 0 elements if unsuccessful.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to reach.</param>
        /// <param name="tiles">A list of tiles to use for pathfinding.</param>
        /// <param name="considerPlayerWalkable">Whether to consider the player as walkable.</param>
        /// <param name="considerCreatureOnLocationWalkable">Whether to consider any creatures on the target location as walkable.</param>
        /// <returns></returns>
        public IEnumerable<Objects.PathFinder.Node> GetTilesToLocation(Objects.Client c,
            Location loc, Map.TileCollection tiles, bool considerPlayerWalkable = false,
            bool considerCreatureOnLocationWalkable = false)
        {
            return this.GetTilesToLocation(c, loc, tiles, c.PathFinder, considerPlayerWalkable, considerCreatureOnLocationWalkable);
        }
        /// <summary>
        /// Gets a collection of pathfinder nodes to a given location. Returns 0 elements if unsuccessful.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to reach.</param>
        /// <param name="tiles">A list of tiles to use for pathfinding.</param>
        /// <param name="pathFinder">The pathfinder to use.</param>
        /// <param name="considerPlayerWalkable">Whether to consider the player as walkable.</param>
        /// <param name="considerCreatureOnLocationWalkable">Whether to consider any creatures on the target location as walkable.</param>
        /// <returns></returns>
        public IEnumerable<Objects.PathFinder.Node> GetTilesToLocation(Objects.Client c,
            Location loc, Map.TileCollection tiles, Objects.PathFinder pathFinder,
            bool considerPlayerWalkable = false, bool considerCreatureOnLocationWalkable = false)
        {
            if (pathFinder == null) return Enumerable.Empty<Objects.PathFinder.Node>();
            //return pathFinder.

            if (!this.IsOnScreen(c.Player.Location)) return Enumerable.Empty<Objects.PathFinder.Node>();

            uint playerId = c.Player.ID;
            Map.Tile playerTile = tiles.GetTile(count: playerId);
            Map.Tile fromTile = tiles.GetTile(this);
            Map.Tile targetTile = tiles.GetTile(loc);
            if (playerTile == null || fromTile == null || targetTile == null) return Enumerable.Empty<Objects.PathFinder.Node>();

            // check if target tile is walkable
            if (!targetTile.IsWalkable() && (!considerPlayerWalkable || targetTile != playerTile)) return Enumerable.Empty<Objects.PathFinder.Node>();
            if (fromTile == targetTile) return Enumerable.AsEnumerable(new Objects.PathFinder.Node[] { new Objects.PathFinder.Node() });
            lock (pathFinder)
            {
                pathFinder.ResetGrid();
                foreach (Map.Tile tile in tiles.GetTiles())
                {
                    if (tile == null) continue;

                    if ((considerPlayerWalkable && tile == playerTile) || tile.IsWalkable()) pathFinder.Grid[tile.MemoryLocation.X, tile.MemoryLocation.Y] = 1;
                    else pathFinder.Grid[tile.MemoryLocation.X, tile.MemoryLocation.Y] = (byte)Enums.MiniMapSpeedValues.Unwalkable;
                }
                pathFinder.Grid[fromTile.MemoryLocation.X, fromTile.MemoryLocation.Y] = 1;
                return pathFinder.FindPath(fromTile.MemoryLocation, targetTile.MemoryLocation);
            }
        }
        /// <summary>
        /// Checks whether this location can shoot another location.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to shoot at.</param>
        /// <returns></returns>
        public bool CanShootLocation(Objects.Client c, Objects.Location loc)
        {
            return this.CanShootLocation(c, loc, c.Map.GetTilesOnScreen());
        }
        /// <summary>
        /// Checks whether this location can shoot another location.
        /// </summary>
        /// <param name="c">The client to perform the operation on.</param>
        /// <param name="loc">The location to shoot at.</param>
        /// <param name="tileCollection">The tiles on screen to use for pathfinding.</param>
        /// <returns></returns>
        public bool CanShootLocation(Objects.Client c, Objects.Location loc, Map.TileCollection tileCollection)
        {
            if (!this.IsOnScreen(loc)) return false;

            Map.Tile playerTile = tileCollection.GetTile(count: c.Player.ID);
            if (playerTile == null) return false;
            Location playerLocation = playerTile.WorldLocation;
            if (playerLocation == loc) return true;
            if (!playerLocation.IsOnScreen(this) || !playerLocation.IsOnScreen(loc)) return false;

            int XSign = (this.X > loc.X) ? 1 : -1;
            int YSign = (this.Y > loc.Y) ? 1 : -1;
            double XDistance = Math.Abs(this.X - loc.X);
            double YDistance = Math.Abs(this.Y - loc.Y);
            double max = this.DistanceTo(loc);

            for (int i = 0; i <= max; i++)
            {
                Location check = this.Offset((int)Math.Ceiling(i * XDistance / max) * XSign,
                    (int)Math.Ceiling(i * YDistance / max) * YSign);
                Map.Tile tile = tileCollection.GetTile(check);
                if (tile == null) return false;
                if (tile.ContainsObjectProperty(Enums.ObjectPropertiesFlags.IsMissileBlocking)) return false;
            }
            return true;
        }
        /// <summary>
        /// Checks whether this location is inside a pre-defined area.
        /// </summary>
        /// <param name="areaEffect">The area effect used.</param>
        /// <param name="location">The center of the area.</param>
        /// <param name="direction">The direction of the area effect used.</param>
        /// <returns></returns>
        public bool IsInAreaEffect(Objects.AreaEffect areaEffect, Objects.Location location,
            Enums.Direction direction)
        {
            ushort y = 0;
            Objects.Location currentLoc = this.Clone();
            byte dir = areaEffect.Type == AreaEffect.EffectType.Spell ? (byte)direction : (byte)1;

            foreach (byte[] row in areaEffect.GetArea())
            {
                int center = row.Length / 2;
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] != dir) continue;
                    if (location.Offset(i - center, y, 0) == currentLoc) return true;
                }
                y++;
            }
            return false;
        }
        /// <summary>
        /// Creates a cloned location.
        /// </summary>
        /// <returns></returns>
        public Location Clone() { return new Location(this.X, this.Y, this.Z); }
    }
}
