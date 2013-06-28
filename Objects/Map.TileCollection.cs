using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Map
    {
    /// <summary>
    /// A class made to hold a list of tiles and to remove bloat from the Map class.
    /// </summary>
        public class TileCollection : IEnumerable<Tile>
        {
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="client">The client associated with this object.</param>
            /// <param name="tiles">The tiles that this collection will hold.</param>
            public TileCollection(Objects.Client client, IEnumerable<Map.Tile> tiles)
            {
                this.Client = client;
                this.Tiles = tiles.ToList();
            }

            private List<Map.Tile> Tiles { get; set; }
            /// <summary>
            /// Gets the client associated with this object.
            /// </summary>
            public Objects.Client Client { get; private set; }
            /// <summary>
            /// Gets the amount of tiles in this collection.
            /// </summary>
            public int Count
            {
                get { return this.Tiles.Count; }
            }

            /// <summary>
            /// Gets an array of tiles that exists in this collection.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Map.Tile> GetTiles() { return this.Tiles.ToArray(); }
            /// <summary>
            /// Gets whether this collection doesn't contain any tiles.
            /// </summary>
            /// <returns></returns>
            public bool IsEmpty() { return this.Tiles.Count == 0; }
            public Map.Tile GetPlayerTile()
            {
                return this.GetTile(count: this.Client.Player.ID);
            }
            /// <summary>
            /// Gets a collection of nearby tiles.
            /// </summary>
            /// <param name="tile">The parent tile.</param>
            /// <param name="range">How far away from the tile to get tiles.</param>
            /// <returns></returns>
            public TileCollection GetNearbyTileCollection(Map.Tile tile, byte range)
            {
                if (tile == null) return new TileCollection(this.Client, Enumerable.Empty<Map.Tile>());
                return new TileCollection(this.Client, this.GetNearbyTiles(tile, range));
            }
            /// <summary>
            /// Gets a collection of nearby tiles.
            /// </summary>
            /// <param name="loc">The parent tile's world location.</param>
            /// <param name="range">How far away from the tile to get tiles.</param>
            /// <returns></returns>
            public TileCollection GetNearbyTileCollection(Objects.Location loc, byte range)
            {
                Map.Tile tile = this.GetTile(loc);
                if (tile == null) return new TileCollection(this.Client, Enumerable.Empty<Map.Tile>());
                return this.GetNearbyTileCollection(tile, range);
            }
            /// <summary>
            /// Gets a collection of nearby tiles.
            /// </summary>
            /// <param name="tile">The parent tile.</param>
            /// <param name="range">How far away from the tile to get tiles.</param>
            /// <returns></returns>
            public IEnumerable<Map.Tile> GetNearbyTiles(Map.Tile tile, byte range)
            {
                if (tile == null) Enumerable.Empty<Map.Tile>();

                for (int x = range * -1; x <= range; x++)
                {
                    for (int y = range * -1; y <= range; y++)
                    {
                        if (x == 0 && y == 0) continue; // skip centre tile
                        Map.Tile t = this.GetTile(tile.WorldLocation.Offset(x, y, 0));
                        if (t != null) yield return t;
                    }
                }
            }
            /// <summary>
            /// Gets a collection of nearby tiles.
            /// </summary>
            /// <param name="loc">The parent tile's world location.</param>
            /// <param name="range">How far away from the tile to get tiles.</param>
            /// <returns></returns>
            public IEnumerable<Map.Tile> GetNearbyTiles(Objects.Location loc, byte range)
            {
                Map.Tile tile = this.GetTile(loc);
                if (tile == null) return Enumerable.Empty<Map.Tile>();
                return this.GetNearbyTiles(tile, range);
            }
            /// <summary>
            /// Gets a collection of adjacent tiles.
            /// </summary>
            /// <param name="tile">The parent tile.</param>
            /// <returns></returns>
            public TileCollection GetAdjacentTileCollection(Map.Tile tile)
            {
                return this.GetNearbyTileCollection(tile, 1);
            }
            /// <summary>
            /// Gets a collection of adjacent tiles.
            /// </summary>
            /// <param name="loc">The parent tile's world location.</param>
            /// <returns></returns>
            public TileCollection GetAdjacentTileCollection(Objects.Location loc)
            {
                Map.Tile tile = this.GetTile(loc);
                if (tile == null) return new TileCollection(this.Client, Enumerable.Empty<Map.Tile>());
                return this.GetAdjacentTileCollection(tile);
            }
            /// <summary>
            /// Gets a collection of adjacent tiles.
            /// </summary>
            /// <param name="tile">The parent tile.</param>
            /// <returns></returns>
            public IEnumerable<Map.Tile> GetAdjacentTiles(Map.Tile tile)
            {
                return this.GetNearbyTiles(tile, 1);
            }
            /// <summary>
            /// Gets a collection of adjacent tiles.
            /// </summary>
            /// <param name="loc">The parent tile's world location.</param>
            /// <returns></returns>
            public IEnumerable<Map.Tile> GetAdjacentTiles(Objects.Location loc)
            {
                Map.Tile tile = this.GetTile(loc);
                if (tile == null) return Enumerable.Empty<Map.Tile>();
                return this.GetNearbyTiles(tile, 1);
            }
            /// <summary>
            /// Gets a tile containing a TileObject with given data. Returns null if unsuccessful.
            /// </summary>
            /// <param name="id">The ID of an item, or type of a creature.</param>
            /// <param name="count">The count or extra of an item, or the ID of a creature.</param>
            /// <returns></returns>
            public Map.Tile GetTile(ushort id = 0, uint count = 0)
            {
                if (id == 0 && count == 0) return null;
                foreach (Map.Tile t in this.GetTiles())
                {
                    foreach (Map.TileObject to in t.GetObjects())
                    {
                        if ((id == 0 || id == to.ID) && (count == 0 || count == to.Count)) return t;
                    }
                }
                return null;
            }
            /// <summary>
            /// Gets a tile based on a given world location. Returns null if unsuccessful.
            /// </summary>
            /// <param name="worldLocation">The world location of the tile.</param>
            /// <returns></returns>
            public Map.Tile GetTile(Objects.Location worldLocation)
            {
                foreach (Map.Tile tile in this.GetTiles())
                {
                    if (tile != null && tile.WorldLocation == worldLocation) return tile;
                }
                return null;
            }
            public Map.Tile GetClosestNearbyTile(Map.Tile fromTile, Map.Tile toTile, byte range = 1)
            {
                if (fromTile == null || toTile == null) return null;

                int closestDistance = 100;
                Map.Tile closestTile = null;

                foreach (Map.Tile t in this.GetNearbyTileCollection(toTile, range).GetTiles())
                {
                    if (!t.IsWalkable() && !t.ContainsCreature(this.Client.Player.ID)) continue;

                    var nodesToTile = fromTile.WorldLocation.GetTilesToLocation(this.Client, t.WorldLocation, this, true).ToArray();
                    if (nodesToTile.Length == 0) continue;
                    if (nodesToTile.Length >= closestDistance) continue;
                    closestDistance = nodesToTile.Length;
                    closestTile = t;
                }

                return closestTile;
            }
            public IEnumerable<Map.Tile> GetTilesWithObjects(IEnumerable<ushort> ids)
            {
                foreach (Map.Tile t in this.GetTiles())
                {
                    foreach (ushort id in ids)
                    {
                        if (!t.ContainsObject(id)) continue;
                        yield return t;
                        break;
                    }
                }
            }
            public TileCollection GetTileCollectionWithObjects(IEnumerable<ushort> ids)
            {
                return new TileCollection(this.Client, this.GetTilesWithObjects(ids));
            }
            public IEnumerable<Map.Tile> GetTilesWithObject(ushort id)
            {
                foreach (Map.Tile t in this.GetTiles())
                {
                    if (!t.ContainsObject(id)) continue;
                    yield return t;
                }
            }
            public TileCollection GetTileCollectionWithObject(ushort id)
            {
                return new TileCollection(this.Client, this.GetTilesWithObject(id));
            }

            public IEnumerator<Tile> GetEnumerator()
            {
                return this.Tiles.GetEnumerator();
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Tiles.GetEnumerator();
            }
            public Tile this[int index]
            {
                get { return this.Tiles[index]; }
            }
        }
    }
}
