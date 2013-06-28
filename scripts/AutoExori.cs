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
        int minMonsterCount = 3;

        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected || client.Player.Mana < client.Player.Level * 4)
            {
                continue;
            }

            int count = 0;
            Location playerLocation = client.Player.Location;
            foreach (Creature c in client.BattleList.GetCreatures(true, true))
            {
                if (c.Location.IsAdjacentTo(playerLocation)) count++;
            }
            if (count >= minMonsterCount) client.Packets.Say("exori");
        }
    }
}