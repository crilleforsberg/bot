using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace KarelazisBot.Forms
{
    public partial class MainHub : Form
    {
        Objects.Client Client = null;
        Forms.Cavebot formCavebot = null;
        Forms.Healer formHealer = null;
        Forms.Hotkeys formHotkeys = null;
        Forms.Information formInformation = null;
        Forms.PvP formPvP = null;
        Forms.Scripter formScripter = null;
        Forms.MapViewer formMapViewer = null;
        bool alwaysOnTop = false;

        internal MainHub(Objects.Client c)
        {
            if (!WinAPI.IsIconic(c.TibiaProcess.MainWindowHandle))
            {
                this.StartPosition = FormStartPosition.Manual;
                WinAPI.RECT rect = new WinAPI.RECT();
                WinAPI.GetWindowRect(c.TibiaProcess.MainWindowHandle, out rect);
                this.Location = new Point(rect.left + 10, rect.top + 30);
            }
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;

            if (!this.Client.HasLoadedProperties)
            {
                ObjectPropertiesLoader loader = new ObjectPropertiesLoader(this.Client);
                loader.ShowDialog();
                if (!loader.Finished) Environment.Exit(Environment.ExitCode);
                loader.Dispose();
            }

            this.timerUpdate.Start();
        }

        private void MainHub_Shown(object sender, EventArgs e)
        {
            this.Client.CloseWhenClientCloses = true;

            // instantiate modules
            formCavebot = new Cavebot(this.Client);
            formHealer = new Healer(this.Client);
            formHotkeys = new Hotkeys(this.Client);
            formInformation = new Information(this.Client);
            formPvP = new PvP(this.Client);
            formScripter = new Scripter(this.Client);
        }

        private void MainHub_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                trayIcon.Visible = true;
                this.Hide();
                e.Cancel = true;
                return;
            }
            trayIcon.Visible = false;

            //this.Client.Modules.Hotkeys.Unhook();
            //this.Client.Modules.HUD.Dispose();

            Environment.Exit(Environment.ExitCode);
        }

        private void toolstripmenuExit_Click(object sender, EventArgs e)
        {
            this.MainHub_FormClosing(sender, new FormClosingEventArgs(CloseReason.None, false));
        }

        #region Tray events
        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (!WinAPI.IsIconic(this.Client.TibiaProcess.MainWindowHandle))
            {
                this.StartPosition = FormStartPosition.Manual;
                WinAPI.RECT rect = new WinAPI.RECT();
                WinAPI.GetWindowRect(this.Client.TibiaProcess.MainWindowHandle, out rect);
                this.Location = new Point(rect.left + 10, rect.top + 30);
            }
            this.Show();
            trayIcon.Visible = false;
        }

        private void toolstriptrayShowHub_Click(object sender, EventArgs e)
        {
            this.trayIcon_DoubleClick(sender, e);
        }

        private void toolstriptrayExit_Click(object sender, EventArgs e)
        {
            this.toolstripmenuExit_Click(sender, e);
        }
        #endregion

        #region Module button events
        private void btnCavebot_Click(object sender, EventArgs e)
        {
            if (formCavebot == null) formCavebot = new Cavebot(Client);
            formCavebot.StartPosition = FormStartPosition.Manual;
            formCavebot.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formCavebot.Visible) formCavebot.Show();
            else formCavebot.Activate();
        }

        private void btnHealer_Click(object sender, EventArgs e)
        {
            if (formHealer == null) formHealer = new Healer(Client);
            formHealer.StartPosition = FormStartPosition.Manual;
            formHealer.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formHealer.Visible) formHealer.Show();
            else formHealer.Activate();
        }

        private void btnHotkeys_Click(object sender, EventArgs e)
        {
            if (formHotkeys == null) formHotkeys = new Hotkeys(Client);
            formHotkeys.StartPosition = FormStartPosition.Manual;
            formHotkeys.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formHotkeys.Visible) formHotkeys.Show();
            else formHotkeys.Activate();
        }

        private void btnInformation_Click(object sender, EventArgs e)
        {
            if (formInformation == null) formInformation = new Information(Client);
            formInformation.StartPosition = FormStartPosition.Manual;
            formInformation.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formInformation.Visible) formInformation.Show();
            else formInformation.Activate();
        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            if (formPvP == null) formPvP = new PvP(Client);
            formPvP.StartPosition = FormStartPosition.Manual;
            formPvP.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formPvP.Visible) formPvP.Show();
            else formPvP.Activate();
        }

        private void btnScripter_Click(object sender, EventArgs e)
        {
            if (formScripter == null) formScripter = new Scripter(Client);
            formScripter.StartPosition = FormStartPosition.Manual;
            formScripter.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formScripter.Visible) formScripter.Show();
            else formScripter.Activate();
        }

        private void btnMapViewer_Click(object sender, EventArgs e)
        {
            if (formMapViewer == null) formMapViewer = new MapViewer(Client);
            formMapViewer.StartPosition = FormStartPosition.Manual;
            formMapViewer.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            if (!formMapViewer.Visible) formMapViewer.Show();
            else formMapViewer.Activate();
        }
        #endregion

        private void toolstripmenuAlwaysOnTop_Click(object sender, EventArgs e)
        {
            this.alwaysOnTop = !this.alwaysOnTop;
            toolstripmenuAlwaysOnTop.Checked = this.alwaysOnTop;
            this.TopMost = this.alwaysOnTop;
            this.formCavebot.TopMost = this.alwaysOnTop;
            this.formHealer.TopMost = this.alwaysOnTop;
            this.formHotkeys.TopMost = this.alwaysOnTop;
            this.formInformation.TopMost = this.alwaysOnTop;
            this.formPvP.TopMost = this.alwaysOnTop;
            this.formScripter.TopMost = this.alwaysOnTop;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            string text = this.Client.Player.Name + " - Karelazi's Bot";
            if (this.trayIcon.Visible) this.trayIcon.Text = text;
            else if (this.Text != text) this.Text = text;
        }
    }
}
