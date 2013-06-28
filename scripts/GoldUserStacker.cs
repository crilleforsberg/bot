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
        ushort goldID = 3031;
		
		while (true)
		{
            Thread.Sleep(1000);
            if (client.Player.IsWalking) continue;

            Item gold = client.Inventory.GetItem(goldID, 100);
            if (gold != null)
            {
                gold.Use();
                Thread.Sleep(1000);
                client.Inventory.GroupItems();
            }
		}
    }
}
