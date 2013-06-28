using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        ushort parcelID = 2595;
        byte count = 3, range = 2;
        bool skipAdjacent = true;

        if (!client.Player.Connected || client.Player.Target == 0) return;

        var parcels = client.Inventory.GetItems(parcelID).ToList();
        if (parcels.Count == 0) return;

        var target = client.Player.TargetCreature;
        var playerLoc = client.Player.Location;
        var targetLoc = target.Location;
        if (!targetLoc.IsOnScreen(playerLoc)) return;

        var tiles = client.Map.GetNearbyTiles(targetLoc, range).GetTiles().ToList();
        if (skipAdjacent)
        {
            foreach (var t in tiles.ToArray())
            {
                if (t.WorldLocation.IsAdjacentTo(targetLoc)) tiles.Remove(t);
            }
        }
        var pDist = playerLoc.DistanceTo(targetLoc);
        // sort tiles so that the ones furthest away comes first
        tiles.Sort(delegate(Map.Tile first, Map.Tile second)
        {
            return playerLoc.DistanceTo(second.WorldLocation).CompareTo(playerLoc.DistanceTo(first.WorldLocation));
        });

        int index = parcels.Count - 1;
        foreach (var t in tiles)
        {
            if (index < 0) break;
            if (!t.IsWalkable(false)) continue;

            for (int i = 0; i < count; i++)
            {
                if (index < 0) break;

                var parcel = parcels[index];
                index--;
                parcel.Move(t.ToItemLocation());
            }
        }
    }
}