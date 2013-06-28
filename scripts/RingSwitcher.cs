using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        ushort ringID = client.ItemList.Rings.Axe;
        Container ringContainer = null;

        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            Item currentRing = client.Inventory.GetItemInSlot(Enums.EquipmentSlots.Ring);
            var creatures = client.BattleList.GetCreatures(true, true).ToList();
            var playerLocation = client.Player.Location;

            if (currentRing != null)
            {
                bool found = false;
                foreach (Creature c in creatures)
                {
                    if (!c.Location.IsAdjacentTo(playerLocation)) continue;
                    found = true;
                    break;
                }
                if (found) continue;

                ItemLocation bestSlot = null;

                if (ringContainer != null) bestSlot = ringContainer.GetFirstEmptySlot();
                if (bestSlot == null) bestSlot = client.Inventory.GetFirstSuitableSlot(currentRing);

                if (bestSlot != null) currentRing.Move(bestSlot);
            }
            else
            {
                bool found = false;
                foreach (Creature c in creatures)
                {
                    if (!c.Location.IsAdjacentTo(playerLocation)) continue;
                    found = true;
                    break;
                }
                if (!found) continue;

                Item ringToEquip = client.Inventory.GetItem(ringID);
                if (ringToEquip == null) continue; // ring not found
                ringToEquip.Move(new ItemLocation(Enums.EquipmentSlots.Ring));
            }
        }
    }
}
