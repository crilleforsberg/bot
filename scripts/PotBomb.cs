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
        ushort potID = 2562;
        byte range = 2;
        bool throwOnEnemies = true;
        List<string> enemies = new List<string>()
        {
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
            "Towie Mcgregor",
            "Trianos",
            "Varas",
            "Wez Zostaw Ryska",
            "William Mcgregor",
            "Ziomsie"
        };

        if (!client.Player.Connected) return;

        var pots = client.Inventory.GetItems(potID).ToList();
        if (pots.Count == 0) return;

        if (client.Player.Target != 0)
        {
            var target = client.Player.TargetCreature;
            var playerLoc = client.Player.Location;
            if (!target.Location.IsOnScreen(playerLoc)) return;

            var pTile = client.Map.GetPlayerTile();
            var tiles = client.Map.GetNearbyTiles(target.Location, 1);

            int index = pots.Count - 1;
            foreach (var t in tiles.GetTiles())
            {
                if (index < 0) break;

                var pot = pots[index];
                index--;
                pot.Move(t.ToItemLocation());
            }
        }
        if (throwOnEnemies)
        {
            var tiles = client.Map.GetTilesOnScreen();
            if (tiles == null) return;
            var pTile = tiles.GetTile(count: client.Player.ID);
            if (pTile == null) return;

            List<Location> thrownLocs = new List<Location>();
            int index = pots.Count - 1;
            foreach (var creature in client.BattleList.GetPlayers(true, true))
            {
                if (index < 0) return;

                var cLoc = creature.Location;
                if (!pTile.WorldLocation.IsOnScreen(cLoc)) continue;
                if (!enemies.Contains(creature.Name)) continue;

                foreach (var t in tiles.GetAdjacentTiles(cLoc))
                {
                    if (index < 0) return;

                    if (!t.IsWalkable(false) || !pTile.WorldLocation.CanShootLocation(client, t.WorldLocation, tiles)) continue;
                    if (thrownLocs.Contains(t.WorldLocation)) continue;

                    var pot = pots[index];
                    index--;
                    pot.Move(t.ToItemLocation());
                    thrownLocs.Add(t.WorldLocation);
                }
            }
        }
    }
}