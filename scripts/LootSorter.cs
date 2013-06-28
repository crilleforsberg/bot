// this script will sort loot into backpacks containing empty backpacks
// each slot in the backpack should be a container
// each master backpack will be reserved for one item id

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        if (!client.Player.Connected) return;

        // item lookup dictionary, used to store master container
        // key = item id, value = master container order number
        Dictionary<ushort, byte> itemDictionary = new Dictionary<ushort, byte>();
        // add items to the dictionary
        itemDictionary.Add(2148, 0); // 2148 = gold item ID, 0 = container number (0 = first container)

        // container lookup dictionary, used to find next empty container
        // key = item id, value = child container index
        Dictionary<ushort, byte> containerDictionary = new Dictionary<ushort, byte>();
        foreach (KeyValuePair<ushort, byte> keypair in itemDictionary)
        {
            containerDictionary.Add(keypair.Key, 0);
        }
        while (true)
        {
            if (!client.Player.Connected) return;
            Thread.Sleep(500);

            foreach (Container c in client.Inventory.GetContainers())
            {
                // check if this is a container we're going to put loot in
                // if so, skip it
                if (itemDictionary.ContainsValue(c.OrderNumber)) continue;

                // check if this container contains more than one container
                // if so, skip it
                int count = 0;
                foreach (Item i in c.GetItems())
                {
                    if (i.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) count++;
                }
                if (count >= 2) continue;

                // do magic
                int itemIndex = c.ItemsAmount - 1;
                while (itemIndex >= 0)
                {
                    if (!c.IsOpen) break;

                    // get item
                    Item item = c.GetItemInSlot((byte)itemIndex);
                    // check if item is valid
                    if (item == null || item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        itemIndex--;
                        continue;
                    }
                    // check if we want this item, if not, continue to the next item
                    if (!itemDictionary.ContainsKey(item.ID))
                    {
                        itemIndex--;
                        continue;
                    }

                    // get target container
                    Container toContainer = client.Inventory.GetContainer(itemDictionary[item.ID]);
                    // check if target container is valid
                    if (toContainer == null || !toContainer.IsOpen)
                    {
                        itemIndex--;
                        continue;
                    }
                    // get child container
                    Item toItem = toContainer.GetItemInSlot(containerDictionary[item.ID]);
                    // check if it's valid
                    while (toItem == null ||
                        !toItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        containerDictionary[item.ID]++;
                        toItem = toContainer.GetItemInSlot(containerDictionary[item.ID]);
                    }
                    // check if we have exhausted the number of child containers we can loot to
                    if (containerDictionary[item.ID] >= toContainer.ItemsAmount)
                    {
                        itemIndex--;
                        continue;
                    }

                    if (toItem != null && toItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        item.Move(toItem);
                        if (!item.WaitForInteraction(800))
                        {
                            if (client.StatusBar.GetText().Contains("You cannot put more objects"))
                            {
                                containerDictionary[item.ID]++;
                                client.StatusBar.SetText(string.Empty);
                                Thread.Sleep(100);
                            }
                        }
                        else itemIndex--;
                    }
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
