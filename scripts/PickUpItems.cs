using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        Location offset = new Location(1, 0, 0), // fixed offset of loot pile
            finishedContainerOffset = new Location(0, 0, 0); // fixed offset of finished container
        bool useOffset = false, // whether to only check on a location specified by offset
            useNestedContainers = true, // whether to dump into nested containers
            openContainersOnGround = true; // whether to use containers on the ground as loot containers

        Container lootContainer = null,
            finishedContainer = null;
        Location lootContainerLocation = null;
        int fullIndex = int.MaxValue;
        string statusBarFull = "You cannot put more objects";
        while (true)
        {
            Thread.Sleep(50);
            if (!client.Player.Connected) continue;

            if (openContainersOnGround)
            {
                if (lootContainerLocation != null && lootContainer != null && lootContainer.IsOpen &&
                    finishedContainer != null && finishedContainer.IsOpen)
                {
                    if (fullIndex <= 0)
                    {
                        lootContainer.Close();
                        fullIndex = int.MaxValue;
                        var top = client.Map.GetTopMoveItem(lootContainerLocation);
                        if (top != null && top.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                        {
                            var best = finishedContainer.GetBestSlot(top.ToItemLocation());
                            if (best != null)
                            {
                                top.Move(best);
                                Thread.Sleep(500);
                            }
                        }
                        lootContainer = null;
                        lootContainerLocation = null;
                    }
                }

                // find loot container
                if (lootContainer == null || !lootContainer.IsOpen)
                {
                    lootContainer = null;
                    foreach (var tile in client.Map.GetAdjacentTiles(client.Player.Location).GetTiles())
                    {
                        if (tile.WorldLocation == client.Player.Location.Offset(finishedContainerOffset)) continue;

                        var top = tile.GetTopUseItem(false);
                        if (top == null || !top.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;
                        for (int i = 0; i < 3; i++)
                        {
                            lootContainer = top.TryOpen();
                            if (lootContainer != null) break;
                        }
                        if (lootContainer == null) continue;

                        lootContainerLocation = tile.WorldLocation;
                        break;
                    }
                    // check if container wasn't found
                    // exit script if so
                    if (lootContainer == null) return;
                }

                // find finished container
                if (finishedContainer == null || !finishedContainer.IsOpen)
                {
                    finishedContainer = null;
                    var top = client.Map.GetTopUseItem(client.Player.Location.Offset(finishedContainerOffset), false);
                    if (top == null || !top.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) return;
                    for (int i = 0; i < 3; i++)
                    {
                        finishedContainer = top.TryOpen();
                        if (finishedContainer != null) break;
                    }
                    if (finishedContainer == null) return;
                }
            }
            else
            {
                // assign loot container to any open container that has free slots
                foreach (var container in client.Inventory.GetContainers())
                {
                    if (container.IsFull) continue;
                    lootContainer = container;
                    break;
                }
                // check if container wasn't found
                // wait and try again if so
                if (lootContainer == null) continue;
            }

            Map.TileObject topitem = null;
            if (!useOffset)
            {
                // find loot pile
                foreach (var tile in client.Map.GetAdjacentTiles(client.Player.Location).GetTiles())
                {
                    if (openContainersOnGround)
                    {
                        var loc = tile.WorldLocation;
                        if (loc == client.Player.Location.Offset(finishedContainerOffset) || loc == lootContainerLocation) continue;
                    }

                    var top = tile.GetTopMoveItem();
                    if (top == null ||
                        !top.HasFlag(Enums.ObjectPropertiesFlags.IsPickupable) ||
                        top.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        continue;
                    }
                    topitem = top;
                    break;
                }
            }
            else
            {
                var tile = client.Map.GetTile(client.Player.Location.Offset(offset));
                if (tile == null) continue;
                if (tile.GetObjects().ToArray().Length <= 1) break; // tile is empty

                var top = tile.GetTopMoveItem();
                if (topitem == null || topitem.ID < 100) return;
                if (!topitem.HasFlag(Enums.ObjectPropertiesFlags.IsPickupable)) return;
                topitem = top;
            }
            if (topitem == null) return;

            if (!useNestedContainers)
            {
                ItemLocation itemLoc = lootContainer.GetBestSlot(topitem.ToItemLocation());
                if (itemLoc == null) continue;
                topitem.Move(itemLoc);
            }
            else
            {
                var items = lootContainer.GetItems().ToArray();
                int i = Math.Min(fullIndex, items.Length - 1);
                while (i >= 0)
                {
                    if (!lootContainer.IsOpen) break;

                    var item = items[i];
                    if (fullIndex <= i || !item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        i--;
                        continue;
                    }

                    topitem.Move(item.ToItemLocation());
                    //Thread.Sleep(200);
                    if (client.Window.StatusBar.GetText().StartsWith(statusBarFull))
                    {
                        fullIndex = i;
                        client.Window.StatusBar.SetText(string.Empty);
                        Thread.Sleep(1000);
                        client.Window.StatusBar.SetText(string.Empty);
                    }
                    break;
                }
            }
        }
    }
}
