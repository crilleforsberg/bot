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
        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            foreach (var script in client.Modules.ScriptManager.GetScripts())
            {
                if (!script.IsRunning) script.Run(true);
            }
        }
    }
}
