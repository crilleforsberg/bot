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
        string spellName = "utevo gran lux";
        ushort spellMana = 60;

        while (true)
        {
            Thread.Sleep(3000);

            if (client.Player.Z <= 7 || client.Player.Light > 5 || client.Player.Mana < spellMana) continue;

            client.Packets.Say(spellName);
        }
    }
}
