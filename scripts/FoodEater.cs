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
        int intervalMin = 1000 * 20, // 20 seconds
			intervalMax = 1000 * 45; // 45 seconds
		Random rand = new Random();
        ushort brownMushroomId = 3725;
		
		while (true)
		{
			bool found = false;
			foreach (Container container in client.Inventory.GetContainers())
			{
				foreach (Item item in container.GetItems())
				{
					if (client.ItemList.Food.All.Contains(item.ID) ||
                        item.ID == brownMushroomId)
					{
						item.Use();
						found = true;
						break;
					}
				}
				if (found) break;
			}
			
			Thread.Sleep(rand.Next(intervalMin, intervalMax));
		}
    }
}
