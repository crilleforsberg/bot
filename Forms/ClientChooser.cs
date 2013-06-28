using System;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class ClientChooser : Form
    {
        public ClientChooser()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
        }

        private void txtboxClassName_TextChanged(object sender, EventArgs e)
        {
            if (txtboxClassName.Text.Length == 0) comboboxClients.Enabled = false;
            else if (!comboboxClients.Enabled) comboboxClients.Enabled = true;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (comboboxClients.SelectedIndex < 0) return;
            Objects.Client c = (Objects.Client)comboboxClients.SelectedItem;
            Forms.MainHub mainHub = new Forms.MainHub(c);
            mainHub.Show();
            this.Hide();
        }

        private void comboboxClients_DropDown(object sender, EventArgs e)
        {
            comboboxClients.Items.Clear();
            foreach (Objects.Client c in Objects.Client.GetClients(txtboxClassName.Text))
            {
                if (c.Addresses.IsValid) comboboxClients.Items.Add(c);
            }
        }

        private void ClientChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
