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
        Tile tile = client.Map.GetTile(client.Player.Location.Offset(x: -1));
        if (tile == null) return;
        TileObject topItem = tile.GetTopUseItem(false);
        if (topItem == null) return;
        topItem.Use();

        // use item west again

        int startTime = Environment.TickCount;
        while (startTime + 1000 > Environment.TickCount)
        {
            if (client.Player.Location == tile.WorldLocation) break;
            Thread.Sleep(200);
        }
        if (client.Player.Location != tile.WorldLocation) return;

        Tile tile = client.Map.GetTile(client.Player.Location.Offset(x: -1));
        if (tile == null) return;
        TileObject topItem = tile.GetTopUseItem(false);
        if (topItem == null) return;
        topItem.Use();
        Thread.Sleep(500);
    }
}
