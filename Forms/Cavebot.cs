using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using KarelazisBot.Objects;

namespace KarelazisBot.Forms
{
    public partial class Cavebot : Form
    {
        Objects.Client Client { get; set; }
        int currentSettingIndex = 0;

        internal Cavebot(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            this.timerStatusWatcher.Start();

            // set default UI values
            comboboxWaypointsOffset.SelectedIndex = 0;
            comboboxWaypointsType.SelectedIndex = 0;
            comboboxTargetingFightMode.SelectedIndex = 0;
            comboboxTargetingMinCount.SelectedIndex = 0;
            comboboxTargetingSpellRune.SelectedIndex = 0;
            comboboxTargetingStance.SelectedIndex = 0;
            comboboxLootingDestination.SelectedIndex = 1;

            // set default settings
            checkboxSettingsDebugMode.Checked = true;
            checkboxSettingsCanUseMagicRope.Checked = true;
            checkboxSettingsEatFood.Checked = true;
            checkboxSettingsStickToCreature.Checked = true;
            checkboxSettingsStopAttackingWhenOutOfRange.Checked = true;
            checkboxSettingsUseAlternateNodeFinder.Checked = true;

            // set up events
            this.Client.Modules.Cavebot.WaypointAdded += new Modules.Cavebot.WaypointHandler(Cavebot_WaypointAdded);
            this.Client.Modules.Cavebot.WaypointInserted += new Modules.Cavebot.WaypointInsertedHandler(Cavebot_WaypointInserted);
            this.Client.Modules.Cavebot.WaypointRemoved += new Modules.Cavebot.WaypointHandler(Cavebot_WaypointRemoved);
            this.Client.Modules.Cavebot.TargetAdded += new Modules.Cavebot.TargetHandler(Cavebot_TargetAdded);
            this.Client.Modules.Cavebot.TargetRemoved += new Modules.Cavebot.TargetHandler(Cavebot_TargetRemoved);
            this.Client.Modules.Cavebot.LootAdded += new Modules.Cavebot.LootHandler(Cavebot_LootAdded);
            this.Client.Modules.Cavebot.LootRemoved += new Modules.Cavebot.LootHandler(Cavebot_LootRemoved);
            this.Client.Modules.Cavebot.StatusChanged += new Modules.Cavebot.StatusChangedHandler(Cavebot_StatusChanged);
        }

        void Cavebot_StatusChanged(bool cavebotStatus)
        {
            checkboxCavebotStatus.Checked = cavebotStatus;
        }

        void Cavebot_LootRemoved(Modules.Cavebot.Loot Loot)
        {
            foreach (DataGridViewRow row in datagridviewLooting.Rows)
            {
                if (ushort.Parse(row.Cells[0].Value.ToString()) == Loot.ID &&
                    uint.Parse(row.Cells[1].Value.ToString()) == Loot.Cap)
                {
                    datagridviewLooting.Rows.Remove(row);
                    break;
                }
            }
        }

        void Cavebot_LootAdded(Modules.Cavebot.Loot Loot)
        {
            datagridviewLooting.Rows.Add(Loot.ID, Loot.Cap, Loot.Destination);
        }

        void Cavebot_TargetRemoved(Modules.Cavebot.Target Target)
        {
            listboxTargetingTargets.Items.Remove(Target);
        }

        void Cavebot_TargetAdded(Modules.Cavebot.Target Target)
        {
            listboxTargetingTargets.Items.Add(Target);
        }

        void Cavebot_WaypointRemoved(Modules.Cavebot.Waypoint waypoint)
        {
            listboxWaypoints.Items.Remove(waypoint);
        }

        void Cavebot_WaypointInserted(Modules.Cavebot.Waypoint waypoint, int index)
        {
            listboxWaypoints.Items.Insert(index, waypoint);
        }

        void Cavebot_WaypointAdded(Modules.Cavebot.Waypoint waypoint)
        {
            listboxWaypoints.Items.Add(waypoint);
        }

        private void Cavebot_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
        }

        private void tabcontrolCavebot_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = "Cavebot - " + tabcontrolCavebot.SelectedTab.Text;
        }

        private void timerStatusWatcher_Tick(object sender, EventArgs e)
        {
            if (this.Client == null) return;
            if (this.Client.Modules.Cavebot.IsRunning)
            {
                if (listboxWaypoints.Items.Count > 0 && this.Client.Modules.Cavebot.CurrentWaypointIndex >= 0 &&
                    this.Client.Modules.Cavebot.CurrentWaypointIndex <= listboxWaypoints.Items.Count) listboxWaypoints.SelectedIndex = this.Client.Modules.Cavebot.CurrentWaypointIndex;
            }

            if (this.Client.Modules.Cavebot.CurrentSettings.CanUseMagicRope != checkboxSettingsCanUseMagicRope.Checked) checkboxSettingsCanUseMagicRope.Checked = this.Client.Modules.Cavebot.CurrentSettings.CanUseMagicRope;
            if (this.Client.Modules.Cavebot.CurrentSettings.EatFood != checkboxSettingsEatFood.Checked) checkboxSettingsEatFood.Checked = this.Client.Modules.Cavebot.CurrentSettings.EatFood;
            if (this.Client.Modules.Cavebot.CurrentSettings.Exhaust != numericTargetingExhaust.Value) numericTargetingExhaust.Value = this.Client.Modules.Cavebot.CurrentSettings.Exhaust;
            if (this.Client.Modules.Cavebot.CurrentSettings.MinimumHealthToShoot != numericTargetingMinHealth.Value) numericTargetingMinHealth.Value = this.Client.Modules.Cavebot.CurrentSettings.MinimumHealthToShoot;
            if (this.Client.Modules.Cavebot.CurrentSettings.NodeRadius != numericSettingsNodeRadius.Value) numericSettingsNodeRadius.Value = this.Client.Modules.Cavebot.CurrentSettings.NodeRadius;
            if (this.Client.Modules.Cavebot.CurrentSettings.NodeSkipRange != numericSettingsSkipNodeRange.Value) numericSettingsSkipNodeRange.Value = this.Client.Modules.Cavebot.CurrentSettings.NodeSkipRange;
            if (this.Client.Modules.Cavebot.CurrentSettings.OpenContainers != checkboxSettingsOpenBags.Checked) checkboxSettingsOpenBags.Checked = this.Client.Modules.Cavebot.CurrentSettings.OpenContainers;
            if (this.Client.Modules.Cavebot.CurrentSettings.PrioritizeDanger != checkboxSettingsPrioritizeDanger.Checked) checkboxSettingsPrioritizeDanger.Checked = this.Client.Modules.Cavebot.CurrentSettings.PrioritizeDanger;
            if (this.Client.Modules.Cavebot.CurrentSettings.StickToCreature != checkboxSettingsStickToCreature.Checked) checkboxSettingsStickToCreature.Checked = this.Client.Modules.Cavebot.CurrentSettings.StickToCreature;
            if (this.Client.Modules.Cavebot.CurrentSettings.StopAttackingWhenOutOfRange != checkboxSettingsStopAttackingWhenOutOfRange.Checked) checkboxSettingsStopAttackingWhenOutOfRange.Checked = this.Client.Modules.Cavebot.CurrentSettings.StopAttackingWhenOutOfRange;
            if (this.Client.Modules.Cavebot.CurrentSettings.UseGoldStacks != checkboxSettingsUseGoldStacks.Checked) checkboxSettingsUseGoldStacks.Checked = this.Client.Modules.Cavebot.CurrentSettings.UseGoldStacks;
            if (this.Client.Modules.Cavebot.CurrentSettings.DebugMode != checkboxSettingsDebugMode.Checked) checkboxSettingsDebugMode.Checked = this.Client.Modules.Cavebot.CurrentSettings.DebugMode;
            if (this.Client.Modules.Cavebot.CurrentSettings.ConsiderAllMonstersWhenKeepingAway != checkboxSettingsConsiderAllMonstersWhenKeepingAway.Checked) checkboxSettingsConsiderAllMonstersWhenKeepingAway.Checked = this.Client.Modules.Cavebot.CurrentSettings.ConsiderAllMonstersWhenKeepingAway;
        }

        private void checkboxCavebotStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxCavebotStatus.Checked) this.Client.Modules.Cavebot.Start();
            else this.Client.Modules.Cavebot.Stop();
        }

        #region Settings UI events
        private void checkboxSettingsKillBeforeLooting_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.KillBeforeLooting = checkboxSettingsKillBeforeLooting.Checked;
        }

        private void checkboxSettingsFastLooting_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.FastLooting = checkboxSettingsFastLooting.Checked;
        }

        private void checkboxSettingsFriendlyMode_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.FriendlyMode = checkboxSettingsFriendlyMode.Checked;
        }

        private void checkboxSettingsUseGoldStacks_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.UseGoldStacks = checkboxSettingsUseGoldStacks.Checked;
        }

        private void checkboxSettingsEatFood_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.EatFood = checkboxSettingsEatFood.Checked;
        }

        private void numericSettingsNodeRadius_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.NodeRadius = (byte)numericSettingsNodeRadius.Value;
        }

        private void numericSettingsSkipNodeRange_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.NodeSkipRange = (byte)numericSettingsSkipNodeRange.Value;
        }

        private void checkboxSettingsPrioritizeDanger_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.PrioritizeDanger = checkboxSettingsPrioritizeDanger.Checked;
        }

        private void checkboxSettingsStickToCreature_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.StickToCreature = checkboxSettingsStickToCreature.Checked;
        }

        private void checkboxSettingsStopAttackingWhenOutOfRange_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.StopAttackingWhenOutOfRange = checkboxSettingsStopAttackingWhenOutOfRange.Checked;
        }

        private void checkboxSettingsDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.DebugMode = checkboxSettingsDebugMode.Checked;
        }

        private void checkboxSettingsCanUseMagicRope_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.CanUseMagicRope = checkboxSettingsCanUseMagicRope.Checked;
        }

        private void checkboxSettingsUseAlternateNodeFinder_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.UseAlternateNodeFinder = checkboxSettingsUseAlternateNodeFinder.Checked;
        }

        private void checkboxSettingsConsiderAllMonstersWhenKeepingAway_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.ConsiderAllMonstersWhenKeepingAway = checkboxSettingsConsiderAllMonstersWhenKeepingAway.Checked;
        }

        private void checkboxSettingsAllowDiagonalMovement_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.AllowDiagonalMovement = checkboxSettingsAllowDiagonalMovement.Checked;
            this.Client.Modules.Cavebot.PathFinder.AllowDiagonals = this.Client.Modules.Cavebot.CurrentSettings.AllowDiagonalMovement;
        }

        private void numericSettingsKeepAwayPerimeter_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.KeepAwayPerimeter = (byte)numericSettingsKeepAwayPerimeter.Value;
        }
        #endregion

        #region Looting UI events
        private void btnLootingAdd_Click(object sender, EventArgs e)
        {
            if (comboboxLootingDestination.Text == string.Empty) return;
            bool found = false;
            foreach (Modules.Cavebot.Loot l in this.Client.Modules.Cavebot.GetLoot())
            {
                if (l.ID == (ushort)numericLootingItemID.Value) { found = true; break; }
            }
            if (found) return;

            Modules.Cavebot.Loot.Destinations destination = Modules.Cavebot.Loot.Destinations.Ground;
            byte index = 0;
            switch (comboboxLootingDestination.Text.ToLower())
            {
                case "ground":
                    destination = Modules.Cavebot.Loot.Destinations.Ground;
                    break;
                default:
                    destination = Modules.Cavebot.Loot.Destinations.EmptyContainer;
                    index = byte.Parse(comboboxLootingDestination.Text[1].ToString());
                    break;
            }

            Modules.Cavebot.Loot newLoot = new Modules.Cavebot.Loot((ushort)numericLootingItemID.Value,
                string.Empty, (ushort)numericLootingMinCapacity.Value, destination, index);
            this.Client.Modules.Cavebot.AddLoot(newLoot);
        }

        private void datagridviewLooting_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && datagridviewLooting.SelectedRows.Count > 0)
            {
                ushort id = (ushort)datagridviewLooting.SelectedRows[0].Cells[0].Value;
                foreach (Modules.Cavebot.Loot l in this.Client.Modules.Cavebot.GetLoot())
                {
                    if (l.ID == id)
                    {
                        this.Client.Modules.Cavebot.RemoveLoot(l);
                        break;
                    }
                }
            }
        }

        private void toolstripLootingRemove_Click(object sender, EventArgs e)
        {
            this.datagridviewLooting_KeyUp(sender, new KeyEventArgs(Keys.Delete));
        }

        private void datagridviewLooting_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && datagridviewLooting.SelectedRows.Count > 0) contextmenuLooting.Show(Cursor.Position);
        }
        #endregion

        #region Targeting UI events
        private void listboxTargetingTargets_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                toolstripTargetingRemove.Enabled = listboxTargetingTargets.SelectedIndex >= 0;
                toolstripTargetingMoveUp.Enabled = listboxTargetingTargets.SelectedIndex > 0;
                toolstripTargetingMoveDown.Enabled = listboxTargetingTargets.SelectedIndex < listboxTargetingTargets.Items.Count - 1;
                contextmenuTargeting.Show(Cursor.Position);
            }
        }

        private void toolstripTargetingAdd_Click(object sender, EventArgs e)
        {
            this.btnTargetingNewTarget_Click(sender, e);
        }

        private void toolstripTargetingRemove_Click(object sender, EventArgs e)
        {
            this.listboxTargetingTargets_KeyUp(sender, new KeyEventArgs(Keys.Delete));
        }

        private void listboxTargetingTargets_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listboxTargetingTargets.SelectedIndex >= 0)
            {
                Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
                this.Client.Modules.Cavebot.RemoveTarget(t);
            }
        }

        private void toolstripTargetingMoveUp_Click(object sender, EventArgs e)
        {
            int index = listboxTargetingTargets.SelectedIndex;
            if (index <= 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            listboxTargetingTargets.Items.RemoveAt(index);
            listboxTargetingTargets.Items.Insert(index - 1, t);
        }

        private void toolstripTargetingMoveDown_Click(object sender, EventArgs e)
        {
            int index = listboxTargetingTargets.SelectedIndex;
            if (index < 0 || index >= listboxTargetingTargets.Items.Count) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            listboxTargetingTargets.Items.RemoveAt(index);
            listboxTargetingTargets.Items.Insert(index + 1, t);
        }

        private void listboxTargetingTargets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            txtboxTargetingName.Text = t.Name;
            if (!this.radiobtnTargetingSetting1.Checked) this.radiobtnTargetingSetting1.Checked = true;
            else this.SetTargetingValues(0);
        }

        private void txtboxTargetingName_TextChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            t.Name = txtboxTargetingName.Text;
            listboxTargetingTargets.Items[listboxTargetingTargets.SelectedIndex] = t;
        }

        private void comboboxTargetingMinCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.Count = (byte)comboboxTargetingMinCount.SelectedIndex;
        }

        private void comboboxTargetingStance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            switch (comboboxTargetingStance.SelectedIndex)
            {
                case 0:
                    s.FightStance = Enums.FightStance.Follow;
                    break;
                case 1:
                    s.FightStance = Enums.FightStance.FollowDiagonalOnly;
                    break;
                case 2:
                    s.FightStance = Enums.FightStance.FollowStrike;
                    break;
                case 3:
                    s.FightStance = Enums.FightStance.Stand;
                    break;
                case 4:
                    s.FightStance = Enums.FightStance.DistanceFollow;
                    break;
                case 5:
                    s.FightStance = Enums.FightStance.DistanceWait;
                    break;
            }
        }

        private void comboboxTargetingFightMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            switch (comboboxTargetingFightMode.SelectedIndex)
            {
                case 0:
                    s.FightMode = Enums.FightMode.Offensive;
                    break;
                case 1:
                    s.FightMode = Enums.FightMode.Balanced;
                    break;
                case 2:
                    s.FightMode = Enums.FightMode.Defensive;
                    break;
            }
        }

        private void comboboxTargetingSpellRune_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            int index = comboboxTargetingSpellRune.SelectedIndex;
            if (index == 0)
            {
                s.Rune = Modules.Cavebot.Target.Setting.Runes.None;
                s.Spell = string.Empty;
            }
            else if (index > 0 && index < 5)
            {
                s.Rune = (Modules.Cavebot.Target.Setting.Runes)comboboxTargetingSpellRune.SelectedItem;
                s.Spell = string.Empty;
            }
            else
            {
                s.Rune = Modules.Cavebot.Target.Setting.Runes.None;
                s.Spell = comboboxTargetingSpellRune.Text;
            }
        }

        private void numericTargetingDanger_ValueChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.DangerLevel = (byte)numericTargetingDanger.Value;
        }

        private void numericTargetingRange_ValueChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.Range = (byte)numericTargetingRange.Value;
        }

        private void checkboxTargetingReachable_CheckedChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.MustBeReachable = checkboxTargetingReachable.Checked;
        }

        private void checkboxTargetingShootable_CheckedChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.MustBeShootable = checkboxTargetingShootable.Checked;
        }

        private void checkboxTargetingLoot_CheckedChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            t.DoLoot = checkboxTargetingLoot.Checked;
        }

        private void numericTargetingRangeDistance_ValueChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.DistanceRange = (byte)numericTargetingRangeDistance.Value;
        }

        private void btnTargetingNewTarget_Click(object sender, EventArgs e)
        {
            Modules.Cavebot.Target t = new Modules.Cavebot.Target(this.Client.Modules.Cavebot);
            this.Client.Modules.Cavebot.AddTarget(t);
        }

        private void numericTargetingMinHealth_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.MinimumHealthToShoot = (byte)numericTargetingMinHealth.Value;
        }

        private void numericTargetingExhaust_ValueChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.Exhaust = (ushort)numericTargetingExhaust.Value;
        }

        private void checkboxSettingsOpenBags_CheckedChanged(object sender, EventArgs e)
        {
            this.Client.Modules.Cavebot.CurrentSettings.OpenContainers = checkboxSettingsOpenBags.Checked;
        }

        private void radiobtnTargetingSetting1_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnTargetingSetting1.Checked) this.SetTargetingValues(0);
        }

        private void radiobtnTargetingSetting2_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnTargetingSetting2.Checked) this.SetTargetingValues(1);
        }

        private void radiobtnTargetingSetting3_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnTargetingSetting3.Checked) this.SetTargetingValues(2);
        }

        private void radiobtnTargetingSetting4_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnTargetingSetting4.Checked) this.SetTargetingValues(3);
        }

        private void SetTargetingValues(int settingIndex)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            this.currentSettingIndex = settingIndex;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            comboboxTargetingMinCount.SelectedIndex = s.Count;
            switch (s.FightStance)
            {
                case Enums.FightStance.Follow:
                default:
                    comboboxTargetingStance.SelectedIndex = 0;
                    break;
                case Enums.FightStance.FollowDiagonalOnly:
                    comboboxTargetingStance.SelectedIndex = 1;
                    break;
                case Enums.FightStance.FollowStrike:
                    comboboxTargetingStance.SelectedIndex = 2;
                    break;
                case Enums.FightStance.Stand:
                    comboboxTargetingStance.SelectedIndex = 3;
                    break;
                case Enums.FightStance.DistanceFollow:
                    comboboxTargetingStance.SelectedIndex = 4;
                    break;
                case Enums.FightStance.DistanceWait:
                    comboboxTargetingStance.SelectedIndex = 5;
                    break;
            }
            switch (s.FightMode)
            {
                case Enums.FightMode.Offensive:
                    comboboxTargetingFightMode.SelectedIndex = 0;
                    break;
                case Enums.FightMode.Balanced:
                    comboboxTargetingFightMode.SelectedIndex = 1;
                    break;
                case Enums.FightMode.Defensive:
                    comboboxTargetingFightMode.SelectedIndex = 2;
                    break;
            }
            if (s.Spell == string.Empty && s.Rune == Modules.Cavebot.Target.Setting.Runes.None) comboboxTargetingSpellRune.SelectedIndex = 0;
            else
            {
                for (int i = 0; i < comboboxTargetingSpellRune.Items.Count; i++)
                {
                    string val = comboboxTargetingSpellRune.Items[i].ToString();
                    if (val == s.Rune.ToString() || val == s.Spell)
                    {
                        comboboxTargetingSpellRune.SelectedIndex = i;
                        break;
                    }
                }
            }
            numericTargetingDanger.Value = s.DangerLevel;
            numericTargetingRange.Value = s.Range;
            numericTargetingRangeDistance.Value = s.DistanceRange;
            checkboxTargetingReachable.Checked = s.MustBeReachable;
            checkboxTargetingShootable.Checked = s.MustBeShootable;
            checkboxTargetingLoot.Checked = t.DoLoot;
            checkboxTargetingUseThisSetting.Checked = s.UseThisSetting;
        }

        private void checkboxTargetingUseThisSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (listboxTargetingTargets.SelectedIndex < 0) return;
            Modules.Cavebot.Target t = (Modules.Cavebot.Target)listboxTargetingTargets.SelectedItem;
            Modules.Cavebot.Target.Setting s = t.GetSettingByIndex(this.currentSettingIndex);
            s.UseThisSetting = checkboxTargetingUseThisSetting.Checked;
        }
        #endregion

        #region Waypoints UI events
        private void btnWaypointsAdd_Click(object sender, EventArgs e)
        {
            if (!this.Client.Player.Connected || this.Client.Player.Health == 0) return;
            if (comboboxWaypointsOffset.SelectedIndex < 0 || comboboxWaypointsType.SelectedIndex < 0) return;
            Modules.Cavebot.Waypoint.Types type;
            switch (comboboxWaypointsType.Text)
            {
                case "Node":
                    type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
                case "Walk":
                    type = Modules.Cavebot.Waypoint.Types.Walk;
                    break;
                case "Rope":
                    type = Modules.Cavebot.Waypoint.Types.Rope;
                    break;
                case "Shovel":
                    type = Modules.Cavebot.Waypoint.Types.Shovel;
                    break;
                case "Machete":
                    type = Modules.Cavebot.Waypoint.Types.Machete;
                    break;
                case "Pick":
                    type = Modules.Cavebot.Waypoint.Types.Pick;
                    break;
                case "Ladder":
                    type = Modules.Cavebot.Waypoint.Types.Ladder;
                    break;
                case "Script":
                    type = Modules.Cavebot.Waypoint.Types.Script;
                    break;
                default:
                    type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
            }
            int diffX = 0, diffY = 0;
            switch (comboboxWaypointsOffset.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    diffY = -1;
                    break;
                case 2:
                    diffX = 1;
                    break;
                case 3:
                    diffY = 1;
                    break;
                case 4:
                    diffX = -1;
                    break;
            }
            if (type == Modules.Cavebot.Waypoint.Types.Script)
            {
                Objects.Location loc = this.Client.Player.Location.Offset(diffX, diffY);
                Modules.Cavebot.Waypoint wp = new Modules.Cavebot.Waypoint(this.Client.Modules.Cavebot,
                    loc, Modules.Cavebot.Waypoint.Types.Script);
                this.Client.Modules.Cavebot.AddWaypoint(wp);
            }
            else
            {
                Modules.Cavebot.Waypoint wp = new Modules.Cavebot.Waypoint(this.Client.Modules.Cavebot,
                    this.Client.Player.Location.Offset(diffX, diffY), type);
                if (type == Modules.Cavebot.Waypoint.Types.Node && numericSettingsNodeRadius.Value > 0)
                {
                    ushort pX = Client.Player.X, pY = Client.Player.Y;
                    byte pZ = Client.Player.Z, radius = (byte)numericSettingsNodeRadius.Value;
                    Map.TileCollection tiles = Client.Map.GetTilesOnScreen();
                    Map.Tile playerTile = tiles.GetTile(count: this.Client.Player.ID);
                    if (playerTile == null) { MessageBox.Show("Could not find player tile"); return; }
                    List<Objects.Location> nodeLocations = new List<Objects.Location>();
                    for (ushort x = (ushort)(pX - radius); x < pX + radius; x++)
                    {
                        for (ushort y = (ushort)(pY - radius); y < pY + radius; y++)
                        {
                            if (x == pX && y == pY) continue;
                            Map.Tile tile = tiles.GetTile(new Objects.Location(x, y, pZ));
                            if (tile == null) continue;
                            if (playerTile.WorldLocation.CanReachLocation(this.Client, tile.WorldLocation, tiles)) nodeLocations.Add(tile.WorldLocation);
                        }
                    }
                    wp.NodeLocations.AddRange(nodeLocations);
                }
                this.Client.Modules.Cavebot.AddWaypoint(wp);
            }
        }

        private void btnWaypointsInsert_Click(object sender, EventArgs e)
        {
            if (!Client.Player.Connected || Client.Player.Health == 0) return;
            if (comboboxWaypointsOffset.SelectedIndex < 0 || comboboxWaypointsType.SelectedIndex < 0 || listboxWaypoints.SelectedIndex < 0) return;
            Modules.Cavebot.Waypoint.Types type;
            switch (comboboxWaypointsType.Text)
            {
                case "Node":
                    type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
                case "Walk":
                    type = Modules.Cavebot.Waypoint.Types.Walk;
                    break;
                case "Rope":
                    type = Modules.Cavebot.Waypoint.Types.Rope;
                    break;
                case "Shovel":
                    type = Modules.Cavebot.Waypoint.Types.Shovel;
                    break;
                case "Machete":
                    type = Modules.Cavebot.Waypoint.Types.Machete;
                    break;
                case "Pick":
                    type = Modules.Cavebot.Waypoint.Types.Pick;
                    break;
                case "Ladder":
                    type = Modules.Cavebot.Waypoint.Types.Ladder;
                    break;
                case "Script":
                    type = Modules.Cavebot.Waypoint.Types.Script;
                    break;
                default:
                    type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
            }
            int diffX = 0, diffY = 0;
            switch (comboboxWaypointsOffset.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    diffY = -1;
                    break;
                case 2:
                    diffX = 1;
                    break;
                case 3:
                    diffY = 1;
                    break;
                case 4:
                    diffX = -1;
                    break;
            }
            int index = listboxWaypoints.SelectedIndex;
            if (type == Modules.Cavebot.Waypoint.Types.Script)
            {
                Objects.Location loc = new Objects.Location(this.Client.Player.X + diffX,
                    this.Client.Player.Y + diffY, this.Client.Player.Z);
                Modules.Cavebot.Waypoint wp = new Modules.Cavebot.Waypoint(this.Client.Modules.Cavebot, loc, Modules.Cavebot.Waypoint.Types.Script);
                this.Client.Modules.Cavebot.InsertWaypoint(wp, index);
            }
            else
            {
                Modules.Cavebot.Waypoint wp = new Modules.Cavebot.Waypoint(this.Client.Modules.Cavebot,
                    new Objects.Location(this.Client.Player.X + diffX, this.Client.Player.Y + diffY, this.Client.Player.Z), type);
                if (type == Modules.Cavebot.Waypoint.Types.Node && numericSettingsNodeRadius.Value > 0)
                {
                    ushort pX = this.Client.Player.X, pY = this.Client.Player.Y;
                    byte pZ = this.Client.Player.Z, radius = (byte)numericSettingsNodeRadius.Value;
                    Map.TileCollection tiles = this.Client.Map.GetTilesOnScreen();
                    Map.Tile playerTile = tiles.GetTile(count: this.Client.Player.ID);
                    if (playerTile == null) { MessageBox.Show("Could not find player tile"); return; }
                    List<Objects.Location> nodeLocations = new List<Objects.Location>();
                    for (ushort x = (ushort)(pX - radius); x < pX + radius; x++)
                    {
                        for (ushort y = (ushort)(pY - radius); y < pY + radius; y++)
                        {
                            if (x == pX && y == pY) continue;
                            Map.Tile tile = tiles.GetTile(new Objects.Location(x, y, pZ));
                            if (tile == null) continue;
                            if (tile.IsWalkable()) nodeLocations.Add(tile.WorldLocation);
                        }
                    }
                    wp.NodeLocations.AddRange(nodeLocations);
                }
                this.Client.Modules.Cavebot.InsertWaypoint(wp, index);
            }
        }

        private void btnWaypointsLoadScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Choose a script to load";
            openFile.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFile.Filter = "C# code file (*.cs)|*.cs";
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Modules.Cavebot.Waypoint wp = this.Client.Modules.Cavebot.GetWaypoints()
                    .ToArray<Modules.Cavebot.Waypoint>()[this.listboxWaypoints.SelectedIndex];
                if (wp.Type != Modules.Cavebot.Waypoint.Types.Script) return;
                using (System.IO.Stream fstream = openFile.OpenFile())
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fstream))
                    {
                        wp.Script = new Modules.Cavebot.Script(this.Client.Modules.Cavebot, reader.ReadToEnd(), wp);
                    }
                }
                txtboxWaypointScript.Text = wp.Script.Code;
            }
        }

        private void btnWaypointsClearScript_Click(object sender, EventArgs e)
        {
            Modules.Cavebot.Waypoint wp = this.Client.Modules.Cavebot.GetWaypoints()
                .ToArray<Modules.Cavebot.Waypoint>()[this.listboxWaypoints.SelectedIndex];
            if (wp.Type != Modules.Cavebot.Waypoint.Types.Script) return;
            if (MessageBox.Show("Are you sure you want to clear this script?", "Warning", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                wp.Script.Code = string.Empty;
            }
        }

        private void toolstripRemoveWaypoint_Click(object sender, EventArgs e)
        {
            if (listboxWaypoints.SelectedIndex < 0) return;
            Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)listboxWaypoints.SelectedItem;
            this.Client.Modules.Cavebot.RemoveWaypoint(wp);
        }

        private void toolstripSetAsCurrent_Click(object sender, EventArgs e)
        {
            if (this.listboxWaypoints.SelectedIndex < 0) return;
            this.Client.Modules.Cavebot.CurrentWaypointIndex = this.listboxWaypoints.SelectedIndex;
        }

        private void listboxWaypoints_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listboxWaypoints.SelectedIndex >= 0) toolstripRemoveWaypoint_Click(sender, new EventArgs());
        }

        private void listboxWaypoints_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && listboxWaypoints.SelectedIndex >= 0)
            {
                contextmenuWaypoints.Show(Cursor.Position);
            }
        }

        private void toolstripEditWaypoint_Click(object sender, EventArgs e)
        {
            if (listboxWaypoints.SelectedIndex < 0) return;
            WaypointEditor wpEditor = new WaypointEditor(this.Client, (Modules.Cavebot.Waypoint)listboxWaypoints.SelectedItem);
            wpEditor.StartPosition = FormStartPosition.Manual;
            wpEditor.Location = Cursor.Position;
            wpEditor.Show();
        }

        private void listboxWaypoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toolstripEditWaypoint_Click(sender, new EventArgs());
        }

        private void listboxWaypoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxWaypoints.SelectedIndex < 0) return;
            Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)listboxWaypoints.SelectedItem;
            bool enable = wp.Type == Modules.Cavebot.Waypoint.Types.Script;
            if (btnWaypointsLoadScript.Enabled != enable) btnWaypointsLoadScript.Enabled = enable;
            if (btnWaypointsClearScript.Enabled != enable) btnWaypointsClearScript.Enabled = enable;
            if (txtboxWaypointScript.Enabled != enable) txtboxWaypointScript.Enabled = enable;
            if (enable) txtboxWaypointScript.Text = wp.Script.Code;
            else txtboxWaypointScript.Text = string.Empty;
        }

        private void txtboxWaypointScript_TextChanged(object sender, EventArgs e)
        {
            if (listboxWaypoints.SelectedIndex < 0) return;
            Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)listboxWaypoints.SelectedItem;
            if (wp.Type != Modules.Cavebot.Waypoint.Types.Script) return;
            wp.Script.Code = txtboxWaypointScript.Text;
        }
        #endregion

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Cavebot scripts (*.cs;*.kbotcave)|*.cs;*.kbotcave|All files (*.*)|*.*";
            openFileDialog.Title = "Load cavebot script";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Client.Modules.Cavebot.RemoveAllLoot();
                this.Client.Modules.Cavebot.RemoveAllTargets();
                this.Client.Modules.Cavebot.RemoveAllWaypoints();

                this.Client.Modules.Cavebot.Load(new System.IO.FileInfo(openFileDialog.FileName));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "C# scripts (*.cs)|*.cs";
            saveFileDialog.Title = "Save cavebot script";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Client.Modules.Cavebot.Save(new System.IO.FileInfo(saveFileDialog.FileName));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all entries?", "Warning", MessageBoxButtons.YesNo) ==
                System.Windows.Forms.DialogResult.Yes)
            {
                this.Client.Modules.Cavebot.Clear();
            }
        }
    }
}
