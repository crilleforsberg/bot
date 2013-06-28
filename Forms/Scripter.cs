using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace KarelazisBot.Forms
{
    public partial class Scripter : Form
    {
        internal Objects.Client Client { get; private set; }

        internal Scripter(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
        }

        void ScriptManager_ScriptStarted(Objects.Script script)
        {
            var row = this.GetRow(script);
            if (row != null) row.Cells[1].Value = script.IsRunning;
        }

        void ScriptManager_ScriptRemoved(Objects.Script script)
        {
            var row = this.GetRow(script);
            if (row != null) this.datagridScripts.Rows.Remove(row);
        }

        void ScriptManager_ScriptFinished(Objects.Script script)
        {
            var row = this.GetRow(script);
            if (row != null) row.Cells[1].Value = script.IsRunning;
        }

        void ScriptManager_ScriptAdded(Objects.Script script)
        {
            this.datagridScripts.Rows.Add(script, script.IsRunning);
        }

        private DataGridViewRow GetRow(Objects.Script script)
        {
            foreach (DataGridViewRow row in this.datagridScripts.Rows)
            {
                Objects.Script s = (Objects.Script)row.Cells[0].Value;
                if (s == script) return row;
            }
            return null;
        }

        private void Scripter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); }
        }

        private void btnNewScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "C# code files (*.cs)|*.cs|All files (*.*)|*.*";
            openFile.Title = "Choose a C# code file to open";
            openFile.InitialDirectory = Application.StartupPath;
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                foreach (string str in openFile.FileNames)
                {
                    Objects.Script script = new Objects.Script(this.Client, new FileInfo(str));
                    this.Client.Modules.ScriptManager.AddScript(script);
                }
            }
        }

        private void btnRemoveScript_Click(object sender, EventArgs e)
        {
            if (this.datagridScripts.SelectedRows.Count <= 0) return;
            Objects.Script script = (Objects.Script)datagridScripts.SelectedRows[0].Cells[0].Value;
            this.Client.Modules.ScriptManager.RemoveScript(script);
        }

        private void btnStartStopScript_Click(object sender, EventArgs e)
        {
            if (this.datagridScripts.SelectedRows.Count <= 0) return;
            Objects.Script script = (Objects.Script)datagridScripts.SelectedRows[0].Cells[0].Value;
            if (!script.IsRunning) script.Run(true);
            else script.Stop();
        }

        private void btnStopAllScripts_Click(object sender, EventArgs e)
        {
            this.Client.Modules.ScriptManager.StopAllScripts();
        }

        private void btnStartAllScripts_Click(object sender, EventArgs e)
        {
            this.Client.Modules.ScriptManager.StartAllScripts();
        }

        private void txtboxQuickExec_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtboxQuickExec.Text.Length > 0 && e.KeyCode == Keys.Enter)
            {
                string template = "using System;\n" +
                    "using System.Collections.Generic;\n" +
                    "using KarelazisBot;\n" + 
                    "using KarelazisBot.Objects;\n" + 
                    "using System.Threading;\n" + 
                    "public class Test\n" + 
                    "{\n" + 
                        "public static void Main(Client client)\n" +
                        "{\n" +
                            txtboxQuickExec.Text + "\n" +
                        "}\n" +
                    "}\n";
                var fi = this.Client.Modules.CacheManager.SetFile("tempscript.cs", System.Text.Encoding.UTF8.GetBytes(template));
                new Objects.Script(this.Client, fi).Run(false);
                fi.Delete();
                e.Handled = true;
                txtboxQuickExec.Text = string.Empty;
            }
        }

        private void Scripter_Shown(object sender, EventArgs e)
        {
            foreach (var script in this.Client.Modules.ScriptManager.GetScripts()) this.ScriptManager_ScriptAdded(script);

            this.Client.Modules.ScriptManager.ScriptAdded += new Modules.ScriptManager.GenericScriptHandler(ScriptManager_ScriptAdded);
            this.Client.Modules.ScriptManager.ScriptFinished += new Modules.ScriptManager.GenericScriptHandler(ScriptManager_ScriptFinished);
            this.Client.Modules.ScriptManager.ScriptRemoved += new Modules.ScriptManager.GenericScriptHandler(ScriptManager_ScriptRemoved);
            this.Client.Modules.ScriptManager.ScriptStarted += new Modules.ScriptManager.GenericScriptHandler(ScriptManager_ScriptStarted);
        }
    }
}
