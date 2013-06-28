using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class MapViewer : Form
    {
        Objects.Client Client { get; set; }
        int panelY { get;set; }
        int checkboxY { get; set; }
        public float Scale { get; set; }
        public readonly float ScaleMin = 1,
                              ScaleMax = 7;
        int oldWidth { get; set; }
        int oldHeight { get; set; }
        Objects.Location CurrentLocation { get; set; }
        Image CurrentFloorImage { get; set; }
        Rectangle CurrentFloorBounds { get; set; }
        bool mouseIsPressed = false;
        Point mouseOldPosition, mouseRightClickPosition;
        bool hasDrawn = false;
        Modules.Cavebot.Waypoint selectedWaypoint, hoveredWaypoint;
        Font TextFont { get; set; }

        public MapViewer(Objects.Client c)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
            this.Client = c;
            this.panelY = this.Height - this.picboxMap.Height;
            this.checkboxY = this.Height - this.checkboxDrawCreatures.Location.Y;
            this.MinimumSize = this.Size;
            this.CurrentLocation = new Objects.Location(32000, 32000, 7);
            this.Scale = 1;
            this.MouseWheel += new MouseEventHandler(picboxMap_MouseWheel);
            this.TextFont = new Font("Tahoma", 10f);
        }

        private void MapViewer_Shown(object sender, EventArgs e)
        {
            this.timerUpdatePanel.Start();
            Objects.Location loc = this.Client.Player.Connected ?
                this.Client.Player.Location.Offset(-(int)Math.Ceiling(this.picboxMap.Width / this.Scale / 2),
                -(int)Math.Ceiling(this.picboxMap.Height / this.Scale / 2), 0) :
                new Objects.Location(32000, 32000, 7);
            this.CurrentFloorBounds = this.Client.Modules.MapViewer.GetBounds(loc);
            this.CurrentLocation = new Objects.Location(loc.X - this.CurrentFloorBounds.X,
                loc.Y - this.CurrentFloorBounds.Y, loc.Z);
            Image img = this.Client.Modules.MapViewer.GetImage(this.CurrentLocation);
            if (img != null) this.CurrentFloorImage = img;
            //this.Client.Modules.MapViewer.GenerateCache();
            //picboxMap.Image = this.Client.Modules.MapViewer.GetImage(this.Client.Player.Location, picboxMap.Width, picboxMap.Height);//this.Client.Modules.MapViewer.GetImage(new Modules.MapViewer.MapFile(new System.IO.FileInfo(@"D:\Spel\Tibianic HR\12712609.map")));
            //picboxMap.Refresh();
            //this.Text = picboxMap.Image.Width + "x" + picboxMap.Image.Height + " - " + this.Client.Player.Location.ToString();
        }

        private void RedrawMap(bool clear, Graphics g)
        {
            if (this.CurrentFloorImage == null) return;
            if (clear) g.Clear(Color.Black);

            Objects.Location playerLoc = this.Client.Player.Location;

            if (checkboxLockToPlayer.Checked && this.Client.Player.Connected)
            {
                // get map location
                Point pt = new Point(playerLoc.X - this.CurrentFloorBounds.X,
                    playerLoc.Y - this.CurrentFloorBounds.Y);
                // set current location based on player location
                this.CurrentLocation = new Objects.Location(pt.X - (int)(this.picboxMap.Width / this.Scale / 2),
                    pt.Y - (int)(this.picboxMap.Height / this.Scale / 2), playerLoc.Z);
            }

            g.DrawImage(this.CurrentFloorImage, new Rectangle(0, 0, this.picboxMap.Width, this.picboxMap.Height),
                new Rectangle(this.CurrentLocation.X, this.CurrentLocation.Y,
                    (int)(this.picboxMap.Width / this.Scale), (int)(this.picboxMap.Height / this.Scale)),
                GraphicsUnit.Pixel);

            // get viewport
            Rectangle viewport = new Rectangle(this.CurrentLocation.X, this.CurrentLocation.Y,
                (int)(picboxMap.Width / this.Scale), (int)(picboxMap.Height / this.Scale));

            // draw creatures on the map
            if (checkboxDrawCreatures.Checked && this.Client.Player.Connected)
            {
                foreach (Objects.Creature c in this.Client.BattleList.GetCreatures(true, true))
                {
                    if (c.Z == this.CurrentLocation.Z)
                    {
                        // get map location of creature
                        Point mapLoc = new Point(c.X - this.CurrentFloorBounds.X, c.Y - this.CurrentFloorBounds.Y);
                        // check if creature is within view
                        if (mapLoc.X >= viewport.Left && mapLoc.Y >= viewport.Top &&
                            mapLoc.X <= viewport.Right && mapLoc.Y <= viewport.Bottom)
                        {
                            // get position relative to the image
                            Point pos = new Point((int)((mapLoc.X - viewport.Left) * this.Scale),
                                (int)((mapLoc.Y - viewport.Top) * this.Scale));
                            g.DrawLine(Pens.Red, new Point(pos.X - (int)(25 / this.Scale), pos.Y),
                                new Point(pos.X + (int)(25 / this.Scale), pos.Y));
                            g.DrawLine(Pens.Red, new Point(pos.X, pos.Y - (int)(25 / this.Scale)),
                                new Point(pos.X, pos.Y + (int)(25 / this.Scale)));
                        }
                    }
                }
            }

            // draw players/npcs on the map
            if (checkboxDrawPlayersNPCs.Checked && this.Client.Player.Connected)
            {
                foreach (Objects.Creature c in this.Client.BattleList.GetPlayers(true, true))
                {
                    if (c.Z == this.CurrentLocation.Z)
                    {
                        // get map location of player
                        Point mapLoc = new Point(c.X - this.CurrentFloorBounds.X, c.Y - this.CurrentFloorBounds.Y);
                        // check if creature is within view
                        if (mapLoc.X >= viewport.Left && mapLoc.Y >= viewport.Top &&
                            mapLoc.X <= viewport.Right && mapLoc.Y <= viewport.Bottom)
                        {
                            // get position relative to the image
                            Point pos = new Point((int)((mapLoc.X - viewport.Left) * this.Scale),
                                (int)((mapLoc.Y - viewport.Top) * this.Scale));
                            g.DrawLine(Pens.Blue, new Point(pos.X - (int)(25 / this.Scale), pos.Y),
                                new Point(pos.X + (int)(25 / this.Scale), pos.Y));
                            g.DrawLine(Pens.Blue, new Point(pos.X, pos.Y - (int)(25 / this.Scale)),
                                new Point(pos.X, pos.Y + (int)(25 / this.Scale)));
                        }
                    }
                }
            }
            
            // draw waypoints
            if (checkboxDrawWaypointPaths.Checked)
            {
                foreach (Modules.Cavebot.Waypoint wp in this.Client.Modules.Cavebot.GetWaypoints())
                {
                    // check if waypoint is on our current floor
                    if (wp.Location.Z != this.CurrentLocation.Z) continue;

                    // get map location of waypoint
                    Point mapLoc = new Point(wp.Location.X - this.CurrentFloorBounds.X,
                        wp.Location.Y - this.CurrentFloorBounds.Y);

                    // check if waypoint is within view
                    if (mapLoc.X >= viewport.Left && mapLoc.Y >= viewport.Top &&
                        mapLoc.X <= viewport.Right && mapLoc.Y <= viewport.Bottom)
                    {
                        // get position relative to the image
                        Point pos = new Point((int)((mapLoc.X - viewport.Left) * this.Scale),
                                    (int)((mapLoc.Y - viewport.Top) * this.Scale));
                        int ellipseSize = 10;
                        g.FillEllipse(Brushes.SkyBlue, new Rectangle(pos.X - ellipseSize / 2, pos.Y - ellipseSize / 2,
                            ellipseSize, ellipseSize));
                        g.DrawEllipse(Pens.Black, new Rectangle(pos.X - ellipseSize / 2, pos.Y - ellipseSize / 2,
                            ellipseSize, ellipseSize));
                    }

                    if (this.hoveredWaypoint != null)
                    {
                        mapLoc = new Point(this.hoveredWaypoint.Location.X - this.CurrentFloorBounds.X,
                        this.hoveredWaypoint.Location.Y - this.CurrentFloorBounds.Y);
                        Point pos = new Point((int)((mapLoc.X - viewport.Left) * this.Scale),
                                    (int)((mapLoc.Y - viewport.Top) * this.Scale));
                        this.DrawOutlinedText(g, this.hoveredWaypoint.ToString(), this.TextFont,
                            Brushes.Yellow, Brushes.Black, pos);
                    }
                }
            }

            // draw player cross on the map
            if (this.CurrentLocation.Z == playerLoc.Z && this.Client.Player.Connected)
            {
                // get map location of player
                Point mapLoc = new Point(playerLoc.X - this.CurrentFloorBounds.X, playerLoc.Y - this.CurrentFloorBounds.Y);
                // check if player is within view
                if (mapLoc.X >= viewport.Left && mapLoc.Y >= viewport.Top &&
                    mapLoc.X <= viewport.Right && mapLoc.Y <= viewport.Bottom)
                {
                    // get position relative to the image
                    Point playerPosition = new Point((int)((mapLoc.X - viewport.Left) * this.Scale),
                        (int)((mapLoc.Y - viewport.Top) * this.Scale));
                    
                    // draw outline
                    g.DrawLine(Pens.Black, new Point(playerPosition.X - (int)(25 / this.Scale), playerPosition.Y - 1),
                        new Point(playerPosition.X + (int)(25 / this.Scale), playerPosition.Y - 1));
                    g.DrawLine(Pens.Black, new Point(playerPosition.X - 1, playerPosition.Y - (int)(25 / this.Scale)),
                        new Point(playerPosition.X - 1, playerPosition.Y + (int)(25 / this.Scale)));
                    g.DrawLine(Pens.Black, new Point(playerPosition.X - (int)(25 / this.Scale), playerPosition.Y + 1),
                        new Point(playerPosition.X + (int)(25 / this.Scale), playerPosition.Y + 1));
                    g.DrawLine(Pens.Black, new Point(playerPosition.X + 1, playerPosition.Y - (int)(25 / this.Scale)),
                        new Point(playerPosition.X + 1, playerPosition.Y + (int)(25 / this.Scale)));

                    g.DrawLine(Pens.White, new Point(playerPosition.X - (int)(25 / this.Scale), playerPosition.Y),
                        new Point(playerPosition.X + (int)(25 / this.Scale), playerPosition.Y));
                    g.DrawLine(Pens.White, new Point(playerPosition.X, playerPosition.Y - (int)(25 / this.Scale)),
                        new Point(playerPosition.X, playerPosition.Y + (int)(25 / this.Scale)));
                }
            }

            // time to draw a text overlay
            // get world location from viewport
            Objects.Location worldLoc = this.CurrentLocation.Offset(viewport.X, viewport.Y, 0);
            worldLoc.SetOffset((ushort)this.CurrentFloorBounds.X, (ushort)this.CurrentFloorBounds.Y, 0);
            // draw it
            this.DrawOutlinedText(g, worldLoc.ToString(), this.TextFont,
                Brushes.Yellow, Brushes.Black, new PointF(5, 5));

            return;
        }

        private void DrawOutlinedText(Graphics g, string text, Font font, Brush color, Brush outlineColor, PointF position)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    g.DrawString(text, font, outlineColor, new PointF(position.X + x, position.Y + y));
                }
            }
            g.DrawString(text, font, color, position);
        }

        private void timerUpdatePanel_Tick(object sender, EventArgs e)
        {
            this.timerUpdatePanel.Interval = 1000;
            if (this.Client.Player.Connected)
            {
                if (checkboxLockToPlayer.Checked &&
                    this.Client.Player.Z != this.CurrentLocation.Z)
                {
                    Image img = this.Client.Modules.MapViewer.GetImage(this.Client.Player.Location);
                    if (img != null) this.CurrentFloorImage = img;
                }
                picboxMap.Invalidate();
            }
            //this.Text = this.CurrentFloorImage.Size.ToString() + " "  +
            //    (this.CurrentFloorImage.Width * this.CurrentFloorImage.Height * 3 / 1024) + " KiB";
        }

        private void MapViewer_Resize(object sender, EventArgs e)
        {
            this.picboxMap.Height = this.Height - this.panelY;
            this.checkboxDrawCreatures.Location = new Point(this.checkboxDrawCreatures.Location.X, this.Height - this.checkboxY);
            this.checkboxDrawPlayersNPCs.Location = new Point(this.checkboxDrawPlayersNPCs.Location.X, this.Height - this.checkboxY);
            this.checkboxDrawWaypointPaths.Location = new Point(this.checkboxDrawWaypointPaths.Location.X, this.Height - this.checkboxY);
            this.checkboxLockToPlayer.Location = new Point(this.checkboxLockToPlayer.Location.X, this.Height - this.checkboxY);
            this.picboxMap.Invalidate();
        }

        private void contextitemResetPosition_Click(object sender, EventArgs e)
        {
            Objects.Location playerLoc = this.Client.Player.Location;
            // get map location
            Point pt = new Point(playerLoc.X - this.CurrentFloorBounds.X,
                playerLoc.Y - this.CurrentFloorBounds.Y);
            // set current location based on player location
            this.CurrentLocation = new Objects.Location(pt.X - (int)(this.picboxMap.Width / this.Scale / 2),
                pt.Y - (int)(this.picboxMap.Height / this.Scale / 2), playerLoc.Z);
            this.picboxMap.Invalidate();
        }

        private void contextitemFloorUp_Click(object sender, EventArgs e)
        {
            if (this.CurrentLocation.Z <= 0 || this.CurrentLocation.Z >= 15) return;
            this.CurrentLocation.Z--;
            this.CurrentFloorBounds = this.Client.Modules.MapViewer.GetBounds(this.CurrentLocation);
            Image img = this.Client.Modules.MapViewer.GetImage(this.CurrentLocation);
            if (img == null) return;
            this.CurrentFloorImage = img;
            picboxMap.Invalidate();
        }

        private void contextitemFloorDown_Click(object sender, EventArgs e)
        {
            if (this.CurrentLocation.Z <= 0 || this.CurrentLocation.Z >= 15) return;
            this.CurrentLocation.Z++;
            this.CurrentFloorBounds = this.Client.Modules.MapViewer.GetBounds(this.CurrentLocation);
            Image img = this.Client.Modules.MapViewer.GetImage(this.CurrentLocation);
            if (img == null) return;
            this.CurrentFloorImage = img;
            picboxMap.Invalidate();
        }

        private void contextitemWaypointRemove_Click(object sender, EventArgs e)
        {
            if (this.selectedWaypoint == null) return;
            this.Client.Modules.Cavebot.RemoveWaypoint(this.selectedWaypoint);
            this.selectedWaypoint = null;
            picboxMap.Invalidate();
        }

        private void picboxMap_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.RedrawMap(false, e.Graphics);
        }

        private void picboxMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.mouseRightClickPosition = e.Location;
                // get world location of mouse click
                Objects.Location worldLoc = new Objects.Location(this.CurrentFloorBounds.X + this.CurrentLocation.X + (int)(e.Location.X / this.Scale),
                    this.CurrentFloorBounds.Y + this.CurrentLocation.Y + (int)(e.Location.Y / this.Scale),
                    this.CurrentLocation.Z);

                bool found = false;
                foreach (Modules.Cavebot.Waypoint wp in this.Client.Modules.Cavebot.GetWaypoints())
                {
                    if (wp.Location.DistanceTo(worldLoc) < 10 / this.Scale)
                    {
                        found = true;
                        this.selectedWaypoint = wp;
                        break;
                    }
                }
                if (!found) this.selectedWaypoint = null;
                contextitemWaypointRemove.Enabled = found;

                contextmenuMap.Show(Cursor.Position);
            }
        }

        private void picboxMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && this.Scale < this.ScaleMax)
            {
                Size oldSize = new Size((int)(this.picboxMap.Width / this.Scale) / 2,
                    (int)(this.picboxMap.Height / this.Scale) / 2);
                this.Scale *= 2f;
                Size newSize = new Size((int)(this.picboxMap.Width / this.Scale) / 2,
                    (int)(this.picboxMap.Height / this.Scale) / 2);
                this.CurrentLocation.X += oldSize.Width - newSize.Width;
                this.CurrentLocation.Y += oldSize.Height - newSize.Height;
                this.picboxMap.Invalidate();
            }
            else if (e.Delta < 0 && this.Scale > this.ScaleMin)
            {
                Size oldSize = new Size((int)(this.picboxMap.Width / this.Scale) / 2,
                    (int)(this.picboxMap.Height / this.Scale) / 2);
                this.Scale /= 2f;
                Size newSize = new Size((int)(this.picboxMap.Width / this.Scale) / 2,
                    (int)(this.picboxMap.Height / this.Scale) / 2);
                this.CurrentLocation.X += oldSize.Width - newSize.Width;
                this.CurrentLocation.Y += oldSize.Height - newSize.Height;
                this.picboxMap.Invalidate();
            }
        }

        private void picboxMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseIsPressed)
            {
                int margin = 1500;
                Objects.Location newLoc = new Objects.Location(this.CurrentLocation.X + this.mouseOldPosition.X - e.Location.X,
                    this.CurrentLocation.Y + this.mouseOldPosition.Y - e.Location.Y, this.CurrentLocation.Z);
                if (newLoc.X > 0 - margin && newLoc.Y > 0 - margin &&
                    newLoc.X < this.CurrentFloorBounds.Width + margin && newLoc.Y < this.CurrentFloorBounds.Height + margin)
                {
                    this.CurrentLocation = newLoc;
                    this.mouseOldPosition = e.Location;
                    picboxMap.Invalidate();
                }
            }
            else
            {
                Objects.Location worldLocation = this.CurrentLocation.Offset((int)(e.Location.X / this.Scale),
                    (int)(e.Location.Y / this.Scale), 0);
                worldLocation.X += this.CurrentFloorBounds.X;
                worldLocation.Y += this.CurrentFloorBounds.Y;
                this.hoveredWaypoint = null;
                foreach (var wp in this.Client.Modules.Cavebot.GetWaypoints())
                {
                    if (worldLocation.DistanceTo(wp.Location) < 5)
                    {
                        this.hoveredWaypoint = wp;
                        //picboxMap.Invalidate();
                        break;
                    }
                }
            }
        }

        private void picboxMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.mouseOldPosition = e.Location;
                this.mouseIsPressed = true;
            }
        }

        private void picboxMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.mouseIsPressed = false;
            }
        }

        private void MapViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
        }

        private void contextitemAddWaypointNode_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Node, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointWalk_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Walk, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointRope_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Rope, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointShovel_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Shovel, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointLadder_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Ladder, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointMachete_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Machete, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointScript_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Script, this.mouseRightClickPosition);
        }

        private void contextitemAddWaypointPick_Click(object sender, EventArgs e)
        {
            this.AddWaypoint(Modules.Cavebot.Waypoint.Types.Pick, this.mouseRightClickPosition);
        }

        private void AddWaypoint(Modules.Cavebot.Waypoint.Types waypointType, Point location)
        {
            Objects.Location worldLocation = this.CurrentLocation.Offset((int)(location.X / this.Scale), (int)(location.Y / this.Scale), 0);
            worldLocation.X += this.CurrentFloorBounds.X;
            worldLocation.Y += this.CurrentFloorBounds.Y;
            this.Client.Modules.Cavebot.AddWaypoint(new Modules.Cavebot.Waypoint(this.Client.Modules.Cavebot, worldLocation, waypointType));
            this.picboxMap.Invalidate();
        }
    }
}
