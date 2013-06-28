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
        ushort ringID = client.ItemList.Rings.Life;

        Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(1000 * 2, 1000 * 5));

            Item currentRing = client.Player.GetItemInSlot(Enums.EquipmentSlots.Ring);
            if (currentRing != null && currentRing.ID != 0) continue; // sanity check + currently using ring
			
			Item ringToEquip = client.Inventory.GetItem(ringID);
            if (ringToEquip == null) continue; // ring not found
            ringToEquip.Move(new ItemLocation(Enums.EquipmentSlots.Ring));
        }
    }
}
