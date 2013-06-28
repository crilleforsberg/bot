using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    class DepotItem
    {
        public DepotItem(ushort itemID, ushort containerID = 0, byte depth = 0)
        {
            this.ItemID = itemID;
            this.ContainerID = containerID;
            this.Depth = depth;
        }

        /// <summary>
        /// The ID of the item.
        /// </summary>
        public ushort ItemID;
        /// <summary>
        /// The ID of the container to deposit items to.
        /// If 0, will deposit to the locker.
        /// </summary>
        public ushort ContainerID;
        /// <summary>
        /// How far to traverse before depositing items.
        /// If ContainerID is 0, the first container found will be used.
        /// </summary>
        public byte Depth;
    }

    public static void Main(Client client)
    {
        List<ushort> depotLockers = new List<ushort>()
        {
            2589, 2590, 2591, 2592
            //7.6+ 3497, 3498, 3499, 3500
        };
        List<DepotItem> itemsToDeposit = new List<DepotItem>()
        {
			new DepotItem(2599, 2594)
            //new DepotItem(2004, 2594) // yellow (ankrahmun) bps to depot chest
        };
        List<DepotItem> itemsToTake = new List<DepotItem>()
        {
            new DepotItem(2599, 2594, 1)
            //new DepotItem(2004, 2004, 1) // yellow bps from a yellow bp inside locker
        };

        Location currentLockerLocation = Location.Invalid;
        TileCollection tilesOnScreen = null;
        Tile playerTile = null;

        // find and reach depot locker
        while (true)
        {
            Thread.Sleep(500);

            if (client.Player.IsWalking) continue;
            if (currentLockerLocation.IsValid() && client.Player.Location == currentLockerLocation) break;

            tilesOnScreen = client.Map.GetTilesOnScreen();
            var lockerTiles = tilesOnScreen.GetTileCollectionWithObjects(depotLockers);
            if (lockerTiles.IsEmpty()) break;

            playerTile = tilesOnScreen.GetPlayerTile();
            if (playerTile == null) break;

            var tiles = lockerTiles.GetTiles().ToList();
            tiles.Sort(delegate(Tile first, Tile second)
            {
                return playerTile.WorldLocation.DistanceTo(first.WorldLocation).CompareTo(
                    playerTile.WorldLocation.DistanceTo(second.WorldLocation));
            });
            foreach (Tile t in tiles)
            {
                TileObject topItem = t.GetTopUseItem(false);
                if (!depotLockers.Contains(topItem.ID)) continue;

                Tile closestTile = tilesOnScreen.GetClosestNearbyTile(playerTile, t);
                if (closestTile == null) continue;

                currentLockerLocation = closestTile.WorldLocation;
                client.Player.GoTo = currentLockerLocation;
                break;
            }
        }

        if (!currentLockerLocation.IsValid() || client.Player.Location != currentLockerLocation) return;

        // open depot locker
        tilesOnScreen = client.Map.GetTilesOnScreen();
        playerTile = tilesOnScreen.GetPlayerTile();
        if (playerTile == null) return;
        foreach (Tile adjacentTile in tilesOnScreen.GetAdjacentTiles(playerTile))
        {
            Container depotContainer = client.Inventory.GetFirstClosedContainer();
            for (int i = 0; i < 5; i++)
            {
                adjacentTile.UpdateObjects();
                TileObject topItem = adjacentTile.GetTopUseItem(false);
                if (!depotLockers.Contains(topItem.ID)) break;

                topItem.Use();
                Thread.Sleep(500);
                if (!depotContainer.IsOpen) continue;

                break;
            }

            if (!depotContainer.IsOpen) continue;

            // locker is open, run deposit logic
            // sort by depth
            itemsToDeposit.Sort(delegate(DepotItem first, DepotItem second)
            {
                return first.Depth.CompareTo(second.Depth);
            });

            DepotItem last = null, current = null;
            for (int i = 0; i < itemsToDeposit.Count; i++)
            {
                current = itemsToDeposit[i];

                // get back to root container if necessary
                if (last == null || current.ContainerID != last.ContainerID || current.Depth != last.Depth)
                {
                    while (depotContainer.HasParent)
                    {
                        depotContainer.OpenParentContainer();
                        Thread.Sleep(500);
                    }
                }

                // open containers if necessary
                byte depth = 0;
                while (current.Depth > depth && depotContainer.IsOpen)
                {
                    Item subContainer = null;
                    if (current.ContainerID != 0)
                    {
                        subContainer = depotContainer.GetItem(current.ContainerID);
                        if (subContainer == null) break;
                        if (!subContainer.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                    }
                    else
                    {
                        foreach (Item item in depotContainer.GetItems())
                        {
                            if (!item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;
                            subContainer = item;
                            break;
                        }
                    }
                    if (subContainer == null) break;

                    subContainer.Use();
                    Thread.Sleep(1000);
                    depth++;
                }
                // check if we reached the depth
                // if not, skip this item
                if (current.Depth != depth) continue;

                // move items to depot
                foreach (Item item in client.Inventory.GetItems())
                {
                    if (!depotContainer.IsOpen) break;

                    if (current.ItemID != item.ID) continue;

                    if (current.ContainerID == 0) // no container specified, put in current container
                    {
                        if (depotContainer.IsFull) break;
                        ItemLocation slot = depotContainer.GetFirstEmptySlot();
                        if (slot == null) break;
                        item.Move(slot);
                        item.WaitForInteraction(500);
                        break;
                    }

                    // put item inside container
                    Item subContainer = depotContainer.GetItem(current.ContainerID);
                    if (subContainer == null) break;
                    item.Move(subContainer.ToItemLocation());
                    item.WaitForInteraction(500);
                    break;
                }

                last = current;
            }

            // deposit logic done, run withdraw logic
            if (itemsToTake.Count == 0 || !depotContainer.IsOpen) return;
            last = null;
            for (int i = 0; i < itemsToTake.Count; i++)
            {
                current = itemsToTake[i];

                // get back to root container if necessary
                if (last == null || current.ContainerID != last.ContainerID || current.Depth != last.Depth)
                {
                    while (depotContainer.HasParent)
                    {
                        depotContainer.OpenParentContainer();
                        Thread.Sleep(500);
                    }
                }

                // open containers if necessary
                byte depth = 0;
                while (current.Depth > depth && depotContainer.IsOpen)
                {
                    Item subContainer = null;
                    if (current.ContainerID != 0)
                    {
                        subContainer = depotContainer.GetItem(current.ContainerID);
                        if (subContainer == null) break;
                        if (!subContainer.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                    }
                    else
                    {
                        foreach (Item item in depotContainer.GetItems())
                        {
                            if (!item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;
                            subContainer = item;
                            break;
                        }
                    }
                    if (subContainer == null) break;

                    subContainer.Use();
                    Thread.Sleep(1000);
                    depth++;
                }
                // check if we reached the depth
                // if not, skip this item
                if (current.Depth != depth) continue;

                // move items to the player's inventory
                foreach (Item item in depotContainer.GetItems())
                {
                    if (!depotContainer.IsOpen) break;

                    if (current.ItemID != item.ID) continue;

                    ItemLocation slot = client.Inventory.GetFirstSuitableSlot(item);
                    if (slot == null) break;
                    item.Move(slot);
                    item.WaitForInteraction(500);
                    break;
                }
            }

            if (depotContainer != null && depotContainer.IsOpen) depotContainer.Close();

            return;
        }
    }
}