using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    class Criteria
    {
        public string Name;
        public byte MinCount;
        public ushort Count;

        public Criteria(string name, byte minCount)
        {
            this.Name = name;
            this.MinCount = minCount;
        }
    }

    public static void Main(Client client)
    {
        ushort runeID = client.ItemList.Runes.Fireball;
        List<Criteria> criterias = new List<Criteria>()
        {
            new Criteria("Dwarf Guard", 3)
        };

        int exhaust = 0;
        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;
            if (Environment.TickCount < exhaust + 2000) continue;

            var creatures = client.BattleList.GetCreatures(true, true).ToList();
            var playerLocation = client.Player.Location;

            foreach (Criteria criteria in criterias) criteria.Count = 0;

            foreach (var c in creatures)
            {
                if (!c.Location.IsAdjacentTo(playerLocation)) continue;

                string name = c.Name;
                foreach (Criteria criteria in criterias)
                {
                    if (criteria.Name != name) continue;
                    criteria.Count++;
                    break;
                }
            }

            foreach (Criteria criteria in criterias)
            {
                if (criteria.MinCount > criteria.Count) continue;

                Item rune = client.Inventory.GetItem(runeID);
                if (rune == null) break;
                rune.UseOnLocation(playerLocation);
                exhaust = Environment.TickCount;
                break;
            }
        }
    }
}
