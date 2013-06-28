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
        Random rand = new Random();
		ushort[] waterIDs = new ushort[] { 4597, 4598, 4599, 4600, 4601, 4602, 4609, 4610, 4611, 4612 };
		
        while (true)
        {
            Thread.Sleep(rand.Next(600, 1200));
			
			// sanity checks
            if (!client.Player.Connected) continue;
            if (client.Player.Cap <= 7) continue;
			
			// get fishing rod
            Item fishingRod = client.Inventory.GetItem(client.ItemList.Tools.FishingRod);
            if (fishingRod == null) continue;
			
			// get fishy tiles
			var fishyTiles = client.Map.GetTilesWithObject(490).GetTiles().ToList(); // 7.4
            //var fishyTiles = client.Map.GetTilesWithObjects(waterIDs).GetTiles().ToList();
			// you can also use i.e. client.Map.GetTilesWithObjects(new ushort[] { 123, 124, 155 });
			// if there are more than one fishy tile
            if (fishyTiles.Count == 0) continue;
			
			// get random fisgy tile
            var playerLoc = client.Player.Location;
            Map.Tile tile = null;
            for (int i = 0; i < fishyTiles.Count; i++)
            {
                tile = fishyTiles[rand.Next(fishyTiles.Count)];
                if (!playerLoc.IsOnScreen(tile.WorldLocation) || tile.GetObjects().ToList().Count > 1) continue;
				break;
            }
            if (tile == null) continue;
			
			// use fishing rod on fishy tile
            fishingRod.UseOnLocation(tile.WorldLocation);
            Thread.Sleep(rand.Next(250, 500));
            client.Inventory.GroupItems();
			Thread.Sleep(500);
        }
    }
}