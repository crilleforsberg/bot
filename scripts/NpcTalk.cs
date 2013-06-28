// NOTE:
// THIS IS NOT A COMPLETE SCRIPT
// YOU NEED TO CHANGE IT TO WORK FOR A SPECIFIC NPC

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
        string message = string.Empty;

        // call Say, which will sleep length*300 milliseconds before sending the packet
        Say(client, "hi");
        // set the NPC response as a variable
        message = "Hello " + client.Player.Name + "!";
        // stop script if message was not found
        if (!WaitForResponse(client, message)) return;
        // answer
        Say(client, "change 100 gold");
        Say(client, "yes");
    }

    static bool WaitForResponse(Client client, string expectedResponse, string npcName = "",  ushort time = 2000)
    {
        int tickStart = Environment.TickCount;
        while (Environment.TickCount < tickStart + time)
        {
            foreach (var gameWndMsg in client.Window.GameWindow.GetMessages())
            {
                var parsedMsg = gameWndMsg.Parse();
                if (parsedMsg.Message != expectedResponse) continue;
                if (!string.IsNullOrEmpty(npcName) && npcName != parsedMsg.Sender) continue;
                return true;
            }

            Thread.Sleep(500);
        }
        return false;
    }
    static void Say(Client client, string message)
    {
        Thread.Sleep(message.Length * 300);
        client.Packets.Say(message);
    }
}
