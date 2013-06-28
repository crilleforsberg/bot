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
        ushort runeID = client.ItemList.Runes.UltimateHealing;
        Item rune = client.Inventory.GetItem(runeID);
        if (rune != null) rune.UseOnSelf();
    }
}
