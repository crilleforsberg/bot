/* 
 * this script will pick up defined items using a bp with child bps structure (i.e. a bp containing 20 bps)
 * open the parent container and let it rip
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    class Loot
    {
        public Loot(ushort id, float weight)
        {
            this.ID = id;
            this.Weight = weight;
        }
        public ushort ID;
        public float Weight;
    }

    public static void Main(Client client)
    {
        List<Loot> loot = new List<Loot>()
        {
            new Loot(2689, 5.0f)
        };
        string globalVariableName = "LootGrabberIndexes";
        Location emptyContainerOffset = new Location(2, 0, 0),
            lootContainerOffset = new Location(0, 0, 0);
        bool repeat = true; // whether to push bp away and open a new bp
        Dictionary<Container, int> lootedContainers = new Dictionary<Container, int>();
        object obj = null;
        if ((obj = client.Modules.ScriptManager.Variables.GetValue(globalVariableName)) != null)
        {
            lootedContainers = (Dictionary<Container, int>)obj;
        }

        if (!client.Player.Connected) return;

        // designate first container as carrying
        var containers = client.Inventory.GetContainers().ToList();
        if (containers.Count == 0) return;
        var carryingContainer = containers[0];

        while (true)
        {
            bool allDone = true;

            // check child containers
            foreach (var container in client.Inventory.GetContainers())
            {
                if (!carryingContainer.IsOpen) return;
                if (container == carryingContainer) continue;
                if (!container.IsOpen) continue;

                bool done = true;

                // check if we've previously traversed this container
                int index = -1;
                if (!lootedContainers.TryGetValue(container, out index)) index = -1;

                // find child container and open it
                foreach (var parentItem in container.GetItems())
                {
                    if (!container.IsOpen) break;

                    // check if we previously traversed this container
                    if (index != -1 && index >= parentItem.Slot) continue;

                    // check if cap is consumed
                    bool noCap = false;
                    foreach (var l in loot)
                    {
                        if (client.Player.Cap > l.Weight) continue;
                        noCap = true;
                        break;
                    }
                    if (noCap)
                    {
                        client.Modules.ScriptManager.Variables.SetValue(globalVariableName, lootedContainers);
                        return;
                    }

                    if (!parentItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;

                    Container childContainer = null;
                    while (container.IsOpen)
                    {
                        childContainer = client.Inventory.GetFirstClosedContainer();
                        parentItem.OpenInNewWindow();
                        Thread.Sleep(800);
                        if (childContainer != null && childContainer.IsOpen) break;
                    }
                    if (childContainer == null || !childContainer.IsOpen || !container.IsOpen) continue;

                    // check for phat lewt
                    var items = childContainer.GetItems().ToList();
                    items.Reverse();
                    foreach (var item in items)
                    {
                        foreach (var l in loot)
                        {
                            if (l.ID != item.ID) continue;

                            ushort count = Math.Min(item.Count, (ushort)(client.Player.Cap / (ushort)Math.Ceiling(l.Weight)));
                            if (count == 0) break;

                            var best = carryingContainer.GetBestSlot(item);
                            if (best == null) break;

                            for (int i = 0; i < 3; i++)
                            {
                                item.Move(best, count);
                                if (item.WaitForInteraction(800)) break;
                            }

                            done = false;

                            break;
                        }
                    }

                    // check container and close it
                    if (childContainer.IsOpen)
                    {
                        // check if no loot left
                        bool found = false;
                        foreach (var item in childContainer.GetItems())
                        {
                            foreach (var l in loot)
                            {
                                if (l.ID != item.ID) continue;
                                found = true;
                                break;
                            }
                            if (found) break;
                        }
                        if (!found) lootedContainers[container] = parentItem.Slot;

                        childContainer.Close();
                        Thread.Sleep(500);
                    }

                    done = false;
                }

                if (done)
                {
                    container.Close();
                    Thread.Sleep(500);
                }
                else allDone = false;
            }

            if (allDone)
            {
                if (repeat)
                {
                    if (emptyContainerOffset == lootContainerOffset) break;
                    var tile = client.Map.GetTile(client.Player.Location.Offset(lootContainerOffset));
                    if (tile == null) break;
                    var topitem = tile.GetTopMoveItem();
                    if (!topitem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                    topitem.Move(client.Player.Location.Offset(emptyContainerOffset).ToItemLocation());
                    Thread.Sleep(1000);

                    tile = client.Map.GetTile(client.Player.Location.Offset(lootContainerOffset));
                    if (tile == null) break;
                    topitem = tile.GetTopUseItem(false);
                    if (!topitem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                    Container container = null;
                    for (int i = 0; i < 3; i++)
                    {
                        container = topitem.TryOpen();
                        if (container != null) break;
                    }
                    if (container == null) break;
                    lootedContainers.Clear();
                    continue;
                }
                break;
            }
        }

        client.Modules.ScriptManager.Variables.RemoveValue(globalVariableName);
    }
}
