using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class Hotkeys : Form
    {
        class KeyDescriptor
        {
            internal KeyDescriptor(Keys key)
            {
                this._Key = key;
            }

            private Keys _Key { get; set; }
            internal Keys Key { get { return this._Key; } }

            public override string ToString()
            {
                return "[" + (int)this.Key + "] " + this.Key.ToString();
            }
        }

        Objects.Client Client = null;
        Forms.HotkeysHelp formHotkeysHelp = null;
        KeyDescriptor currentKeyPressed = null;
        Objects.Script currentScript = null;

        internal Hotkeys(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            timerStatusWatcher.Start();
            checkboxHotkeysStatus.Checked = true;
            this.Client.Modules.Hotkeys.ActionAdded += new Modules.Hotkeys.ActionAddedHandler(Hotkeys_ActionAdded);
            this.Client.Modules.Hotkeys.ActionRemoved += new Modules.Hotkeys.ActionRemovedHandler(Hotkeys_ActionRemoved);
            this.Client.Modules.Hotkeys.StatusChanged += new Modules.Hotkeys.StatusChangedHandler(Hotkeys_StatusChanged);
        }

        void Hotkeys_StatusChanged(bool status)
        {
            this.checkboxHotkeysStatus.Checked = status;
        }

        void Hotkeys_ActionRemoved(Modules.Hotkeys.Action action)
        {
            foreach (DataGridViewRow row in this.datagridHotkeys.Rows)
            {
                KeyDescriptor keydesc = new KeyDescriptor(action.Key);
                if (row.Cells[0].Value.ToString() == keydesc.ToString())
                {
                    this.datagridHotkeys.Rows.Remove(row);
                    break;
                }
            }
        }

        void Hotkeys_ActionAdded(Modules.Hotkeys.Action action)
        {
            KeyDescriptor keydesc = new KeyDescriptor(action.Key);
            datagridHotkeys.Rows.Add(keydesc.ToString(), action.Script.ScriptFile.Name);
        }

        private void Hotkeys_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); }
        }

        private void txtboxKey_KeyUp(object sender, KeyEventArgs e)
        {
            //txtboxKey.Text = "[" + e.KeyValue + "] " + e.KeyCode.ToString();
            this.currentKeyPressed = new KeyDescriptor(e.KeyCode);
            txtboxKey.Text = this.currentKeyPressed.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.currentKeyPressed == null || this.currentScript == null) return;
            if (this.currentScript.ScriptFile == null || !this.currentScript.ScriptFile.Exists) return;
            if (this.Client.Modules.Hotkeys.ContainsKey(this.currentKeyPressed.Key)) return;
            this.Client.Modules.Hotkeys.AddAction(this.currentKeyPressed.Key, this.currentScript.ScriptFile, true);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            if (formHotkeysHelp == null) formHotkeysHelp = new HotkeysHelp(this.Client);
            if (!formHotkeysHelp.Visible) formHotkeysHelp.Show();
            else formHotkeysHelp.Activate();
        }

        private void checkboxHotkeysStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkboxHotkeysStatus.Focused) return;
            if (checkboxHotkeysStatus.Checked) this.Client.Modules.Hotkeys.Start();
            else this.Client.Modules.Hotkeys.Stop();
        }

        private void toolstripRemove_Click(object sender, EventArgs e)
        {
            this.datagridHotkeys_KeyUp(sender, new KeyEventArgs(Keys.Delete));
        }

        private void datagridHotkeys_KeyUp(object sender, KeyEventArgs e)
        {
            if (datagridHotkeys.SelectedRows.Count == 0) return;
            DataGridViewRow row = datagridHotkeys.SelectedRows[0];
            string cell = row.Cells[0].Value.ToString();
            int key = int.Parse(cell.Substring(1, cell.IndexOf(']') - 1));
            this.Client.Modules.Hotkeys.RemoveKey((Keys)key);
        }

        private void txtboxAction_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "C# code files (*.cs)|*.cs|All files (*.*)|*.*";
            openFile.Title = "Choose a C# code file";
            openFile.InitialDirectory = Application.StartupPath;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.currentScript = new Objects.Script(this.Client, new FileInfo(openFile.FileName));
                txtboxScriptPath.Text = this.currentScript.ScriptFile.Name;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
