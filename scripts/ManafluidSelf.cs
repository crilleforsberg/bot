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
        Item vial = client.Inventory.GetItem(client.ItemList.Runes.Vial, (byte)Enums.FluidType.Mana);
        if (vial != null) vial.UseOnSelf();
    }
}
