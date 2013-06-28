using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class HotkeysHelp : Form
    {
        Objects.Client Client { get; set; }

        internal HotkeysHelp(Objects.Client c)
        {
            InitializeComponent();
            this.Client = c;
            this.Icon = Properties.Resources.icon;
        }

        private void HotkeysHelp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); }
        }
    }
}
