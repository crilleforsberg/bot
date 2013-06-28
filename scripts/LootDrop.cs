using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects
using KarelazisBot.Modules;

public class Test
{
    public static void Main(Client client)
    {
        if (!client.Player.Connected) return;

        Location loc = Location.Invalid;

        if (client.Modules.Cavebot.IsRunning &&
            client.Modules.Cavebot.GetWaypoints().ToArray().Length > 0 &&
            client.Modules.Cavebot.CurrentWaypointIndex >= 0)
        {
            var wp = client.Modules.Cavebot.GetWaypoints().ToArray()[client.Modules.Cavebot.CurrentWaypointIndex];
            if (wp == null) return;
            if (!client.Player.Location.IsOnScreen(wp.Location)) return;
            loc = wp.Location;
        }
        else loc = client.Player.Location;
        // you can use client.Player.Location.Offset(x, y, z) to get a relative position
        // like so: loc = client.Player.Location.Offset(1, -1, 0)
        // this would give the Location object the position 1 sqm northeast of the player

        if (!loc.IsValid() || !client.Player.Location.IsOnScreen(loc)) return;

        Thread.Sleep(500);

        List<ushort> itemsToDrop = new List<ushort>() { 2148, 2377 };
        Random rand = new Random();
        foreach (Container container in client.Inventory.GetContainers())
        {
            var items = container.GetItems().ToList<Item>();
            items.Reverse();
            foreach (Item item in items)
            {
                if (itemsToDrop.Contains(item.ID))
                {
                    item.Move(loc.ToItemLocation());
                    Thread.Sleep(rand.Next(250, 500));
                }
            }
        }
    }
}
