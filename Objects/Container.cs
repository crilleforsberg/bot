using System;
using System.Collections.Generic;
using System.Text;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that represents an in-game container.
    /// </summary>
    public class Container
    {
        #region constructors
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        /// <param name="orderNumber">The index of this container.</param>
        public Container(Client c, byte orderNumber)
        {
            this.Client = c;
            this.OrderNumber = orderNumber;
            this.Address = this.Client.Addresses.Containers.Start +
                this.Client.Addresses.Containers.Step * orderNumber;
        }
        #endregion

        #region properties
        /// <summary>
        /// The memory address of this container.
        /// </summary>
        private int Address { get; set; }
        /// <summary>
        /// Gets the client associated with this container.
        /// </summary>
        public Client Client { get; private set; }
        /// <summary>
        /// Returns true if this container is opened.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.Containers.Distances.IsOpen) == 1;
                //return this.Client.Memory.ReadByte(this.Client.Player.TibianicPointer +
                //    this.Client.Addresses.Misc.TibianicOffsetContainerIsOpen +
                //    this.OrderNumber * this.Client.Addresses.Containers.Step) == 1;
            }
        }
        /// <summary>
        /// Gets or sets this container's index number.
        /// </summary>
        public byte OrderNumber { get; set; }
        /// <summary>
        /// Gets this container's item ID.
        /// </summary>
        public uint ID
        {
            get { return this.Client.Memory.ReadUInt32(this.Address + this.Client.Addresses.Containers.Distances.ID); }
        }
        /// <summary>
        /// Gets the amount of items currently in this container.
        /// </summary>
        public byte ItemsAmount
        {
            get { return this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.Containers.Distances.Amount); }
        }
        /// <summary>
        /// Gets the amount of slots currently in this container.
        /// </summary>
        public byte Slots
        {
            get { return this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.Containers.Distances.Slots); }
        }
        /// <summary>
        /// Returns true if this container is full.
        /// </summary>
        public bool IsFull
        {
            get { return this.ItemsAmount == Slots; }
        }
        /// <summary>
        /// Gets or sets this container's name.
        /// </summary>
        public string Name
        {
            get { return this.Client.Memory.ReadString(this.Address + this.Client.Addresses.Containers.Distances.Name); }
            set { this.Client.Memory.WriteString(this.Address + this.Client.Addresses.Containers.Distances.Name, value); }
        }
        public bool HasParent
        {
            get { return this.Client.Memory.ReadByte(this.Address + this.Client.Addresses.Containers.Distances.HasParent) == 1; }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets this container's items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Item> GetItems()
        {
            if (!this.IsOpen) yield break;
            for (byte i = 0; i < this.ItemsAmount; i++)
            {
                Item item = new Item(this.Client);
                item.Address = this.Address + this.Client.Addresses.Containers.Distances.FirstItem +
                    this.Client.Addresses.Containers.ItemStep * i;
                item.ID = this.Client.Memory.ReadUInt16(item.Address + this.Client.Addresses.Item.Distances.ID);
                item.Count = this.Client.Memory.ReadByte(item.Address + this.Client.Addresses.Item.Distances.Count);
                item.ContainerNumber = this.OrderNumber;
                item.Slot = i;
                yield return item;
            }
        }
        /// <summary>
        /// Gets all items with a given ID in this container.
        /// </summary>
        /// <param name="id">The item ID to look for.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetItems(ushort id)
        {
            foreach (Item item in this.GetItems())
            {
                if (item.ID == id) yield return item;
            }
        }
        /// <summary>
        /// Gets all items with given IDs in this container.
        /// </summary>
        /// <param name="ids">The collection of IDs to look for.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetItems(IEnumerable<ushort> ids)
        {
            foreach (Item item in this.GetItems())
            {
                foreach (ushort id in ids)
                {
                    if (item.ID == id)
                    {
                        yield return item;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Gets an item in this container. Returns null if unsucessful.
        /// </summary>
        /// <param name="id">The item ID to look for.</param>
        /// <returns></returns>
        public Item GetItem(ushort id)
        {
            if (!this.IsOpen) return null;
            foreach (Item item in this.GetItems())
            {
                if (item.ID == id) return item;
            }
            return null;
        }
        /// <summary>
        /// Gets an item in this container. Returns null if unsucessful.
        /// </summary>
        /// <param name="id">The item ID to look for.</param>
        /// <param name="count">The item count or extra byte to look for.</param>
        /// <returns></returns>
        public Item GetItem(ushort id, byte count)
        {
            foreach (Item item in this.GetItems())
            {
                if (item.ID == id && item.Count == count) return item;
            }
            return null;
        }
        /// <summary>
        /// Gets an item in a given slot. Returns null if unsucessful.
        /// </summary>
        /// <param name="slot">The container's slot</param>
        /// <returns></returns>
        public Item GetItemInSlot(byte slot)
        {
            if (!this.IsOpen || this.Slots <= slot || this.ItemsAmount <= slot) return null;
            int address = this.Address + this.Client.Addresses.Containers.Distances.FirstItem +
                this.Client.Addresses.Containers.ItemStep * slot;
            return new Item(this.Client, address,
                this.Client.Memory.ReadUInt16(address + this.Client.Addresses.Item.Distances.ID),
                this.Client.Memory.ReadUInt16(address + this.Client.Addresses.Item.Distances.Count),
                this.OrderNumber, slot);
        }
        /// <summary>
        /// Gets the first empty slot. Returns null if unsucessful.
        /// </summary>
        /// <returns></returns>
        public ItemLocation GetFirstEmptySlot()
        {
            if (!this.IsOpen || this.IsFull) return null;
            Item item = new Item(this.Client);
            byte itemSlot = this.ItemsAmount;
            item.Address = this.Address + this.Client.Addresses.Containers.ItemStep * itemSlot;
            item.Count = 0;
            item.ContainerNumber = this.OrderNumber;
            item.ID = 0;
            item.Slot = itemSlot;
            return item.ToItemLocation();
        }
        public bool ContainsItemLocation(ItemLocation itemLoc)
        {
            return itemLoc.WorldLocation.X == 0xFFFF &&
                itemLoc.WorldLocation.Y == this.OrderNumber + Constants.Inventory.MinimumContainerNumber;
        }
        public ItemLocation GetBestSlot(Item item)
        {
            return this.GetBestSlot(item.ToItemLocation());
        }
        /// <summary>
        /// Attempts to get the best slot for an item location to move into this container. Returns null if unsuccessful.
        /// </summary>
        /// <param name="itemToMove">The item location to move into this container.</param>
        /// <returns></returns>
        public ItemLocation GetBestSlot(ItemLocation itemToMove)
        {
            if (!this.IsOpen) return null;

            // check if item is non-stackable
            if (itemToMove.ItemCount == 0) return this.GetFirstEmptySlot(); 

            foreach (Item item in this.GetItems())
            {
                if (item.ID != itemToMove.ItemID) continue;
                if (item.Count + itemToMove.ItemCount <= 100) return item.ToItemLocation();
            }
            return this.GetFirstEmptySlot();
        }
        /// <summary>
        /// Attempts to close this container.
        /// </summary>
        public void Close()
        {
            if (!this.IsOpen) return;
            this.Client.Packets.CloseContainer(this);
        }
        public void OpenParentContainer()
        {
            if (!this.HasParent) return;
            this.Client.Packets.OpenParentContainer(this);
        }
        #endregion
    }
}
