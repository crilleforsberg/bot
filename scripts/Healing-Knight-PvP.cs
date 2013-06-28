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
            Thread.Sleep(10);
            if (client.Player.IsWalking) continue;

            Item rune = null;
            if (client.Player.HealthPercent <= 80 &&
                (rune = client.Inventory.GetItem(client.ItemList.Runes.UltimateHealing)) != null)
            {
                rune.UseOnSelf();
                Thread.Sleep(1000);
                continue;
            }
        }
    }
}
