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
        long timeOfCast = 0;
        while (true)
        {
            Thread.Sleep(500);

            var creatures = client.BattleList.GetCreatures(true, true);
            if (creatures.Count > 0 &&
                TimeSpan.FromMilliseconds(Environment.TickCount - timeOfCast).TotalSeconds >= 200)
            {
                client.Packets.Say("utana vid");
                timeOfCast = Environment.TickCount;
            }
        }
    }
}
