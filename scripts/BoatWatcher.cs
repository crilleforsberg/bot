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
        string recipient = "";
        ushort messageInterval = 1500,
            minimumTimeBetweenSets = 5000;
		
		while (true)
		{
            Thread.Sleep(500);

            if (string.IsNullOrEmpty(recipient) || !client.Player.Connected) continue;

            var players = new List<Creature>();
            Location playerLoc = client.Player.Location;
            uint playerID = client.Player.ID;
            foreach (Creature c in client.BattleList.GetPlayers(true, true))
            {
                if (playerLoc.IsOnScreen(c.Location) && playerID != c.ID) players.Add(c);
            }
            if (players.Count == 0) continue;
            int currentChar = 1;
            foreach (Creature p in players)
            {
                client.Packets.PrivateMessage(recipient, "(" + currentChar + "/" + players.Count + ") " +
                    p.Name + " - Range: " + Math.Round(client.Player.Location.DistanceTo(p.Location), 2) + " - " +
                    p.HealthPercent + "% HP - Skull: " + p.Skull + " - Has attacked me: " + p.HasAttackedMe);
                currentChar++;
                Thread.Sleep(messageInterval);
            }
            Thread.Sleep(minimumTimeBetweenSets);
		}
    }
}
