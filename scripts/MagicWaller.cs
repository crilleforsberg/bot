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
        while (true)
        {
            Thread.Sleep(500);

            var players = client.BattleList.GetPlayers().ToList();

            if (players.Count > 1)
            {
                Location loc = client.Player.Location.Offset(-4, 0, 0);
                Item mwall = client.Inventory.GetItem(client.ItemList.Runes.MagicWall);
                if (mwall == null) continue;
                mwall.UseOnLocation(loc);
                Thread.Sleep(1000 * 16);

                loc = client.Player.Location.Offset(-3, 0, 0);
                mwall = client.Inventory.GetItem(client.ItemList.Runes.MagicWall);
                if (mwall == null) continue;
                mwall.UseOnLocation(loc);
                Thread.Sleep(1000 * 16);

                loc = client.Player.Location.Offset(-2, 0, 0);
                mwall = client.Inventory.GetItem(client.ItemList.Runes.MagicWall);
                if (mwall == null) continue;
                mwall.UseOnLocation(loc);
                Thread.Sleep(1000 * 17);
            }
        }
    }
}
