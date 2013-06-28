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
        byte equipManaPercent = 35,
            unequipManaPercent = 70,
            lifeRingContainer = 0;
        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;

            if (client.Player.ManaPercent <= equipManaPercent)
            {
                while (client.Inventory.GetItemInSlot(Enums.EquipmentSlots.Ring) == null)
                {
                    Item ring = client.Inventory.GetItem(client.ItemList.Rings.Life);
                    if (ring == null) break;
                    ring.Move(new ItemLocation(Enums.EquipmentSlots.Ring));
                    lifeRingContainer = ring.ContainerNumber;
                    Thread.Sleep(1000);
                }
            }
            else if (client.Player.ManaPercent >= unequipManaPercent)
            {
                Item ring = null;
                while ((ring = client.Inventory.GetItemInSlot(Enums.EquipmentSlots.Ring)) != null)
                {
                    Container c = client.Inventory.GetContainer(lifeRingContainer);

                    ItemLocation bestLocation = null;
                    if (!c.IsOpen || c.IsFull) bestLocation = client.Inventory.GetFirstSuitableSlot(ring);
                    else bestLocation = c.GetFirstEmptySlot();
                    
                    if (bestLocation == null) break;
                    ring.Move(bestLocation);
                    Thread.Sleep(500);
                }
            }
        }
    }
}
