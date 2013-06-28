using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Linq; // necessary to cast IEnumerables to i.e. a list or array
using System.Threading; // used for putting the thread to sleep
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    public static void Main(Client client)
    {
        if (client.Player.Target == 0)
        {
            System.Windows.Forms.MessageBox.Show("You need to attack the mother slime before you run this script");
            return;
        }
        uint motherID = client.Player.Target;
        client.Packets.Attack(0);
		
		Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(5000, 10000));

            if (client.Player.Target != 0) continue;
            Location playerLoc = client.Player.Location;
            foreach (Creature c in client.BattleList.GetCreatures(true, true))
            {
                if (c.ID != motherID && c.Location.DistanceTo(playerLoc) < 2)
                {
                    c.Attack();
                    break;
                }
            }
        }
    }
}