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
        // note:
        // I didn't test this script yet
        // but it compiles fine

        ushort spearID = 3277,
            minSpearCount = 2,
            spearWeight = 20;
		Random rand = new Random();
			
        while (true)
        {
            Thread.Sleep(rand.Next(1000, 1500));

            if (!client.Player.Connected) continue;
			
            Item handItem = client.Inventory.GetItemInSlot(Enums.EquipmentSlots.LeftHand);
            if (handItem == null ||
                (handItem.ID == spearID && handItem.Count <= minSpearCount))
            {
				ItemLocation itemLoc = handItem != null ?
					handItem.ToItemLocation() :
					new ItemLocation(Enums.EquipmentSlots.LeftHand);
                var tilesOnScreen = client.Map.GetTilesOnScreen();
                Tile playerTile = tilesOnScreen.GetTile(count: client.Player.ID);
                if (playerTile == null) continue;
                var adjacentTiles = tilesOnScreen.GetAdjacentTiles(playerTile);
                foreach (Tile t in adjacentTiles.GetTiles())
                {
                    if (!client.Player.Location.IsAdjacentTo(t.WorldLocation)) continue;
                    TileObject to = t.GetTopMoveItem(client);
                    if (to == null || to.Data != spearID) continue;
                    if (to.DataEx * spearWeight > client.Player.Cap) to.DataEx = (uint)(client.Player.Cap / (spearWeight - 1));
                    to.Move(client, itemLoc);
                    break;
                }
            }
        }
    }
}
