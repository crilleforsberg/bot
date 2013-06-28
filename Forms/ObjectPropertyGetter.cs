using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class ObjectPropertyGetter : Form
    {
        Objects.Client client = null;

        internal ObjectPropertyGetter(Objects.Client c)
        {
            this.client = c;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*string a = string.Empty;
            for (ushort id = 100; id < 3000; id++)
            {
                bool val = this.client.Packets.GetObjectProperty(id, 6);
                if (val) a += id + "\n";
                //System.Threading.Thread.Sleep(10);
            }
            MessageBox.Show(a); return;*/
            string s = string.Empty;
            for (byte i = 0; i < 30; i++)
            {
                bool val = this.client.Packets.GetObjectProperty((ushort)numericID.Value, i);
                if (!val) continue;
                s += (Enums.ObjectProperties772)i + "\n";
            }
            MessageBox.Show(s);
        }
    }
}
