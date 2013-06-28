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
        // REMEMBER TO SET THESE
        List<string> whiteList = new List<string>()
        {
            "Mithaldo" , "Shadow"
        };

        bool logoutOnPlayerOnScreen = true,
             logoutOnLowRunes = false,
             logoutOnLowItems = false;
             //logoutOnPrivateMessage = false,
             //logoutOnDefaultMessage = false;

        ushort lowRunesCount = 0,
            lowRunesID = client.ItemList.Runes.Blank,
            lowItemsCount = 0,
            lowItemsID = client.ItemList.Food.Fish,
            minHealthPercent = 30,
            minPlayersCount = 1,
            minCreaturesCount = 1;

        long timeSinceMoved = Environment.TickCount; // snapshot of current time in milliseconds

        while (true)
        {
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;

            if (logoutOnPlayerOnScreen)
            {
                var players = client.BattleList.GetPlayers(true, true).ToList<Creature>();
                foreach (Creature c in players.ToArray())
                {
                    if (whiteList.Contains(c.Name) || c.Name == client.Player.Name) players.Remove(c);
                }

                bool found = false;
                foreach (Creature c in players)
                {
                    if (c.Name.StartsWith("GM ") || c.Name == "Iryont")
                    {
                        found = true;
                        break;
                    }
                }

                if (!found && logoutOnPlayerOnScreen && players.Count >= minPlayersCount)
                {
                    client.Packets.Logout();
                    return;
                }
            }

            if (logoutOnLowRunes)
            {
                var runes = client.Inventory.GetItems(lowRunesID).ToList<Item>();
                if (runes.Count <= lowItemsCount)
                {
                    client.Packets.Logout();
                    return;
                }
            }

            if (logoutOnLowItems)
            {
                var items = client.Inventory.GetItems(lowItemsID).ToList<Item>();
                if (items.Count <= lowItemsCount)
                {
                    client.Packets.Logout();
                    return;
                }
            }

            /*if (logoutOnPrivateMessage || logoutOnDefaultMessage)
            {
                foreach (GameWindowMessage msg in client.Window.GetGameWindowMessages())
                {   
                    if (msg.Type == (byte)Enums.SpeechType.Private &&
                        logoutOnPrivateMessage)
                    {
                        client.Packets.Logout();
                        break;
                    }
                    if (logoutOnDefaultMessage &&
                        (msg.Type == (byte)Enums.SpeechType.Say ||
                        msg.Type == (byte)Enums.SpeechType.Whisper ||
                        msg.Type == (byte)Enums.SpeechType.Yell) &&
                        !msg.JoinedMessage.StartsWith(client.Player.Name + " "))
                    {
                        client.Packets.Logout();
                        return;
                    }
                }
            }*/
        }
    }
}
