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
    public partial class WaypointEditor : Form
    {
        Objects.Client Client { get; set; }
        Modules.Cavebot.Waypoint Waypoint { get; set; }

        internal WaypointEditor(Objects.Client c, Modules.Cavebot.Waypoint waypoint)
        {
            this.Client = c;
            this.Waypoint = waypoint;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            timerUpdate.Start();
            comboboxWaypoint.Items.Add(waypoint);
            if (waypoint.Type == Modules.Cavebot.Waypoint.Types.Node)
            {
                foreach (Objects.Location loc in waypoint.NodeLocations) comboboxWaypoint.Items.Add(loc);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            lblPlayerLocation.Text = "Player location: " + (this.Client.Player.Connected ? this.Client.Player.Location.ToString() : "--");
        }

        private void comboboxWaypoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboboxWaypoint.SelectedIndex < 0) return;
            if (comboboxWaypoint.SelectedIndex == 0)
            {
                Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)comboboxWaypoint.SelectedItem;
                comboboxType.Enabled = true;
                foreach (object obj in comboboxType.Items)
                {
                    if (obj.ToString() == wp.Type.ToString()) { comboboxWaypoint.SelectedItem = obj; break; }
                }
                numericLocX.Value = wp.Location.X;
                numericLocY.Value = wp.Location.Y;
                numericLocZ.Value = wp.Location.Z;
            }
            else
            {
                Objects.Location subnode = (Objects.Location)comboboxWaypoint.SelectedItem;
                comboboxType.Enabled = false;
                numericLocX.Value = subnode.X;
                numericLocY.Value = subnode.Y;
                numericLocZ.Value = subnode.Z;
            }
        }

        private void comboboxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboboxWaypoint.SelectedIndex < 0 || comboboxWaypoint.SelectedIndex > 0) return;
            Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)comboboxWaypoint.SelectedItem;
            switch (comboboxType.SelectedItem.ToString())
            {
                case "Node":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
                case "Walk":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Walk;
                    break;
                case "Rope":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Rope;
                    break;
                case "Shovel":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Shovel;
                    break;
                case "Machete":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Machete;
                    break;
                case "Pick":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Pick;
                    break;
                case "Ladder":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Ladder;
                    break;
                case "Script":
                    wp.Type = Modules.Cavebot.Waypoint.Types.Script;
                    break;
                default:
                    wp.Type = Modules.Cavebot.Waypoint.Types.Node;
                    break;
            }
        }

        private void numericLocX_ValueChanged(object sender, EventArgs e)
        {
            if (comboboxWaypoint.SelectedIndex < 0) return;
            if (comboboxWaypoint.SelectedIndex == 0)
            {
                Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)comboboxWaypoint.SelectedItem;
                wp.Location.X = (ushort)numericLocX.Value;
            }
            else
            {
                Objects.Location subnode = (Objects.Location)comboboxWaypoint.SelectedItem;
                subnode.X = (ushort)numericLocX.Value;
            }
        }

        private void numericLocY_ValueChanged(object sender, EventArgs e)
        {
            if (comboboxWaypoint.SelectedIndex < 0) return;
            if (comboboxWaypoint.SelectedIndex == 0)
            {
                Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)comboboxWaypoint.SelectedItem;
                wp.Location.Y = (ushort)numericLocY.Value;
            }
            else
            {
                Objects.Location subnode = (Objects.Location)comboboxWaypoint.SelectedItem;
                subnode.Y = (ushort)numericLocY.Value;
            }
        }

        private void numericLocZ_ValueChanged(object sender, EventArgs e)
        {
            if (comboboxWaypoint.SelectedIndex < 0) return;
            if (comboboxWaypoint.SelectedIndex == 0)
            {
                Modules.Cavebot.Waypoint wp = (Modules.Cavebot.Waypoint)comboboxWaypoint.SelectedItem;
                wp.Location.Z = (byte)numericLocZ.Value;
            }
            else
            {
                Objects.Location subnode = (Objects.Location)comboboxWaypoint.SelectedItem;
                subnode.Z = (byte)numericLocZ.Value;
            }
        }
    }
}
