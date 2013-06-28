using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that serves to describe an item's location, no matter its origin.
    /// </summary>
    public class ItemLocation
    {
        /// <summary>
        /// Constructor for an item in the player's inventory.
        /// </summary>
        /// <param name="item"></param>
        public ItemLocation(Objects.Item item)
        {
            this.WorldLocation = item.ToLocation();
            this.ItemID = item.ID;
            this.ItemCount = item.Count;
            this.StackIndex = item.Slot;
        }
        /// <summary>
        /// Constructor for a non-existant item in a equpiment slot.
        /// Use Item.ToItemLocation() for an existing item.
        /// </summary>
        /// <param name="slot">The equipment slot.</param>
        public ItemLocation(Enums.EquipmentSlots slot)
        {
            Objects.Item item = new Objects.Item(null)
            {
                ID = 0,
                Count = 0,
                Slot = 0,
                ContainerNumber = (byte)slot,
            };
            this.WorldLocation = item.ToLocation();
            this.ItemID = item.ID;
            this.ItemCount = item.Count;
            this.StackIndex = item.Slot;
        }
        /// <summary>
        /// Constructor for an item on the ground.
        /// </summary>
        /// <param name="tileObject"></param>
        public ItemLocation(Map.TileObject tileObject)
        {
            this.WorldLocation = tileObject.Parent.WorldLocation;
            this.ItemID = tileObject.ID;
            this.ItemCount = tileObject.Count;
            this.StackIndex = tileObject.StackIndex;
        }
        /// <summary>
        /// Constructor for an unspecified item on the ground.
        /// </summary>
        /// <param name="worldLocation"></param>
        public ItemLocation(Objects.Location worldLocation)
        {
            this.WorldLocation = worldLocation;
        }

        /// <summary>
        /// Gets the location of this item.
        /// </summary>
        public Objects.Location WorldLocation { get; private set; }
        /// <summary>
        /// Gets the ID for this item.
        /// </summary>
        public ushort ItemID { get; private set; }
        /// <summary>
        /// Gets or sets the count for this item. If ItemID is less than 100, it will be the creature/player ID.
        /// </summary>
        public uint ItemCount { get; set; }
        /// <summary>
        /// Gets the stack index of this item. If the item is in a container or an equipment slot, this will be the item's slot.
        /// </summary>
        public byte StackIndex { get; private set; }

        /// <summary>
        /// Returns true if this item is carried in the player's inventory.
        /// </summary>
        /// <returns></returns>
        public bool IsInventoryItem() { return this.WorldLocation.X == 0xFFFF; }

        public override string ToString()
        {
            return this.ItemID + ":" + this.ItemCount;
        }
    }
}
