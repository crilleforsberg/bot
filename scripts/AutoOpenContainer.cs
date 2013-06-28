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
        while (!client.Player.Connected)
        {
            Thread.Sleep(500);
            continue;
        }
        Item item = client.Player.GetItemInSlot(Enums.EquipmentSlots.Ammo);
        if (item == null) return;
        item.Use();
    }
}