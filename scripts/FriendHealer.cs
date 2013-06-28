using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

// this can be named whatever, basically only useful if you're writing libraries
// you can also use namespaces!
public class Test
{
    // entry point of all scripts must be called Main
    // libriares does not require an entry point
    public static void Main(Client client)
    {
        List<string> friends = new List<string>() { "Farcanoks", "Yuko Von Stein", "Mullen" };
        bool useSio = false;
        while (true)
        {
            Thread.Sleep(100);

            if (client.Player.HealthPercent <= 70) continue;

            List<Creature> players = client.BattleList.GetPlayers(true, true).ToList<Creature>(),
                           friendsOnScreen = new List<Creature>();
            Location playerLoc = client.Player.Location;
            foreach (Creature p in players)
            {
                if (!playerLoc.IsOnScreen(p.Location) || !friends.Contains(p.Name)) continue;
                friendsOnScreen.Add(p);
            }
            foreach (Creature p in friendsOnScreen)
            {
                if (p.HPPercent > 65) continue;

                if (useSio && client.Player.Mana >= 100) client.Packets.Say("exura sio \"" + p.Name);
                else
                {
                    Item rune = client.Inventory.GetItem(client.ItemList.Runes.UltimateHealing);
                    if (rune != null) rune.UseOnCreature(p);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
