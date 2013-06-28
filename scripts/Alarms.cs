using System;
using System.Collections.Generic;
using KarelazisBot;
using KarelazisBot.Objects;
using System.Threading;
using System.Linq;

public class ExternalScript
{
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
            stopOnPlayerOnScreen = false,
            pauseOnPlayerOnScreen = true,
            alarmOnCreatureOnScreen = false,
            alarmOnGamemaster = true,
            stopOnGamemaster = false,
            pauseOnGamemaster = false,
            alarmOnPlayerKiller = true,
            stopOnPlayerKiller = false,
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
        bool stopOtherScripts = true,
            stopCavebot = true;

        List<Script> pausedScripts = new List<Script>();
        Location oldLocation = client.Player.Connected ? client.Player.Location : new Location();
        long timeSinceMoved = Environment.TickCount, // snapshot of current time in milliseconds
             minTimeSinceMoved = 1000 * 15; // 15 seconds
		
		int oldMsgIndex = 0;
        while (true)
        {
            Thread.Sleep(500);
			
            bool stop = false;

            if (alarmOnDisconnect && !client.Player.Connected)
            {
                if (!client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.Speak("disconnected", doFlashWindow);
            }

            if (alarmOnLowHealth && client.Player.Connected &&
                client.Player.HealthPercent <= minHealthPercent)
            {
                if (!client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.Speak("low health", doFlashWindow);
            }

            if (alarmOnPlayerOnScreen || alarmOnGamemaster || alarmOnPlayerKiller ||
                stopOnPlayerOnScreen || stopOnGamemaster || stopOnPlayerKiller ||
                pauseOnPlayerOnScreen || pauseOnGamemaster)
            {
                var players = client.BattleList.GetPlayers(true, true).ToList<Creature>();
                foreach (Creature c in players.ToArray())
                {
                    if (whiteList.Contains(c.Name) || client.Player.Name == c.Name) players.Remove(c);
                }

                if (alarmOnGamemaster || stopOnGamemaster || pauseOnGamemaster)
                {
                    bool found = false;
                    foreach (var msg in client.Window.GameWindow.GetMessages())
                    {
                        if (msg.Sender.StartsWith("CM") ||
                            msg.Sender.StartsWith("GM") ||
                            msg.Sender.StartsWith("Iryont") ||
                            msg.Sender.StartsWith("Gamemaster"))
                        {
                            found = true;
                            break;
                        }
                    }
                    foreach (Creature player in players)
                    {
                        if (player.Name.StartsWith("CM") ||
                            player.Name.StartsWith("GM") ||
                            player.Name.StartsWith("Iryont") ||
                            player.Name.StartsWith("Gamemaster"))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        if (alarmOnGamemaster && !client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.SpeakAsync("gamemaster", doFlashWindow);
                        if (stopOnGamemaster || pauseOnGamemaster) stop = true;
                    }
                }

                if ((stopOnPlayerOnScreen || alarmOnPlayerOnScreen || pauseOnPlayerOnScreen) && players.Count >= minPlayersCount)
                {
                    if (alarmOnPlayerOnScreen && !client.Modules.TextToSpeech.IsSpeaking())
                    {
                        // say players if > 1, say player if == 1
                        client.Modules.TextToSpeech.Speak(players.Count +
                            (players.Count > 1 ? " players on screen" : "player on screen"), doFlashWindow);
                    }
                    if (stopOnPlayerOnScreen || pauseOnPlayerOnScreen) stop = true;
                }

                if (alarmOnPlayerKiller || stopOnPlayerKiller)
                {
                    uint playerID = client.Player.ID;
                    foreach (Creature player in players)
                    {
                        if (player.ID == playerID) continue;

                        if (player.HasAttackedMeRecently((ushort)(1000 * 5)))
                        {
                            if (!client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.Speak("player killer", doFlashWindow);
                            if (stopOnPlayerKiller) stop = true;

                            break;
                        }
                    }
                }
            }

            #region creatures
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
            #endregion

            #region low runes/items
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
            #endregion

            #region stuck
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
            #endregion

            #region messages
            if (alarmOnPrivateMessage || alarmOnDefaultMessage)
            {
                foreach (var msg in client.Window.GameWindow.GetMessages())
                {   
					msg.UpdateTime();
					if (msg.Time <= 0) continue;
					
                    if (msg.Type == GameWindow.Message.Types.PrivateMessage &&
                        alarmOnPrivateMessage)
                    {
                        if (!client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.SpeakAsync("private message", doFlashWindow);
                        break;
                    }
                    if (alarmOnDefaultMessage &&
                        (msg.Type == GameWindow.Message.Types.Say || msg.Type == GameWindow.Message.Types.Whisper || msg.Type == GameWindow.Message.Types.Yell) &&
                        msg.Sender != client.Player.Name)
                    {
                        if (!client.Modules.TextToSpeech.IsSpeaking()) client.Modules.TextToSpeech.Speak("default chat message", doFlashWindow);
                        break;
                    }
                }
            }
            #endregion

            if (stop)
            {
                if (stopCavebot && client.Modules.Cavebot.IsRunning) client.Modules.Cavebot.Stop();
                if (stopOtherScripts)
                {
                    foreach (var script in client.Modules.ScriptManager.GetScripts())
                    {
                        if (script.ScriptFile.Name.Contains("Alarms")) continue;
                        if (!script.IsRunning) continue;
                        script.Stop();
                        pausedScripts.Add(script);
                    }
                }
            }
            else if (pausedScripts.Count > 0)
            {
                foreach (Script s in pausedScripts)
                {
                    if (!s.IsRunning) s.Run();
                }
                pausedScripts.Clear();
            }
        }
    }
}
