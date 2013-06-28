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
        string name = "Monk";
        byte lowHealth = 15;

		while (true)
		{
            Thread.Sleep(500);

            Location playerLoc = client.Player.Location;
            uint targetID = client.Player.Target;

            if (targetID == 0)
            {
                foreach (Creature c in client.BattleList.GetAll(true, true))
                {
                    if (c.Name != name) continue;
                    if (!playerLoc.IsAdjacentTo(c.Location)) continue;
                    c.Attack();
                    break;
                }
            }
            else
            {
                foreach (Creature c in client.BattleList.GetAll(true, true))
                {
                    if (c.ID == targetID)
                    {
                        if (c.HealthPercent > lowHealth) continue;
                        c.Attack();
                        break;
                    }
                }
            }
		}
    }
}
