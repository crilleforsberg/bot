using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KarelazisBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!File.Exists("CSScriptLibrary.dll"))
            {
                MessageBox.Show("CSScriptLibrary.dll is missing.\nCan not run application.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int pid = 0;
            var scriptPaths = new List<string>();
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-p":
                        case "--process":
                            if (args.Length == i + 1) break;
                            int.TryParse(args[i + 1], out pid);
                            break;
                        case "-s":
                        case "--script":
                            if (args.Length == i + 1) break;
                            string script = args[i + 1];
                            if (!script.ToLower().EndsWith(".cs") || !File.Exists(script)) break;
                            scriptPaths.Add(script);
                            break;
                        case "-c":
                        case "--cavebot":
                            break;
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (pid != 0)
            {
                Process p = null;
                try { p = Process.GetProcessById(pid); }
                catch { return; }
                if (p == null || p.HasExited) return;
                Objects.Client c = new Objects.Client(p, true);
                foreach (string s in scriptPaths)
                {
                    c.Modules.ScriptManager.AddScript(new Objects.Script(c, new FileInfo(s)));
                }
                c.Modules.ScriptManager.StartAllScripts();
                Application.Run(new Forms.MainHub(c));
                return;
            }

            List<Objects.Client> clients = Objects.Client.GetClients().ToList();
            if (clients.Count == 1 && clients[0].Addresses.IsValid)
            {
                Objects.Client c = new Objects.Client(clients[0].TibiaProcess);
                Application.Run(new Forms.MainHub(c));
            }
            else Application.Run(new Forms.ClientChooser());
        }
    }
}