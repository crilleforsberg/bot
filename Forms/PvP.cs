using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace KarelazisBot.Forms
{
    public partial class PvP : Form
    {
        Objects.Client Client { get; set; }
        string GlobalIP = "--";

        internal PvP(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            this.comboboxCombobotClientAction.SelectedIndex = 0;
            this.Client.Modules.CombobotClient.Connected += new Modules.CombobotClient.ConnectedHandler(CombobotClient_Connected);
            this.Client.Modules.CombobotClient.Disconnected += new Modules.CombobotClient.Disconnectedhandler(CombobotClient_Disconnected);
            this.Client.Modules.CombobotServer.ClientConnected += new Modules.CombobotServer.ClientConnectedHandler(CombobotServer_ClientConnected);
            this.Client.Modules.CombobotServer.ClientDisconnected += new Modules.CombobotServer.ClientDisconnectedHandler(CombobotServer_ClientDisconnected);
            this.Client.Modules.CombobotServer.ClientPingReceived += new Modules.CombobotServer.ClientPingReceivedHandler(CombobotServer_ClientPingReceived);
        }

        void CombobotServer_ClientPingReceived(Modules.CombobotClientDescriptor descriptor)
        {
            int index = 0;
            foreach (object obj in listboxCombobotServerConnectedCharacters.Items)
            {
                if ((Modules.CombobotClientDescriptor)obj == descriptor)
                {
                    listboxCombobotServerConnectedCharacters.Items[index] = descriptor;
                    break;
                }
                index++;
            }
        }

        void CombobotServer_ClientDisconnected(Modules.CombobotClientDescriptor descriptor)
        {
            listboxCombobotServerConnectedCharacters.Items.Remove(descriptor);
        }

        void CombobotServer_ClientConnected(Modules.CombobotClientDescriptor descriptor)
        {
            listboxCombobotServerConnectedCharacters.Items.Add(descriptor);
        }

        void CombobotClient_Disconnected()
        {
            btnCombobotClientConnect.Text = "Connect";
        }

        void CombobotClient_Connected()
        {
            btnCombobotClientConnect.Text = "Disconnect";
        }

        private void PvP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); }
        }

        private void PvP_Shown(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(this.ThreadGetGlobalIP));
            t.Start();
        }

        void ThreadGetGlobalIP()
        {
            try
            {
                WebClient wc = new WebClient();
                string ip = wc.DownloadString(@"http://automation.whatismyip.com/n09230945.asp");
                this.GlobalIP = ip;
                this.lblCombobotServerGlobalIP.Text = "Your IP: " + GlobalIP;
            }
            catch { }
        }

        #region Combobot client
        private void comboboxCombobotClientAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboboxCombobotClientAction.SelectedIndex)
            {
                case 0:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.Say;
                    break;
                case 1:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.CustomRune;
                    break;
                case 2:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.SuddenDeath;
                    break;
                case 3:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.HeavyMagicMissile;
                    break;
                case 4:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.Explosion;
                    break;
                case 5:
                    this.Client.Modules.CombobotClient.Action = Modules.CombobotClient.ActionType.Paralyze;
                    break;
            }
            switch (comboboxCombobotClientAction.SelectedIndex)
            {
                case 0:
                case 1:
                    txtboxCombobotClientActionEx.Enabled = true;
                    break;
                default:
                    txtboxCombobotClientActionEx.Enabled = false;
                    break;
            }
        }

        private void txtboxCombobotClientActionEx_TextChanged(object sender, EventArgs e)
        {
            this.Client.Modules.CombobotClient.ActionEx = txtboxCombobotClientActionEx.Text;
        }

        private void btnCombobotClientConnect_Click(object sender, EventArgs e)
        {
            if (btnCombobotClientConnect.Text == "Connect")
            {
                try
                {
                    string[] split = txtboxCombobotClientIPPort.Text.Split(':');
                    string ip = split[0];
                    ushort port = ushort.Parse(split[1]);
                    if (this.Client.Modules.CombobotClient.IsConnected) this.Client.Modules.CombobotClient.Disconnect();
                    this.Client.Modules.CombobotClient.Connect(ip, port);
                    btnCombobotClientConnect.Text = "Disconnect";
                }
                catch { btnCombobotClientConnect.Text = "Connect"; }
            }
            else
            {
                this.Client.Modules.CombobotClient.Disconnect();
                btnCombobotClientConnect.Text = "Connect";
            }
        }
        #endregion

        #region Combobot server
        private void btnCombobotServerStart_Click(object sender, EventArgs e)
        {
            if (btnCombobotServerStart.Text == "Start server")
            {
                this.Client.Modules.CombobotServer.Start((ushort)numericCombobotServerPort.Value);
                numericCombobotServerPort.Enabled = false;
                btnCombobotServerStart.Text = "Stop server";
            }
            else
            {
                this.Client.Modules.CombobotServer.Stop();
                numericCombobotServerPort.Enabled = true;
                btnCombobotServerStart.Text = "Start server";
            }
        }
        #endregion
    }
}
