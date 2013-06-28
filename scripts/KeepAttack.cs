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
        uint target = 0;
        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;

            uint currentTarget = client.Player.Target;
            if (currentTarget == 0 && target == 0) continue;
            else if (currentTarget > 0 && target != currentTarget)
            {
                target = currentTarget;
                continue;
            }
            else if (currentTarget == 0 && target != 0)
            {
                Creature c = client.BattleList.GetAny(target);
                if (c == null || !c.IsVisible || !c.Location.IsOnScreen(client.Player.Location)) continue;
                c.Attack();
            }
        }
    }
}