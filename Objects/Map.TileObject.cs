using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KarelazisBot.Objects
{
    public partial class Map
    {
        /// <summary>
        /// A class for an object on a tile.
        /// </summary>
        public class TileObject
        {
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="parent">The tile that will host this object.</param>
            /// <param name="data">First chunk of data, i.e. item ID.</param>
            /// <param name="dataEx">Second chunk of data, i.e. item count or creature ID.</param>
            /// <param name="stackIndex">This object's stack index.</param>
            public TileObject(Tile parent, int address, ushort data, uint dataEx, byte stackIndex)
            {
                this.Parent = parent;
                this.Address = address;
                this.ID = data;
                this.Count = dataEx;
                this.StackIndex = stackIndex;
            }

            /// <summary>
            /// This object's parent tile.
            /// </summary>
            public Tile Parent { get; set; }
            /// <summary>
            /// Gets or sets the memory address for this tile object.
            /// </summary>
            public int Address { get; set; }
            /// <summary>
            /// Creatures: Type, Items: ID
            /// </summary>
            public ushort ID { get; set; }
            /// <summary>
            /// Creatures: ID, Items: Count/ExtraByte
            /// </summary>
            public uint Count { get; set; }
            /// <summary>
            /// This object's stack index.
            /// </summary>
            public byte StackIndex { get; set; }
            /// <summary>
            /// This object's item properties.
            /// </summary>
            private Objects.ObjectProperties Properties { get; set; }
            /// <summary>
            /// Checks whether this object has one or more properties.
            /// </summary>
            /// <param name="client">The client to get item properties from.</param>
            /// <param name="flag">The flag(s) to check for.</param>
            /// <returns></returns>
            public bool HasFlag(Enums.ObjectPropertiesFlags flag)
            {
                if (this.Parent == null) return false;
                if (this.Properties == null) this.Properties = this.Parent.Client.GetObjectProperty(this.ID);
                return this.Properties.HasFlag(flag);
            }

            /// <summary>
            /// Creates an item location, useful for sending packets.
            /// </summary>
            /// <returns></returns>
            public Objects.ItemLocation ToItemLocation()
            {
                return new Objects.ItemLocation(this);
            }
            /// <summary>
            /// Attempts to move this object to a given item location.
            /// </summary>
            /// <param name="c">The client used to perform the operation on.</param>
            /// <param name="toLocation">The item location to move this item to.</param>
            public void Move(Objects.ItemLocation toLocation)
            {
                this.Parent.Client.Packets.MoveItem(this.ToItemLocation(), toLocation);
            }
            /// <summary>
            /// Attempts to use this object.
            /// </summary>
            /// <param name="c">The client used to perform the operation on.</param>
            public void Use()
            {
                this.Parent.Client.Packets.UseItem(this.ToItemLocation(), true);
            }
            public void UseOnLocation(Location loc)
            {
                this.Parent.Client.Packets.UseItemOnLocation(this.ToItemLocation(), loc);
            }
            public void UseOnTile(Tile tile)
            {
                this.Parent.Client.Packets.UseItemOnTile(this.ToItemLocation(), tile);
            }
            public void UseOnTileObject(TileObject tileObject)
            {
                this.Parent.Client.Packets.UseItemOnTileObject(this.ToItemLocation(), tileObject);
            }
            /// <summary>
            /// Attempts to open this item as a container, if the player is adjacent to it.
            /// Will return null if unsuccesful.
            /// </summary>
            /// <param name="time">The amount of time, in milliseconds, to wait for the container to open.</param>
            /// <returns></returns>
            public Container TryOpen(ushort time = 1000)
            {
                if (!this.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) return null;
                if (!this.Parent.Client.Player.Location.IsAdjacentTo(this.Parent.WorldLocation)) return null;
                var container = this.Parent.Client.Inventory.GetFirstClosedContainer();
                if (container == null) return null;
                this.Use();
                int startTime = Environment.TickCount;
                while (Environment.TickCount - startTime < time && !container.IsOpen) Thread.Sleep(50);
                return container.IsOpen ? container : null;
            }
        }
    }
}
