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
        List<string> whiteList = new List<string>()
        {
            "Raydents", "Mullen"
        };

        Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(700, 2000));
            if (!client.Player.Connected || client.Player.Target != 0) continue;

            foreach (Creature c in client.BattleList.GetPlayers())
            {
                if (whiteList.Contains(c.Name) || client.Player.Name == c.Name) continue;
                if (client.Player.SafeMode != Enums.SafeMode.Disabled) client.Player.SafeMode = Enums.SafeMode.Disabled;
                c.Attack();
                break;
            }
        }
    }
}