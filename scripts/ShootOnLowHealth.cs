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
        ushort exhaust = 2000;
		var names = new List<string>()
		{
			"Orc Leader", "Orc Warlord"
		};
        long time = Environment.TickCount;

        while (true)
        {
            Thread.Sleep(500);

            if (Environment.TickCount <= time + exhaust) continue;

            Creature creature = client.Player.TargetCreature;
            if (creature == null) continue;
            if (creature.HealthPercent > 10) continue;
			if (!names.Contains(creature.Name)) continue;

            Item rune = client.Inventory.GetItem(client.ItemList.Runes.HeavyMagicMissile);
            if (rune == null) continue;

            rune.UseOnBattleList(creature);
            time = Environment.TickCount;
        }
    }
}
