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
        string spellWords = "exevo con";
        ushort minCap = 20, spellMana = 100;

        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;

            if (client.Player.ManaPercent > 80 &&
                client.Player.Mana > spellMana &&
                client.Player.Cap > minCap)
            {
                client.Packets.Say(spellWords);
                Thread.Sleep(1000);
                client.Inventory.GroupItems();
            }
        }
    }
}