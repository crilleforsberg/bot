// IF YOU ARE USING AN OLD VERSION (I.E. IF YOU ARE NOT CRILLE OR ZACH)
// REPLACE Map.Tile WITH Tile

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
        ushort potID = 2562, potWeight = 55;
        byte range = 1, increasedRange = 2;
        bool hotkey = true, // whether this script will run as a hotkey
            pickup = false, // whether to try to pick up pots
            throwBack = true, // whether to throw them back at enemies
            increaseRangeWhenSingle = true;
        List<string> enemies = new List<string>()
        {
            "Towie Mcgregor",
            "Aeno",
            "Airo Insane",
            "Alvah",
            "Alvah Kazer",
            "Animator",
            "Astrid Insane",
            "Belian Void",
            "Bhaal",
            "Cater Insane",
            "Chris Mcgregor",
            "Ciziia",
            "Cobbler",
            "Command",
            "Cortmaz",
            "Crixus",
            "Daerkhil Dope",
            "Denied",
            "Dimas",
            "Emperor Potocky",
            "Empress Naomii",
            "Exodus",
            "Fatal Hussein",
            "Flop",
            "Ghazmolt",
            "Grandmaster Flash",
            "Guess Whooo",
            "Hell Bolt",
            "Heresy",
            "Hermione Mcgregor",
            "Hutchinson",
            "Kazimierz Wielki",
            "King Wojtek",
            "Maxi Inc",
            "Mhr Mcgregor",
            "Molecule",
            "Montgomery Burns",
            "Ody Dope",
            "Pandoraa",
            "Panzcho Mcgregor",
            "Puge Mcgregor",
            "Rafa Wielki",
            "Ralthuk",
            "Ricki Insane",
            "Rychu Wichura",
            "Sanzcho Mcgregor",
            "Sinna",
            "Taz Sho",
            "Trianos",
            "Varas",
            "Wez Zostaw Ryska",
            "William Mcgregor",
            "Ziomsie"
        };

        if (!client.Player.Connected) return;

        while (true)
        {
            var pLoc = client.Player.Location;
            var players = client.BattleList.GetPlayers().ToList();
            foreach (var p in players.ToArray())
            {
                if (!enemies.Contains(p.Name)) players.Remove(p);
            }
            List<Location> locsThrown = new List<Location>();
            foreach (var tile in client.Map.GetAdjacentTiles(pLoc).GetTiles())
            {
                var topitem = tile.GetTopMoveItem();
                if (topitem.ID != potID) continue;

                if (pickup && client.Player.Cap > potWeight)
                {
                    ItemLocation best = null;
                    foreach (var container in client.Inventory.GetContainers())
                    {
                        best = container.GetFirstEmptySlot();
                        if (best != null) break;
                    }
                    if (best != null)
                    {
                        topitem.Move(best);
                        continue;
                    }
                }

                var tilesOnScreen = client.Map.GetTilesOnScreen();

                if (throwBack)
                {
                    bool thrown = false;
                    foreach (var creature in players)
                    {
                        Location cLoc = creature.Location;
                        if (!pLoc.IsOnScreen(cLoc)) continue;

                        string name = creature.Name;
                        if (!enemies.Contains(name)) continue;

                        var adjTiles = tilesOnScreen.GetNearbyTiles(cLoc, (!increaseRangeWhenSingle || players.Count > 1) ? range : increasedRange).ToList();
                        adjTiles.Sort(delegate(Map.Tile first, Map.Tile second)
                        {
                            return cLoc.DistanceTo(first.WorldLocation).CompareTo(cLoc.DistanceTo(second.WorldLocation));
                        });
                        foreach (var adjTile in adjTiles)
                        {
                            if (pLoc.IsAdjacentTo(adjTile.WorldLocation)) continue;
                            if (locsThrown.Contains(adjTile.WorldLocation)) continue;
                            if (!adjTile.IsWalkable(false) || !pLoc.CanShootLocation(client, adjTile.WorldLocation, tilesOnScreen)) continue;

                            topitem.Move(adjTile.ToItemLocation());
                            locsThrown.Add(adjTile.WorldLocation);
                            thrown = true;
                            break;
                        }

                        if (thrown) break;
                    }
                    if (thrown) continue;
                }

                var tiles = new List<Map.Tile>(tilesOnScreen.GetTiles());
                foreach (var t in tiles.ToArray())
                {
                    if (!t.IsWalkable(false) || !pLoc.CanShootLocation(client, t.WorldLocation, tilesOnScreen)) tiles.Remove(t);
                }
                tiles.Sort(delegate(Map.Tile first, Map.Tile second)
                {
                    return pLoc.DistanceTo(second.WorldLocation).CompareTo(pLoc.DistanceTo(first.WorldLocation));
                });
                foreach (var t in tiles.ToArray())
                {
                    topitem.Move(t.ToItemLocation());
                    tiles.Remove(t);
                    break;
                }
            }

            if (hotkey) break;
            Thread.Sleep(500);
        }
    }
}