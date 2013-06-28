using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        Random rand = new Random();
        List<string> names = new List<string>()
        {
            "Dragon", "Dragon Lord"
        };

        while (true)
        {
            Thread.Sleep(rand.Next(700, 2000));
            if (!client.Player.Connected || client.Player.Target != 0) continue;

            foreach (Creature c in client.BattleList.GetCreatures())
            {
                if (!names.Contains(c.Name)) continue;
                c.Attack();
                break;
            }
        }
    }
}