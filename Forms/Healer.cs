using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace KarelazisBot.Forms
{
    public partial class Healer : Form
    {
        Objects.Client Client = null;

        internal Healer(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            comboboxAction.SelectedIndex = 1;
            if (this.Client.Player != null && this.Client.Player.Connected)
            {
                numericHealth.Value = (int)(Client.Player.HealthMax * 0.65);
                numericMana.Value = 80;// (int)(Client.Player.ManaMax * 0.5);
            }

            this.Client.Modules.Healer.CriteriaAdded += new Modules.Healer.CriteriaAddedHandler(Healer_CriteriaAdded);
            this.Client.Modules.Healer.CriteriaInserted += new Modules.Healer.CriteriaInsertedHandler(Healer_CriteriaInserted);
            this.Client.Modules.Healer.CriteriaRemoved += new Modules.Healer.CriteriaRemovedHandler(Healer_CriteriaRemoved);
        }

        void Healer_CriteriaRemoved(Modules.Healer.Criteria c, int index)
        {
            datagridHealer.Rows.RemoveAt(index);
        }

        void Healer_CriteriaInserted(Modules.Healer.Criteria c, int index)
        {
            datagridHealer.Rows.Insert(index, c.Action, c.Health + "/" + c.Mana);
        }

        void Healer_CriteriaAdded(Modules.Healer.Criteria c)
        {
            datagridHealer.Rows.Add(c.Action, c.Health + "/" + c.Mana);
        }

        private void Healer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
        }

        private void comboboxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtboxActionEx.Enabled = comboboxAction.SelectedIndex == 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            switch (comboboxAction.Text)
            {
                case "Say":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, txtboxActionEx.Text));
                    break;
                case "UH":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, 0, Modules.Healer.ActionType.UltimateHealingRune));
                    break;
                case "IH":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, 0, Modules.Healer.ActionType.IntenseHealingRune));
                    break;
                case "Mana":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria(0, (ushort)numericMana.Value, Modules.Healer.ActionType.ManaFluid));
                    break;
                case "MP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria(0, (ushort)numericMana.Value, Modules.Healer.ActionType.ManaPotion));
                    break;
                case "SMP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria(0, (ushort)numericMana.Value, Modules.Healer.ActionType.StrongManaPotion));
                    break;
                case "GMP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria(0, (ushort)numericMana.Value, Modules.Healer.ActionType.GreatManaPotion));
                    break;
                case "HP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, Modules.Healer.ActionType.HealthPotion));
                    break;
                case "SHP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, Modules.Healer.ActionType.StrongHealthPotion));
                    break;
                case "GHP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, Modules.Healer.ActionType.GreatHealthPotion));
                    break;
                case "UHP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, Modules.Healer.ActionType.UltimateHealthPotion));
                    break;
                case "GSP":
                    this.Client.Modules.Healer.AddCriteria(new Modules.Healer.Criteria((ushort)numericHealth.Value, (ushort)numericMana.Value, Modules.Healer.ActionType.GreatSpiritPotion));
                    break;
            }
        }

        private void checkboxHealerStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxHealerStatus.Checked != this.Client.Modules.Healer.IsRunning)
            {
                if (checkboxHealerStatus.Checked) this.Client.Modules.Healer.Start();
                else this.Client.Modules.Healer.Stop();
            }
        }

        private void datagridHealer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && datagridHealer.SelectedRows.Count > 0)
            {
                DataGridViewRow row = datagridHealer.SelectedRows[0];
                List<Modules.Healer.Criteria> criterias = new List<Modules.Healer.Criteria>(this.Client.Modules.Healer.GetCriterias());
                if (row.Index >= criterias.Count) return;
                this.Client.Modules.Healer.RemoveCriteria(criterias[row.Index]);
            }
        }

        private void toolstripRemove_Click(object sender, EventArgs e)
        {
            datagridHealer_KeyUp(sender, new KeyEventArgs(Keys.Delete));
        }

        private void datagridHealer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && datagridHealer.SelectedRows.Count > 0) contextmenuHealer.Show(Cursor.Position);
        }

        private void numericCheckInterval_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Healer.CheckInterval = (ushort)numericCheckInterval.Value;
        }

        private void numericMinSleep_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Healer.SleepMin = (ushort)numericMinSleep.Value;
        }

        private void numericMaxSleep_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Healer.SleepMax = (ushort)numericMaxSleep.Value;
        }
    }
}
