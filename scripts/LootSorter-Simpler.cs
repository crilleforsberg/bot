// this script will sort loot into designated containers

using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects


public class Test
{
    public static void Main(Client client)
    {
        // this dictionary will serve as an item lookup database of sorts
        // key = item id, value = container number (0 = first container)
        // you can add as many as you want, you can have several item ids
        // with the same container number if you want to
        Dictionary<ushort, byte> itemDictionary = new Dictionary<ushort, byte>();
        itemDictionary.Add(2148, 0); // sort gold into the first container

        while (true)
        {
            if (!client.Player.Connected) return;
            Thread.Sleep(500);

            foreach (Container c in client.Inventory.GetContainers())
            {
                // check if this is a container we're going to put loot in
                // if so, skip it
                bool found = false;
                foreach (KeyValuePair<ushort, byte> keypair in itemDictionary)
                {
                    if (keypair.Value == c.OrderNumber)
                    {
                        found = true;
                        break;
                    }
                }
                if (found) continue;

                // do magic
                int itemIndex = c.ItemsAmount;
                while (itemIndex >= 0)
                {
                    if (!c.IsOpen) break;

					itemIndex--;
					
                    // get item
                    Item item = c.GetItemInSlot((byte)itemIndex);
                    if (item == null) continue;
                    // skip this item if we dont want to sort it
                    if (!itemDictionary.ContainsKey(item.ID)) continue;

                    // get target container
                    Container toContainer = client.Inventory.GetContainer(itemDictionary[item.ID]);
                    // check if it's valid, if not, skip to next item
                    if (toContainer == null || !toContainer.IsOpen) continue;
					// check if container is full, look for a new container if so
					if (toContainer.IsFull)
					{
						foreach (Item i in toContainer.GetItems())
						{
							if (!i.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;
							
							i.Use();
							Thread.Sleep(500);
							break;
						}
					}
					
                    // get target item
                    ItemLocation toItemLoc = toContainer.GetBestSlot(item.ToItemLocation());
                    if (toItemLoc == null) continue;
                    item.Move(toItemLoc);
                    if (item.WaitForInteraction(800)) itemIndex--;
                }

                // look for a new container to open
                foreach (Item i in c.GetItems())
                {
                    if (i.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        i.Use();
                        Thread.Sleep(500);
                        break;
                    }
                }
            }
        }
    }
}