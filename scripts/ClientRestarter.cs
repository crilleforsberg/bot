using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    static string globalVariableName = "clientRestarterDelegate"; // the name of the global variable, no need to change
    static string loginScript = "Vivec.cs", // the name of the login script, must be configured properly
        scriptsPath = "scripts", // the sub-path to where your scripts are stored (set to "" if they are stored in the root dir)
        loginPath = Path.Combine(scriptsPath, "login scripts"); // the sub-path to your login scripts, i.e. scripts\login scripts

    public static void Main(Client client)
    {
        int time = 0, minTimeToWait = 6000;
        bool relog = true;

        client.Closing += clientClosing;
        client.Modules.ScriptManager.Variables.SetValue(globalVariableName, clientClosing);

        bool waitingForClose = false;
        while (true)
        {
            Thread.Sleep(2000);

            if (client.TibiaProcess.HasExited)
            {
                waitingForClose = true;
                break;
            }

            int tick = Environment.TickCount;
            client.TibiaProcess.Refresh();
            if (!client.TibiaProcess.Responding)
            {
                if (tick > time + minTimeToWait)
                {
                    client.TibiaProcess.Kill();
                    waitingForClose = true;
                    break;
                }
            }
            else time = tick;

            if (relog && !client.Player.Connected)
            {
                FileInfo fi = new FileInfo(Path.Combine(loginPath, loginScript));
                if (fi.Exists)
                {
                    var script = client.Modules.ScriptManager.CreateScript(fi);
                    script.Run();
                    Thread.Sleep(500);
                }
            }
        }
        while (waitingForClose) Thread.Sleep(100);
    }
    public static void Cleanup(Client client)
    {
        object result = null;
        if ((result = client.Modules.ScriptManager.Variables.GetValue(globalVariableName)) == null) return;
        if (!(result is Client.ClosingHandler)) return;
        client.Closing -= (Client.ClosingHandler)result;
    }
    public static Client.ClosingHandler clientClosing = delegate(Client client)
    {
        bool startMinimized = true, // whether to start the client in a minimized state
            loadRunningScripts = true, // whether to load currently running scripts
            loadDefinedScripts = true; // whether to load defined scripts (the ones below)
        string[] scripts = { Path.Combine(loginPath, loginScript) };
        string path = Path.Combine(client.BotDirectory.FullName, scriptsPath);

        var psi = new ProcessStartInfo(client.ClientFile.FullName, "engine 1"); // 0=dx5, 1=dx9, 2=ogl
        psi.WindowStyle = startMinimized ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal;
        psi.WorkingDirectory = client.ClientDirectory.FullName;
        var process = Process.Start(psi);
        process.WaitForInputIdle();
        while (process.MainWindowHandle == IntPtr.Zero)
        {
            process.Refresh();
            Thread.Sleep(500);
        }

        string arguments = "-p " + process.Id;
        List<string> loadedScripts = new List<string>();
        if (loadRunningScripts)
        {
            foreach (var script in client.Modules.ScriptManager.GetScripts())
            {
                if (!script.IsRunning) continue;
                loadedScripts.Add(script.ScriptFile.FullName);
            }
        }
        if (loadDefinedScripts)
        {
            foreach (string s in scripts) loadedScripts.Add(Path.Combine(path, s));
        }
        foreach (string s in loadedScripts) arguments += " --script \"" + s + "\"";
        psi = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, arguments);
        psi.WorkingDirectory = client.BotDirectory.FullName;
        Process.Start(psi).WaitForInputIdle();
    };
}
