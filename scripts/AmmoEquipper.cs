using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        ushort id = 3447;

        Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(100 * 2, 100 * 8));

            Item current = client.Inventory.GetItemInSlot(Enums.EquipmentSlots.Ammo);
            if (current == null) continue; // sanity check
            if (current.ID != 0 && current.Count > rand.Next(20, 40)) continue;
            
            Item itemToEquip = client.Inventory.GetItem(id);
            if (itemToEquip == null) continue; // item not found
            itemToEquip.Move(current.ToItemLocation());
        }
    }
}
