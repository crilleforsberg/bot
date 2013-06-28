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
        client.ItemList.Food.Ham = 3582;
        client.ItemList.Food.Fish = 3578;
        client.ItemList.Food.Meat = 3577;
        client.ItemList.Food.DragonHam = 3583;
		client.ItemList.Food.Cheese = 3607;

        client.ItemList.Tools.Rope = 3003;
        client.ItemList.Tools.Shovel = 3457;
		client.ItemList.Tools.FishingRod = 3483;

        client.ItemList.Runes.UltimateHealing = 3160;
		client.ItemList.Runes.Blank = 3147;
        client.ItemList.Runes.Vial = 2874;

        client.ItemList.Rings.Life = 3052;
    }
}
