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
        ushort manaRequired = 200,
            healthPercent = 2;

        while (true)
        {
            Thread.Sleep(100);
            if (client.Player.Mana < manaRequired || client.Player.Target == 0) continue;

            Creature c = client.Player.TargetCreature;
            if (c == null || !client.Player.Location.IsAdjacentTo(c.Location)) continue;
            if (c.HealthPercent <= healthPercent) client.Packets.Say("exori"); 
        }
    }
}
