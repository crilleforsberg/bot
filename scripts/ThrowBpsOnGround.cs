/* this script will throw containers containing loot on the ground
 * open the parent container for your containers containing loot before running this script
 * 
 * typical layout for your loot:
 * parent container
 *   -> 20 containers
 *      -> 20 items
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    class ItemDescription
    {
        public ItemDescription(ushort id, Location loc)
        {
            this.ID = id;
            this.Location = loc;
            this.Count = 0;
        }
        public ushort ID;
        public Location Location;
        public int Count;
    }

    public static void Main(Client client)
    {
        List<ItemDescription> items = new List<ItemDescription>()
        {
            new ItemDescription(2787, new Location(1, 1, 0)),
            new ItemDescription(2513, new Location(0, -1, 0)),
            new ItemDescription(2417, new Location(-1, -1, 0))
        };
        Location undecidedLoc = new Location(1, 0, 0);
        int minimumPercent = 80; // >=80% item consistency required for a decision

        var comparer = new Comparison<KeyValuePair<ushort, int>>(
            delegate(KeyValuePair<ushort, int> first, KeyValuePair<ushort, int> second)
            {
                return first.Value.CompareTo(second.Value);
            });
        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            foreach (var parentItem in client.Inventory.GetItems())
            {
                if (!parentItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;
                var container = client.Inventory.GetFirstClosedContainer();
                if (container == null) break;
                parentItem.OpenInNewWindow();
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    if (!container.IsOpen) continue;
                    break;
                }
                if (!container.IsOpen) continue;

                int itemCount = container.ItemsAmount;

                foreach (var itemDesc in items) itemDesc.Count = 0;

                foreach (var childItem in container.GetItems())
                {
                    foreach (var itemDesc in items)
                    {
                        if (itemDesc.ID != childItem.ID) continue;
                        itemDesc.Count++;
                        break;
                    }
                }

                // find which item passed
                ItemDescription bestItem = null;
                foreach (var itemDesc in items)
                {
                    if (itemDesc.Count == 0) continue;
                    if ((itemDesc.Count * 100) / itemCount < minimumPercent) continue;
                    bestItem = itemDesc;
                    break;
                }

                parentItem.Move(client.Player.Location.Offset(bestItem == null ? undecidedLoc : bestItem.Location).
                    ToItemLocation());
                parentItem.WaitForInteraction(500);
                container.Close();

                break;
            }
        }
    }
}
