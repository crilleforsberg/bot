using System;
using System.Collections.Generic;
using System.Threading;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that represents the in-game map.
    /// </summary>
    public partial class Map
    {
        #region constructors
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public Map(Objects.Client c)
        {
            this.Client = c;

            this.CachedTiles = new Map.Tile[Constants.Map.MaxTiles];
            int mapBegin = this.Client.Memory.ReadInt32(this.Client.Addresses.Map.Pointer);
            for (int i = 0; i < this.CachedTiles.Length; i++)
            {
                this.CachedTiles[i] = new Map.Tile(this.Client,
                    mapBegin + this.Client.Addresses.Map.TileStep * i,
                    i);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether level spy is currently in use.
        /// </summary>
        public bool LevelSpyStatus { get; private set; }

        private Map.Tile[] CachedTiles { get; set; }
        #endregion

        #region public methods
        public int MapStart
        {
            get { return this.Client.Memory.ReadInt32(this.Client.Addresses.Map.Pointer); }
        }
        public int TileNumberStart
        {
            get { return this.Client.Memory.ReadInt32(this.Client.Addresses.Map.TileNumberPointer); }
        }
        /// <summary>
        /// Enables name spy.
        /// </summary>
        public void NameSpyOn()
        {
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.NameSpy1, this.Client.Addresses.Map.NameSpyPatchedBytes);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.NameSpy2, this.Client.Addresses.Map.NameSpyPatchedBytes);
        }
        /// <summary>
        /// Disables name spy.
        /// </summary>
        public void NameSpyOff()
        {
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.NameSpy1, this.Client.Addresses.Map.NameSpyOriginalBytes1);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.NameSpy2, this.Client.Addresses.Map.NameSpyOriginalBytes2);
        }
        /// <summary>
        /// Disables level spy.
        /// </summary>
        public void LevelSpyOff()
        {
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy1, this.Client.Addresses.Map.LevelSpyOriginalBytes1);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy2, this.Client.Addresses.Map.LevelSpyOriginalBytes2);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy3, this.Client.Addresses.Map.LevelSpyOriginalBytes3);
            this.LevelSpyStatus = false;
        }
        /// <summary>
        /// Enables level spy.
        /// </summary>
        public void LevelSpyOn()
        {
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy1, this.Client.Addresses.Map.LevelSpyPatchedBytes);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy2, this.Client.Addresses.Map.LevelSpyPatchedBytes);
            this.Client.Memory.WriteBytes(this.Client.Addresses.Map.LevelSpy3, this.Client.Addresses.Map.LevelSpyPatchedBytes);
            this.LevelSpyStatus = true;
        }
        /// <summary>
        /// Levelspies one floor up or down.
        /// </summary>
        /// <returns></returns>
        public bool LevelSpy(Enums.Direction direction)
        {
            if (!this.LevelSpyStatus) return false;
            if (direction != Enums.Direction.Up && direction != Enums.Direction.Down) return false;

            int address = this.GetLevelSpyAddress();
            byte playerZ = this.Client.Player.Z;
            int floor = this.Client.Memory.ReadInt32(address);
            int z = floor + (direction == Enums.Direction.Down ? 1 : -1);
            if (playerZ <= 7 && z > 0 && z <= 7) // check if player is above ground
            {
                this.Client.Memory.WriteInt32(address, z);
                return true;
            }
            else if (playerZ > 7 && z > 0 && z <= 4) // check if player is underground
            {
                this.Client.Memory.WriteInt32(address, z);
                return true;
            }
            return false;
        }
        public Map.TileObject GetTopMoveItem(Location worldLocation, Map.Tile playerTile = null)
        {
            Map.Tile pTile = playerTile;
            if (pTile == null) pTile = this.GetPlayerTile();
            if (pTile == null) return null;

            Map.Tile tile = this.GetTile(worldLocation, pTile);
            if (tile == null) return null;

            return tile.GetTopMoveItem();
        }
        public Map.TileObject GetTopUseItem(Location worldLocation, bool useOn, Map.Tile playerTile = null)
        {
            Map.Tile pTile = playerTile;
            if (pTile == null) pTile = this.GetPlayerTile();
            if (pTile == null) return null;

            Map.Tile tile = this.GetTile(worldLocation, pTile);
            if (tile == null) return null;

            return tile.GetTopUseItem(useOn);
        }
        /// <summary>
        /// Gets a tile.
        /// </summary>
        /// <param name="tileNumber">The tile number to look for.</param>
        /// <param name="playerTile">The player's tile, used to get memory location and world location.</param>
        /// <param name="mapBegin">The memory address of the beginning of the map structure.</param>
        /// <returns></returns>
        public Map.Tile GetTile(int tileNumber, Map.Tile playerTile, int mapBegin = 0)
        {
            if (tileNumber < 0 || tileNumber >= this.CachedTiles.Length) return null;

            if (playerTile == null) playerTile = this.GetPlayerTile();
            if (playerTile == null) return null;

            if (mapBegin == 0) mapBegin = this.MapStart;

            Map.Tile t = this.CachedTiles[tileNumber];
            t.Address = mapBegin + this.Client.Addresses.Map.TileStep * tileNumber;
            t.UpdateObjects();
            t.RealMemoryLocation = this.TileNumberToMemoryLocation(t.TileNumber, playerTile);
            t.MemoryLocation = this.CentralizeMemoryLocation(t.RealMemoryLocation, playerTile);
            t.WorldLocation = this.MemoryLocationToWorldLocation(t.MemoryLocation, playerTile);
            return t;
        }
        /// <summary>
        /// Gets a tile at a given world location.
        /// </summary>
        /// <param name="loc">The world location to look fopr.</param>
        /// <param name="playerTile">The player's tile, used to get memory location and world location.</param>
        /// <returns></returns>
        public Map.Tile GetTile(Location loc, Map.Tile playerTile = null, int mapStart = 0)
        {
            if (playerTile == null) playerTile = this.GetPlayerTile();
            if (playerTile == null) return null;

            // todo: figure this shit out
            if (playerTile.WorldLocation.Z != loc.Z) return null;

            Location diff = loc - playerTile.WorldLocation;
            // set index to player index
            int index = this.GetPlayerFloor((byte)playerTile.WorldLocation.Z) * Constants.Map.MaxTilesPerFloor +
                Constants.Map.TileNumberCenterIndex;
            // subtract instead of add Z as map is from bottom to top
            //if (diff.Z != 0) index -= diff.Z * Constants.Map.MaxTilesPerFloor;
            if (diff.Y != 0) index += diff.Y * Constants.Map.MaxX;
            if (diff.X != 0) index += diff.X;

            if (index < 0 || index >= Constants.Map.MaxTiles) return null;
            ushort tileNumber = this.Client.Memory.ReadUInt16(this.TileNumberStart +
                this.Client.Addresses.Map.TileNumberStep * index);
            return this.GetTile(tileNumber, playerTile, mapStart);
        }
        /// <summary>
        /// Gets the player's tile.
        /// </summary>
        /// <returns></returns>
        public Map.Tile GetPlayerTile()
        {
            byte playerZ = this.Client.Player.Z;
            int floor = 0;
            if (playerZ <= 7) floor = 7 - playerZ;
            else floor = 2;
            int tileNumber = this.Client.Memory.ReadInt32(this.TileNumberStart +
                (Constants.Map.MaxTilesPerFloor * floor * 4) +
                Constants.Map.TileNumberCenterOffset);
            Map.Tile t = this.CachedTiles[tileNumber];
            t.UpdateObjects();
            uint id = this.Client.Player.ID;
            if (!t.ContainsCreature(id)) return null;
            t.WorldLocation = this.Client.Player.Location;
            t.RealMemoryLocation = this.TileNumberToMemoryLocation(t.TileNumber, t);
            t.MemoryLocation = this.CentralizeMemoryLocation(t.RealMemoryLocation, t);
            return t;
        }
        /// <summary>
        /// Gets a collection of tiles containing a given object ID.
        /// </summary>
        /// <param name="objectID">The object ID to look for.</param>
        /// <returns></returns>
        public Map.TileCollection GetTilesWithObject(ushort objectID)
        {
            return this.GetTilesWithObjects(new ushort[] { objectID });
        }
        /// <summary>
        /// Gets a collection of tiles containing at least one given object ID.
        /// </summary>
        /// <param name="ids">The collection of object IDs to look for.</param>
        /// <returns></returns>
        public Map.TileCollection GetTilesWithObjects(IEnumerable<ushort> ids)
        {
            List<Map.Tile> tiles = new List<Map.Tile>();
            foreach (Map.Tile tile in this.GetTilesOnScreen().GetTiles())
            {
                int oldCount = tiles.Count;
                foreach (Map.TileObject to in tile.GetObjects())
                {
                    foreach (ushort id in ids)
                    {
                        if (to.ID == id)
                        {
                            tiles.Add(tile);
                            break;
                        }
                    }
                    if (oldCount != tiles.Count) break;
                }
            }
            return new Map.TileCollection(this.Client, tiles);
        }
        /// <summary>
        /// Gets a list of adjacent tiles from a given tile.
        /// </summary>
        /// <param name="tile">The tile to get adjacent tiles from.</param>
        /// <returns></returns>
        public Map.TileCollection GetAdjacentTiles(Map.Tile tile)
        {
            return this.GetNearbyTiles(tile, 1);
        }
        public Map.TileCollection GetAdjacentTiles(Objects.Location loc)
        {
            return this.GetNearbyTiles(loc, 1);
        }
        /// <summary>
        /// Gets a collection of nearby tiles from a given tile.
        /// </summary>
        /// <param name="tile">The tile to get the nearby tiles from.</param>
        /// <param name="range">At what range to get nearby tiles from.</param>
        /// <returns></returns>
        public Map.TileCollection GetNearbyTiles(Map.Tile tile, byte range)
        {
            return this.GetNearbyTiles(tile.WorldLocation, range);
        }
        public Map.TileCollection GetNearbyTiles(Objects.Location loc, byte range)
        {
            List<Map.Tile> tiles = new List<Map.Tile>();
            Tile p = this.GetPlayerTile();
            if (p == null) return new Map.TileCollection(this.Client, tiles);
            for (int x = range * -1; x <= range; x++)
            {
                for (int y = range * -1; y <= range; y++)
                {
                    if (x == 0 && y == 0) continue; // skip centre tile
                    tiles.Add(this.GetTile(loc.Offset(x, y), p));
                }
            }
            return new Map.TileCollection(this.Client, tiles);
        }
        /// <summary>
        /// Gets a collection of tiles on the current floor.
        /// </summary>
        /// <returns></returns>
        public Map.TileCollection GetTilesOnScreen()
        {
            List<Map.Tile> listTiles = new List<Map.Tile>();
            Map.Tile playerTile = this.GetPlayerTile();
            if (playerTile == null) return new Map.TileCollection(this.Client, listTiles);

            byte floor = this.GetPlayerFloor((byte)playerTile.WorldLocation.Z);
            ushort[] tileNumbers = this.Client.Memory.ReadUInt16Array(this.TileNumberStart +
                floor * (Constants.Map.MaxTilesPerFloor * this.Client.Addresses.Map.TileNumberStep),
                Constants.Map.MaxTilesPerFloor);

            int mapStart = this.MapStart;
            for (int i = 0; i < tileNumbers.Length; i++)
            {
                Map.Tile t = this.GetTile(tileNumbers[i], playerTile, mapStart);
                if (t != null) listTiles.Add(t);
            }
            return new Map.TileCollection(this.Client, listTiles);
        }
        #endregion public methods

        #region private methods
        private byte GetPlayerFloor(byte playerZ = byte.MaxValue)
        {
            if (playerZ == byte.MaxValue) playerZ = this.Client.Player.Z;

            byte floor = 0;
            if (playerZ <= 7) floor = (byte)(7 - playerZ);
            else floor = 2;
            return floor;
        }
        private int GetLevelSpyAddress()
        {
            int pointer = this.Client.Memory.ReadInt32(this.Client.Addresses.Map.LevelSpyPointer);
            pointer += this.Client.Addresses.Map.LevelSpyOffset1;
            pointer = this.Client.Memory.ReadInt32(pointer);
            pointer += this.Client.Addresses.Map.LevelSpyOffset2;
            return pointer;
        }
        /// <summary>
        /// Gets a tile's memory location based on a tile number.
        /// </summary>
        /// <param name="memoryLocation"></param>
        /// <returns></returns>
        private int MemoryLocationToTileNumber(Location memoryLocation)
        {
            int y = Constants.Map.MaxY,
                x = Constants.Map.MaxX;
            return memoryLocation.X + memoryLocation.Y * x + memoryLocation.Z * y * x;
        }
        /// <summary>
        /// Gets a tile number based on a memory location.
        /// </summary>
        /// <param name="tileNumber"></param>
        /// <param name="playerTile">The player's tile to use as reference.</param>
        /// <returns></returns>
        private Objects.Location TileNumberToMemoryLocation(int tileNumber, Map.Tile playerTile)
        {
            int y = Constants.Map.MaxY,
                x = Constants.Map.MaxX;
            Objects.Location loc = new Objects.Location();
            loc.Z = (int)(tileNumber / (y * x));
            loc.Y = (int)((tileNumber - (loc.Z * y * x)) / x);
            loc.X = (int)((tileNumber - (loc.Z * y * x)) - loc.Y * x);
            return loc;
        }
        private Objects.Location CentralizeMemoryLocation(Objects.Location memLoc, Map.Tile playerTile)
        {
            // 8,6 == center
            int diffX = 8 - playerTile.RealMemoryLocation.X;
            int diffY = 6 - playerTile.RealMemoryLocation.Y;
            Objects.Location loc = memLoc.Offset(diffX, diffY);
            int maxX = Constants.Map.MaxX, maxY = Constants.Map.MaxY;
            if (loc.X >= maxX) loc.X -= maxX;
            else if (loc.X < 0) loc.X += maxX;
            if (loc.Y >= maxY) loc.Y -= maxY;
            else if (loc.Y < 0) loc.Y += maxY;
            return loc;
        }
        /// <summary>
        /// Gets a world location based on a memory location.
        /// </summary>
        /// <param name="worldLocation">The world location to use as reference.</param>
        /// <param name="playerTile">The player's tile to use as reference.</param>
        /// <returns></returns>
        private Objects.Location WorldLocationToMemoryLocation(Objects.Location worldLocation, Map.Tile playerTile)
        {
            // get and apply diffs
            int diffX = worldLocation.X - playerTile.WorldLocation.X;
            int diffY = worldLocation.Y - playerTile.WorldLocation.Y;
            int diffZ = worldLocation.Z - playerTile.WorldLocation.Z;
            Objects.Location memLoc = playerTile.RealMemoryLocation.Offset(diffX, diffY, diffZ);

            // re-align values if they're out of range
            if (memLoc.X < 0) memLoc.X += Constants.Map.MaxX;
            else if (memLoc.X >= Constants.Map.MaxX) memLoc.X -= Constants.Map.MaxX;
            if (memLoc.Y < 0) memLoc.Y += Constants.Map.MaxY;
            else if (memLoc.Y >= Constants.Map.MaxY) memLoc.Y -= Constants.Map.MaxY;

            return memLoc;
        }
        /// <summary>
        /// Gets a memory location based on a world location.
        /// </summary>
        /// <param name="memoryLocation">The memory location to use as reference.</param>
        /// <param name="playerTile">The player's tile to use as reference.</param>
        /// <returns></returns>
        private Objects.Location MemoryLocationToWorldLocation(Objects.Location memoryLocation, Map.Tile playerTile)
        {
            if (playerTile == null || !memoryLocation.IsValid()) return Objects.Location.Invalid;

            Objects.Location playerMemLoc = playerTile.MemoryLocation;
            return playerTile.WorldLocation.Offset(memoryLocation.X - playerMemLoc.X,
                memoryLocation.Y - playerMemLoc.Y,
                memoryLocation.Z - playerMemLoc.Z);

            /*Objects.Location loc = Objects.Location.Invalid;
            if (playerTile == null) return loc;
            
            // get the center of a "screen"
            // keep in mind that memory locations are zero-based
            int centerX = Constants.Map.MaxX / 2 - 1,
                centerY = Constants.Map.MaxY / 2 - 1;

            loc = new Objects.Location(memoryLocation.X, memoryLocation.Y, memoryLocation.Z);
            Objects.Location playerMemLoc = playerTile.MemoryLocation;

            // get and apply diffs
            int diffX = centerX - playerMemLoc.X;
            int diffY = centerY - playerMemLoc.Y;
            loc.X += diffX;
            loc.Y += diffY;

            // re-align values if they are out of range
            int maxX = Constants.Map.MaxX - 1;
            int maxY = Constants.Map.MaxY - 1;
            if (loc.X > maxX) loc.X -= Constants.Map.MaxX;
            else if (loc.X < 0) loc.X += Constants.Map.MaxX;
            if (loc.Y > maxY) loc.Y -= Constants.Map.MaxY;
            else if (loc.Y < 0) loc.Y += Constants.Map.MaxY;

            // finally align to the player's world location
            Objects.Location playerLoc = playerTile.WorldLocation;
            return playerLoc.Offset(loc.X - centerX, loc.Y - centerY, loc.Z - playerMemLoc.Z);*/
        }
        #endregion
    }
}
