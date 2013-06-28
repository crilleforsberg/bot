using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects
using System.Threading;

public class Test
{
    public static void Main(Client client)
    {
        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            if (client.Player.Mana < 25 || !client.Player.HasFlag(Player.Flags.Paralyzed)) continue;
            client.Packets.Say("exura");
        }
    }
}