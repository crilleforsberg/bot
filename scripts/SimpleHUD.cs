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
        GameWindow.Message msg = new GameWindow.Message()
        {
            ForgeIndex = 9,
            IsVisible = true,
            Location = new Location(client.Player.X, 1, 1),
            Type = GameWindow.Message.Types.DarkYellowMessage,
            Time = 1000
        };

		while (true)
		{
            Thread.Sleep(500);

            if (!client.Player.Connected) continue;

            msg.Text = "Experience to level " + (client.Player.Level + 1) + ": " +
                (client.Modules.ExperienceCounter.GetExperienceTNL() - client.Player.Experience) + "\n" + client.Window.GameWindow.NextIndex;
            //msg.Index = client.Window.GameWindow.NextIndex;
            msg.Location = new Location(1, 1, 1);
            client.Window.GameWindow.ForgeMessage(msg);
		}
    }
}
