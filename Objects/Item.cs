using System;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class for an in-game item stored in the player's inventory.
    /// </summary>
    public class Item
    {
        #region constructors
        /// <summary>
        /// Constructor for an empty item.
        /// </summary>
        /// <param name="c">The client to be associated with this item.</param>
        public Item(Objects.Client c) { this.Client = c; }
        /// <summary>
        /// Constructor an item.
        /// </summary>
        /// <param name="c">The client to be associated with this item.</param>
        /// <param name="address">The memory address of this item.</param>
        /// <param name="id">The item ID.</param>
        /// <param name="count">The item count or extra byte.</param>
        /// <param name="containerNumber">The parent container's index number.</param>
        /// <param name="slot">The item's slot.</param>
        public Item(Objects.Client c, int address, ushort id, ushort count, byte containerNumber, byte slot)
        {
            this.Client = c;
            this.Address = address;
            this.ID = id;
            this.Count = count;
            this.ContainerNumber = containerNumber;
            this.Slot = slot;
            this.Type = Types.Container;
        }
        public Item(Client c, int address, ushort id, ushort count, Enums.EquipmentSlots eqSlot)
        {
            this.Client = c;
            this.Address = address;
            this.ID = id;
            this.Count = count;
            this.Type = Types.Equipment;
        }
        #endregion

        public override string ToString()
        {
            return "ID: " + this.ID + " Count: " + this.Count;
        }

        #region properties
        /// <summary>
        /// The client associated with this item.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// This item's memory address.
        /// </summary>
        public int Address { get; set; }
        /// <summary>
        /// This item's ID.
        /// </summary>
        public ushort ID { get; set; }
        /// <summary>
        /// This item's count or extra byte.
        /// </summary>
        public ushort Count { get; set; }
        /// <summary>
        /// This item's parent container index number.
        /// </summary>
        public byte ContainerNumber { get; set; }
        /// <summary>
        /// This item's slot.
        /// </summary>
        public byte Slot { get; set; }
        public Types Type { get; private set; }
        /// <summary>
        /// This item's properties.
        /// </summary>
        private Objects.ObjectProperties Properties { get; set; }
        #endregion

        public enum Types
        {
            Container,
            Equipment
        }

        #region methods
        /// <summary>
        /// Attempts to get the parent container. Returns null if unsuccessful.
        /// </summary>
        /// <returns></returns>
        public Objects.Container GetParent()
        {
            if (this.Type != Types.Container) return null;
            return this.Client.Inventory.GetContainer(this.ContainerNumber);
        }
        /// <summary>
        /// Checks whether this item has a given property.
        /// </summary>
        /// <param name="flag">The property to check for.</param>
        /// <returns></returns>
        public bool HasFlag(Enums.ObjectPropertiesFlags flag)
        {
            if (this.Client == null) return false;
            if (this.Properties == null) this.Properties = this.Client.GetObjectProperty(this.ID);
            return this.Properties.HasFlag(flag);
        }
        /// <summary>
        /// Returns true if this item is stored in a container.
        /// </summary>
        /// <returns></returns>
        public bool IsContainerItem()
        {
            return this.Type == Types.Container;
        }
        /// <summary>
        /// Creates an Objects.Location object, primarily used for packets.
        /// </summary>
        /// <returns></returns>
        public Objects.Location ToLocation()
        {
            return new Objects.Location(0xFFFF, this.ContainerNumber +
                (this.IsContainerItem() ? Constants.Inventory.MinimumContainerNumber : 0),
                this.Slot);
        }
        /// <summary>
        /// Creates an item location, primarily used for packets.
        /// </summary>
        /// <returns></returns>
        public Objects.ItemLocation ToItemLocation()
        {
            return new Objects.ItemLocation(this);
        }

        /// <summary>
        /// Attempts to use this item.
        /// </summary>
        public void Use(bool openInNewWindow = false)
        {
            this.Client.Packets.UseItem(this.ToItemLocation(), (this.IsContainerItem() ? openInNewWindow : true));
        }
        /// <summary>
        /// Attempts to use this item on a creature or player.
        /// </summary>
        /// <param name="c">The creature or player to use this item on.</param>
        public void UseOnCreature(Objects.Creature c)
        {
            if (c == null) return;
            this.UseOnLocation(c.Location);
        }
        /// <summary>
        /// Attempts to use this item on a creature in the battlelist.
        /// </summary>
        /// <param name="c">The creature to use this item on.</param>
        public void UseOnBattleList(Objects.Creature c)
        {
            if (c == null) return;
            this.Client.Packets.UseItemOnBattleList(this.ToItemLocation(), c);
        }
        /// <summary>
        /// Attempts to use this item on the player.
        /// </summary>
        public void UseOnSelf() { this.UseOnLocation(this.Client.Player.Location); }
        /// <summary>
        /// Attempts to use this item on a world location.
        /// </summary>
        /// <param name="loc">The world location to use this item on.</param>
        public void UseOnLocation(Objects.Location loc) { this.Client.Packets.UseItemOnLocation(this.ToItemLocation(), loc); }
        /// <summary>
        /// Attempts to use this item on a tile.
        /// </summary>
        /// <param name="tile">The tile to use this item on.</param>
        public void UseOnTile(Map.Tile tile)
        {
            if (tile == null) return;
            this.Client.Packets.UseItemOnTile(this.ToItemLocation(), tile);
        }
        /// <summary>
        /// Attempts to use this item on a tile object.
        /// Only use this method if you got a tile's top item.
        /// </summary>
        /// <param name="tileObject"></param>
        public void UseOnTileObject(Map.TileObject tileObject)
        {
            if (tileObject == null) return;
            this.Client.Packets.UseItemOnTileObject(this.ToItemLocation(), tileObject);
        }
        /// <summary>
        /// Attempts to open this item in a new window.
        /// </summary>
        public Container OpenInNewWindow()
        {
            Container c = this.Client.Inventory.GetFirstClosedContainer();
            if (c == null) return null;
            this.Client.Packets.UseItem(this.ToItemLocation(), true);
            return c;
        }
        /// <summary>
        /// Attempts to move this item to a given item location.
        /// </summary>
        /// <param name="toLocation">The item location to move this item to.</param>
        public void Move(Objects.ItemLocation toLocation)
        {
            this.Move(toLocation, this.Count);
        }
        /// <summary>
        /// Attempts to move this item to a given item location.
        /// </summary>
        /// <param name="toLocation">The item locatino to move this item to.</param>
        /// <param name="count">How many of this item to move.</param>
        public void Move(Objects.ItemLocation toLocation, ushort count)
        {
            if (toLocation == null) return;
            Objects.ItemLocation itemLoc = this.ToItemLocation();
            itemLoc.ItemCount = count;
            this.Client.Packets.MoveItem(itemLoc, toLocation);
        }

        /// <summary>
        /// Waits for an item to be moved, used etc.
        /// Returns true if interaction is completed within the timeframe.
        /// Returns false if time expires.
        /// </summary>
        /// <param name="maxTime">Max amount of milliseconds to wait.</param>
        /// <returns></returns>
        public bool WaitForInteraction(int maxTime)
        {
            if (this.IsContainerItem())
            {
                Objects.Container parent = this.GetParent();
                if (parent == null || !parent.IsOpen) return true;
                byte itemAmount = parent.ItemsAmount;
                for (int i = 0; i < maxTime; i += 50)
                {
                    if (itemAmount != parent.ItemsAmount) return true;
                    Item tempItem = parent.GetItemInSlot(this.Slot);
                    if (tempItem == null) return true;
                    if (tempItem.ID != this.ID || tempItem.Count != this.Count) return true;
                    System.Threading.Thread.Sleep(50);
                }
                return false;
            }
            else
            {
                for (int i = 0; i < maxTime; i += 50)
                {
                    Item tempItem = this.Client.Inventory.GetItemInSlot((Enums.EquipmentSlots)this.ContainerNumber);
                    if (tempItem == null) return true;
                    if (tempItem.ID != this.ID || tempItem.Count != this.Count) return true;
                    else if (tempItem.ID == 0) return true;
                    System.Threading.Thread.Sleep(50);
                }
                return false;
            }
        }
        /// <summary>
        /// Updates the ID and Count values.
        /// </summary>
        public void Update()
        {
            if (this.IsContainerItem())
            {
                Objects.Container parent = this.GetParent();
                if (parent == null || !parent.IsOpen || this.Slot >= parent.ItemsAmount)
                {
                    this.ID = 0;
                    this.Count = 0;
                }
                else
                {
                    this.ID = this.Client.Memory.ReadUInt16(this.Address +
                        this.Client.Addresses.Item.Distances.ID);
                    this.Count = this.Client.Memory.ReadUInt16(this.Address +
                        this.Client.Addresses.Item.Distances.Count);
                }
            }
            else
            {
                this.ID = this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.Item.Distances.ID);
                this.Count = this.Client.Memory.ReadUInt16(this.Address + this.Client.Addresses.Item.Distances.Count);
            }
        }
        public Container TryOpenInNewWindow(int time = 800)
        {
            var container = this.Client.Inventory.GetFirstClosedContainer();
            if (container == null) return null;
            this.Use(true);
            for (int i = 0; i < time; i += 50)
            {
                System.Threading.Thread.Sleep(50);
                if (container.IsOpen) break;
            }
            return container.IsOpen ? container : null;
        }
        #endregion
    }
}
