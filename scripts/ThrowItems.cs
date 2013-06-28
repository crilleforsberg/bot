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
        ushort id = 0;

        while (true)
        {
            Thread.Sleep(2000);

            if (!client.Player.Connected) continue;

            Item item = client.Inventory.GetItem(id);
            if (item == null) continue;
            item.Move(new ItemLocation(client.Player.Location.Offset(-1, 1)));
        }
    }
}
