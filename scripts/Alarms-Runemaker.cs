using System;
using System.Collections.Generic;
using KarelazisBot;
using KarelazisBot.Objects;
using System.Threading;
using System.Linq;

public class Test
{
    // entry point of all scripts must be called Main
    // libriares does not require an entry point
    public static void Main(Client client)
    {
        // REMEMBER TO SET THESE
        List<string> whiteList = new List<string>()
        {
            "Raydents", "Mullen", "Yallar", "Nerevarine", "Dick Dastardly"
        },
        blackList = new List<string>()
        {
            "Demon Skeleton"
        };

        bool alarmOnPlayerOnScreen = false,
             alarmOnCreatureOnScreen = false,
             alarmOnGamemaster = true,
             alarmOnPlayerKiller = true,
             alarmOnLowRunes = false,
             alarmOnLowHealth = true,
             alarmOnLowItems = false,
             alarmOnStuck = false,
             alarmOnPrivateMessage = true,
             alarmOnDefaultMessage = true,
             alarmOnDisconnect = true,
             doFlashWindow = true;

        ushort lowRunesCount = 10,
            lowRunesID = client.ItemList.Runes.UltimateHealing,
            lowItemsCount = 5,
            lowItemsID = client.ItemList.Rings.Life,
            minHealthPercent = 30,
            minPlayersCount = 1,
            minCreaturesCount = 1;

        Location oldLocation = client.Player.Connected ? client.Player.Location : new Location();
        long timeSinceMoved = Environment.TickCount, // snapshot of current time in milliseconds
             minTimeSinceMoved = 1000 * 15; // 15 seconds
		
		int oldMsgIndex = 0;
        while (true)
        {
            Thread.Sleep(500);

            if (alarmOnDisconnect && !client.Player.Connected)
            {
                client.Modules.TextToSpeech.Speak("disconnected", doFlashWindow);
            }

            if (alarmOnLowHealth && client.Player.Connected &&
                client.Player.HealthPercent <= minHealthPercent)
            {
                client.Modules.TextToSpeech.Speak("low health", doFlashWindow);
            }

            if (alarmOnPlayerOnScreen || alarmOnGamemaster || alarmOnPlayerKiller)
            {
                var players = client.BattleList.GetPlayers(true, true).ToList<Creature>();
                foreach (Creature c in players.ToArray())
                {
                    if (whiteList.Contains(c.Name) || client.Player.Name == c.Name) players.Remove(c);
                }

                if (alarmOnGamemaster)
                {
                    bool found = false;
                    foreach (var msg in client.Window.GameWindow.GetMessages())
                    {
                        if (msg.Sender.StartsWith("GM ") ||
                            msg.Sender.StartsWith("Iryont") ||
                            msg.Sender.StartsWith("Gamemaster "))
                        {
                            found = true;
                            break;
                        }
                    }
                    foreach (Creature player in players)
                    {
                        if (player.Name.StartsWith("GM ") ||
                            player.Name.StartsWith("Iryont") ||
                            player.Name.StartsWith("Gamemaster "))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) client.Modules.TextToSpeech.Speak("gamemaster", doFlashWindow);
                }

                if (alarmOnPlayerOnScreen && players.Count >= minPlayersCount)
                {
                    // say players if > 1, say player if == 1
                    client.Modules.TextToSpeech.Speak(players.Count +
                        (players.Count > 1 ? " players on screen" : "player on screen"), doFlashWindow);
                }

                if (alarmOnPlayerKiller)
                {
                    uint playerID = client.Player.ID;
                    foreach (Creature player in players)
                    {
                        if (player.ID == playerID) continue;

                        if (player.HasAttackedMeRecently((ushort)(1000 * 5)))
                        {
                            client.Modules.TextToSpeech.Speak("player killer", doFlashWindow);
                            break;
                        }
                    }
                }
            }

            if (alarmOnCreatureOnScreen || blackList.Count > 0)
            {
                var creatures = client.BattleList.GetCreatures(true, true).ToList<Creature>();

                if (alarmOnCreatureOnScreen && creatures.Count > minCreaturesCount)
                {
                    client.Modules.TextToSpeech.Speak("creature" + (creatures.Count > 1 ? "s" : "") + " on screen", doFlashWindow);
                }
                else if (blackList.Count > 0)
                {
                    foreach (var c in creatures)
                    {
                        if (!blackList.Contains(c.Name)) continue;

                        client.Modules.TextToSpeech.Speak(c.Name + " on screen", doFlashWindow);
                        break;
                    }
                }
            }

            if (alarmOnLowRunes)
            {
                var runes = client.Inventory.GetItems(lowRunesID).ToList<Item>();
                if (runes.Count >= lowItemsCount) client.Modules.TextToSpeech.Speak("low runes", doFlashWindow);
            }

            if (alarmOnLowItems)
            {
                var items = client.Inventory.GetItems(lowItemsID).ToList<Item>();
                if (items.Count >= lowItemsCount) client.Modules.TextToSpeech.Speak("low items", doFlashWindow);
            }

            if (alarmOnStuck)
            {
                if (client.Player.Connected)
                {
                    if (client.Player.Location != oldLocation)
                    {
                        oldLocation = client.Player.Location;
                        timeSinceMoved = Environment.TickCount;
                    }
                    else if (Environment.TickCount - timeSinceMoved >= minTimeSinceMoved)
                    {
                        client.Modules.TextToSpeech.Speak("stuck", doFlashWindow);
                    }
                }
                else timeSinceMoved = Environment.TickCount;
            }

            if (alarmOnPrivateMessage || alarmOnDefaultMessage)
            {
                foreach (var msg in client.Window.GameWindow.GetMessages())
                {   
					msg.UpdateTime();
					if (msg.Time <= 0) continue;
					
                    if (msg.Type == GameWindow.Message.Types.PrivateMessage &&
                        alarmOnPrivateMessage)
                    {
                        client.Modules.TextToSpeech.Speak("private message", doFlashWindow);
                        break;
                    }
                    if (alarmOnDefaultMessage &&
                        (msg.Type == GameWindow.Message.Types.Say || msg.Type == GameWindow.Message.Types.Whisper || msg.Type == GameWindow.Message.Types.Yell) &&
                        msg.Sender != client.Player.Name)
                    {
                        client.Modules.TextToSpeech.Speak("default chat message", doFlashWindow);
                        break;
                    }
                }
            }
        }
    }
}
