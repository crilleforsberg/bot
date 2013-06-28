using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KarelazisBot.Objects
{
    public partial class Map
    {
        /// <summary>
        /// A class that represents an in-game tile.
        /// </summary>
        public class Tile
        {
            /// <summary>
            /// Constructor for an empty tile.
            /// </summary>
            public Tile(Objects.Client client)
                : this(client, 0, 0, Enumerable.Empty<TileObject>(), new Objects.Location()) { }
            public Tile(Objects.Client client, int address, int tileNumber)
                : this(client, address, tileNumber, Enumerable.Empty<TileObject>(), new Objects.Location()) { }
            /// <summary>
            /// Constructor for a tile.
            /// </summary>
            /// <param name="address">This tile's memory address.</param>
            /// <param name="tileNumber">This tile's index number.</param>
            /// <param name="objects">A list of tile objects.</param>
            /// <param name="location">This tile's world location.</param>
            public Tile(Objects.Client client, int address, int tileNumber, IEnumerable<TileObject> objects, Objects.Location location)
            {
                this.Client = client;
                this.Address = address;
                this.TileNumber = tileNumber;
                this.Objects = objects.ToList();
                this.WorldLocation = location;
            }

            /// <summary>
            /// Clones this tile.
            /// </summary>
            /// <returns></returns>
            public Tile Clone()
            {
                Tile tile = new Tile(this.Client, this.Address, this.TileNumber, this.Objects, this.WorldLocation);
                tile.MemoryLocation = this.MemoryLocation;
                return tile;
            }

            /// <summary>
            /// Gets the client associated with this object.
            /// </summary>
            public Objects.Client Client { get; private set; }
            /// <summary>
            /// Gets or sets this tile's memory address.
            /// </summary>
            public int Address { get; set; }
            /// <summary>
            /// Gets or sets this tile's index number.
            /// </summary>
            public int TileNumber { get; set; }
            /// <summary>
            /// Gets or sets this tile's world location.
            /// </summary>
            public Objects.Location WorldLocation { get; set; }
            /// <summary>
            /// Gets or sets this tile's memory location. Useful for pathfinding.
            /// </summary>
            public Objects.Location MemoryLocation { get; set; }
            /// <summary>
            /// Gets or sets this tile's real memory location. Used for converting to a tile number.
            /// </summary>
            public Objects.Location RealMemoryLocation { get; set; }
            /// <summary>
            /// Gets this tile's first object. Returns null if this tile is empty.
            /// </summary>
            public TileObject Ground
            {
                get
                {
                    TileObject[] objs = this.GetObjects().ToArray();
                    if (objs.Length > 0) return objs[0];
                    return null;
                }
            }
            /// <summary>
            /// Gets the amount of TileObjects this tile has.
            /// </summary>
            public int ObjectsCount
            {
                //get { return this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.MapDistances.TileObjectsCount); }
                get { return this.Objects.Count; }
            }

            internal List<TileObject> Objects { get; set; }

            /// <summary>
            /// Gets all TileObjects on this tile.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<TileObject> GetObjects()
            {
                /*for (byte i = 0; i < this.ObjectsCount; i++)
                {
                    int address = this.Address + i * this.Client.Addresses.Map.ObjectStep;
                    yield return new TileObject(this, address,
                        this.Client.Memory.ReadUInt16(address + this.Client.Addresses.MapDistances.ObjectData),
                        this.Client.Memory.ReadUInt32(address + this.Client.Addresses.MapDistances.ObjectDataEx),
                        i);
                }*/

                return this.Objects;
            }
            public void UpdateObjects()
            {
                this.Objects.Clear();
                byte count = this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.Map.Distances.TileObjectsCount);
                for (byte j = 0; j < count; j++)
                {
                    int objAddress = this.Address + j * this.Client.Addresses.Map.ObjectStep;
                    this.Objects.Add(new TileObject(this, objAddress,
                        this.Client.Memory.ReadUInt16(objAddress + this.Client.Addresses.Map.Distances.ObjectData),
                        this.Client.Memory.ReadUInt32(objAddress + this.Client.Addresses.Map.Distances.ObjectDataEx),
                        j));
                }
            }
            /// <summary>
            /// Checks whether this tile contains a creature.
            /// </summary>
            /// <returns></returns>
            public bool ContainsCreature()
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.ID < 100) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks whether this tile contains a specific creature.
            /// </summary>
            /// <param name="id">The ID of the creature to look for.</param>
            /// <returns></returns>
            public bool ContainsCreature(uint id)
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.Count == id) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks whether this tile contains a specific object ID.
            /// </summary>
            /// <param name="id">The item ID to look for.</param>
            /// <returns></returns>
            public bool ContainsObject(ushort id)
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.ID == id) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks whether this item is walkable.
            /// </summary>
            /// <param name="considerPathBlockingItems">Whether to consider items that only blocks pathfinding, such as parcels.</param>
            /// <returns></returns>
            public bool IsWalkable(bool considerPathBlockingItems = true)
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.ID < 100) return false;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsBlocking) ||
                        to.HasFlag(Enums.ObjectPropertiesFlags.IsFloorChange) ||
                        (considerPathBlockingItems && to.HasFlag(Enums.ObjectPropertiesFlags.IsPathBlocking)))
                    {
                        return false;
                    }
                }
                return true;
            }
            /// <summary>
            /// Checks whether this tile contains a ladder.
            /// </summary>
            /// <returns></returns>
            public bool ContainsLadder()
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.ID < 100) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsAlwaysTopUse)) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks whether this tile contains a rope hole.
            /// </summary>
            /// <returns></returns>
            public bool ContainsRopeHole()
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.ID < 100) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.HasAutomapColor |
                        Enums.ObjectPropertiesFlags.HasHelpLens |
                        Enums.ObjectPropertiesFlags.IsGround)) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks whether this tile contains an object with one or more specified flags.
            /// </summary>
            /// <param name="c">The client to get item properties from.</param>
            /// <param name="flags">The flags to check for. Can be more than one.</param>
            /// <returns></returns>
            public bool ContainsObjectProperty(Enums.ObjectPropertiesFlags flags)
            {
                foreach (TileObject to in this.GetObjects())
                {
                    if (to.HasFlag(flags)) return true;
                }
                return false;
            }
            /// <summary>
            /// Gets a TileObject that is used for using items.
            /// </summary>
            /// <param name="considerCreatures">Set this to true if you intend to i.e. use a rope on this tile.</param>
            /// <returns></returns>
            public TileObject GetTopUseItem(bool considerCreatures)
            {
                var objects = this.GetObjects().ToArray();

                // check for always top use items
                if (!considerCreatures)// || this.Client.TibiaVersion >= 760)
                {
                    foreach (TileObject to in objects)
                    {
                        if (to.ID < 100) continue;
                        if (to.HasFlag(Enums.ObjectPropertiesFlags.IsAlwaysTopUse)) return to;
                    }
                }

                // check for items/creatures
                foreach (TileObject to in objects)
                {
                    if (considerCreatures && to.ID < 100) return to;
                    else if (to.ID < 100) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsSplash)) continue;
                    if (!to.HasFlag(Enums.ObjectPropertiesFlags.IsGround) &&
                        !to.HasFlag(Enums.ObjectPropertiesFlags.IsTopUseIfOnly) &&
                        !to.HasFlag(Enums.ObjectPropertiesFlags.IsWalkThrough))
                    {
                        return to;
                    }
                }

                // check for toporder items
                foreach (TileObject to in objects)
                {
                    if (to.ID < 100) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsSplash)) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsTopUseIfOnly) ||
                        to.HasFlag(Enums.ObjectPropertiesFlags.IsWalkThrough))
                    {
                        return to;
                    }
                }
                // return ground
                return this.Ground;
            }
            /// <summary>
            /// Gets a TileObject that is used for moving an item or creature.
            /// </summary>
            /// <param name="client">The client to get item properties from.</param>
            /// <returns></returns>
            public TileObject GetTopMoveItem()
            {
                var objects = this.GetObjects().ToArray();

                // check for items
                foreach (TileObject to in objects)
                {
                    if (to.ID < 100) continue;
                    if (to.HasFlag(Enums.ObjectPropertiesFlags.IsSplash)) continue;
                    if (!to.HasFlag(Enums.ObjectPropertiesFlags.IsGround) &&
                        !to.HasFlag(Enums.ObjectPropertiesFlags.IsTopUseIfOnly) &&
                        !to.HasFlag(Enums.ObjectPropertiesFlags.IsWalkThrough))
                    {
                        return to;
                    }
                }
                // check for creatures
                foreach (TileObject to in objects)
                {
                    if (to.ID < 100) return to;
                }
                // return ground, as no item/creature was found
                return this.Ground;
            }
            /// <summary>
            /// Creates an item location for this tile's world location. Useful for sending packets.
            /// </summary>
            /// <returns></returns>
            public Objects.ItemLocation ToItemLocation() { return new Objects.ItemLocation(this.WorldLocation); }
        }
    }
}
