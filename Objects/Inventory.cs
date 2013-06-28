using System.Collections.Generic;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class for the player's inventory.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client associated with the player.</param>
        public Inventory(Objects.Client c)
        {
            this.Client = c;

            this.CachedContainers = new Container[Constants.Inventory.MaxContainers];
            for (int i = 0; i < this.CachedContainers.Length; i++)
            {
                this.CachedContainers[i] = new Container(this.Client, (byte)i);
            }
        }

        private Container[] CachedContainers { get; set; }

        /// <summary>
        /// Gets the client associated with the player.
        /// </summary>
        public Objects.Client Client { get; private set; }

        public ItemLocation GetFirstEmptySlot()
        {
            foreach (Container c in this.GetContainers())
            {
                var slot = c.GetFirstEmptySlot();
                if (slot != null) return slot;
            }
            return null;
        }
        public ItemLocation GetBestSlot(ItemLocation itemLoc, bool mustBeOtherContainer = true)
        {
            foreach (Container c in this.GetContainers())
            {
                if (mustBeOtherContainer && c.ContainsItemLocation(itemLoc)) continue;
                var best = c.GetBestSlot(itemLoc);
                if (best != null) return best;
            }
            return null;
        }
        /// <summary>
        /// Gets a list of all opened containers, starting at a given index.
        /// </summary>
        /// <param name="firstContainer">The index to start getting opened containers.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Container> GetContainers(byte firstContainer = 0)
        {
            foreach (Container c in this.CachedContainers)
            {
                if (c.IsOpen) yield return c;
            }
        }
        /// <summary>
        /// Gets a single container, using a given index.
        /// </summary>
        /// <param name="containerNumber">The index to use to get the container.</param>
        /// <returns></returns>
        public Objects.Container GetContainer(byte containerNumber)
        {
            return this.CachedContainers[containerNumber];
        }
        /// <summary>
        /// Gets a collection of items in the player's opened containers.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Objects.Item> GetItems()
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                foreach (Objects.Item item in container.GetItems()) yield return item;
            }
        }
        /// <summary>
        /// Gets a collection of items in the player's opened containers.
        /// </summary>
        /// <param name="itemID">The item ID to check for.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Item> GetItems(ushort itemID)
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                foreach (Objects.Item item in container.GetItems())
                {
                    if (item.ID == itemID) yield return item;
                }
            }
        }
        /// <summary>
        /// Gets a list of items in the player's opened containers.
        /// </summary>
        /// <param name="itemID">The item ID to check for.</param>
        /// <param name="count">The item count or extra byte to check for.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Item> GetItems(ushort itemID, ushort count)
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                foreach (Objects.Item item in container.GetItems())
                {
                    if (item.ID == itemID && item.Count == count) yield return item;
                }
            }
        }
        /// <summary>
        /// Gets an item in the player's opened containers. Returns null if unsuccessful.
        /// </summary>
        /// <param name="itemID">The item ID to check for.</param>
        /// <param name="count">The item count or extra byte to check for.</param>
        /// <returns></returns>
        public Objects.Item GetItem(ushort itemID, ushort count)
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                foreach (Objects.Item item in container.GetItems())
                {
                    if (item.ID == itemID && item.Count == count) return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets an item in the player's opened containers. Returns null if unsuccessful.
        /// </summary>
        /// <param name="itemID">The item ID to check for.</param>
        /// <returns></returns>
        public Objects.Item GetItem(ushort itemID)
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                foreach (Objects.Item item in container.GetItems())
                {
                    if (item.ID == itemID) return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Attempts to get a suitable slot for a given item. If none is found, null is returned.
        /// </summary>
        /// <param name="startingContainer">The container index to start looking for a suitable slot.</param>
        /// <param name="fromItem">The item to use for comparisons.</param>
        /// <returns></returns>
        public Objects.ItemLocation GetFirstSuitableSlot(Objects.Item fromItem, byte startingContainer = 0)
        {
            int maxContainerValue = Constants.Inventory.MaxContainers;
            byte fromNumber = fromItem.ContainerNumber >= maxContainerValue ?
                (byte)(fromItem.ContainerNumber - maxContainerValue) :
                fromItem.ContainerNumber;
            foreach (Objects.Container container in this.GetContainers(startingContainer))
            {
                byte toNumber = container.OrderNumber >= maxContainerValue ?
                    (byte)(container.OrderNumber - maxContainerValue) :
                    container.OrderNumber;
                if (fromNumber == toNumber) continue;

                if (fromItem.Count > 0) // check if there is an existing item with the same ID (stackable)
                {
                    foreach (Objects.Item item in container.GetItems())
                    {
                        if (fromItem.ID == item.ID &&
                            (fromItem.Count + item.Count <= 100 || container.ItemsAmount < container.Slots))
                        {
                            return item.ToItemLocation();
                        }
                    }
                }
                // check if container has room for a non-stacking item
                if (!container.IsFull) return container.GetFirstEmptySlot();
            }
            return null;
        }
        /// <summary>
        /// Attemps to find a non-full container. Returns null if unsuccessful.
        /// </summary>
        /// <returns></returns>
        public Objects.Container GetFirstNonFullContainer()
        {
            foreach (Objects.Container container in this.GetContainers())
            {
                if (container.ItemsAmount < container.Slots)
                {
                    return container;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the first empty container index in memory.
        /// </summary>
        /// <returns></returns>
        public byte GetClosedContainerNumber()
        {
            foreach (Container c in this.CachedContainers)
            {
                if (!c.IsOpen) return c.OrderNumber;
            }
            return byte.MaxValue;
        }
        /// <summary>
        /// Gets the first empty container. Returns null if unsuccessful.
        /// </summary>
        /// <returns></returns>
        public Objects.Container GetFirstClosedContainer()
        {
            foreach (Container c in this.CachedContainers)
            {
                if (!c.IsOpen) return c;
            }
            return null;
        }
        /// <summary>
        /// Groups any stackable items together. Will ignore fluid containers by default.
        /// </summary>
        public void GroupItems()
        {
            this.GroupItems(new List<ushort>());
        }
        /// <summary>
        /// Groups any stackable items together. Note that this will put the calling thread to sleep.
        /// </summary>
        /// <param name="ignoredItems">A list of ignored items. Fluid containers are ignored by default.</param>
        public void GroupItems(List<ushort> ignoredItems)
        {
            while (true)
            {
                bool found = false;

                foreach (var container in this.GetContainers())
                {
                    if (!container.IsOpen) continue;

                    foreach (var fromItem in container.GetItems())
                    {
                        if (found) break;
                        if (fromItem.Count == 100 || fromItem.Count == 0) continue;
                        if (fromItem.HasFlag(Enums.ObjectPropertiesFlags.IsFluidContainer)) continue;
                        if (ignoredItems.Contains(fromItem.ID)) continue;

                        foreach (var toItem in container.GetItems())
                        {
                            if (fromItem.ID != toItem.ID) continue;
                            if (fromItem.Slot == toItem.Slot) continue;
                            if (toItem.Count == 100) continue;
                            found = true;
                            fromItem.Move(toItem.ToItemLocation());
                            fromItem.WaitForInteraction(800);
                            break;
                        }
                    }

                    if (found) break;
                }

                if (!found) break;
            }
        }
        /// <summary>
        /// Gets an item in a player's equipment slot. Returns null if unsuccessful.
        /// </summary>
        /// <param name="slot">The equipment slot to get the item from.</param>
        /// <returns></returns>
        public Objects.Item GetItemInSlot(Enums.EquipmentSlots slot)
        {
            Item item = null;
            switch (slot)
            {
                case Enums.EquipmentSlots.Ammo:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqAmmo, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqAmmo),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqAmmo + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Ammo);
                    break;
                case Enums.EquipmentSlots.Torso:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqTorso, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqTorso),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqTorso + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Torso);
                    break;
                case Enums.EquipmentSlots.Container:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqContainer, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqContainer),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqContainer + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Container);
                    break;
                case Enums.EquipmentSlots.Feet:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqFeet, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqFeet),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqFeet + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Feet);
                    break;
                case Enums.EquipmentSlots.Head:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqHead, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqHead),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqHead + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Head);
                    break;
                case Enums.EquipmentSlots.LeftHand:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqLeftHand, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqLeftHand),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqLeftHand + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.LeftHand);
                    break;
                case Enums.EquipmentSlots.Neck:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqNeck, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqNeck),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqNeck + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Neck);
                    break;
                case Enums.EquipmentSlots.RightHand:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqRightHand, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqRightHand),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqRightHand + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.RightHand);
                    break;
                case Enums.EquipmentSlots.Ring:
                    item = new Objects.Item(this.Client, this.Client.Addresses.Containers.EqRing, this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqRing),
                        this.Client.Memory.ReadUInt16(this.Client.Addresses.Containers.EqRing + this.Client.Addresses.Item.Distances.Count), Enums.EquipmentSlots.Ring);
                    break;
                default:
                    return null;
            }
            if (item == null || item.ID == 0) return null;
            return item;
        }
    }
}
