using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    public static void Main(Client client)
    {
        bool status = client.Modules.Cavebot.IsRunning;
        if (!status)
        {
            client.Modules.Cavebot.Start();
            client.Modules.ScriptManager.StartAllScripts();
        }
        else
        {
            client.Modules.Cavebot.Stop();
            client.Modules.ScriptManager.StopAllScripts();
        }
        client.Window.StatusBar.SetText("Bot " + (status ? " off!" : "on!"));
    }
}