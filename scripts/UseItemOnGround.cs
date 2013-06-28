using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        Item item = client.Inventory.GetItem(2416);
        if (item == null) return;
        item.UseOnLocation(client.Player.Location.Offset(y: -1));
    }
}
