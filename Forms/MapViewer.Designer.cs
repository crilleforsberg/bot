namespace KarelazisBot.Forms
{
    partial class MapViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.checkboxDrawCreatures = new System.Windows.Forms.CheckBox();
            this.checkboxDrawPlayersNPCs = new System.Windows.Forms.CheckBox();
            this.checkboxDrawWaypointPaths = new System.Windows.Forms.CheckBox();
            this.contextmenuMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextitemResetPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemFloorUp = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemFloorDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextitemWaypointAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointNode = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointWalk = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointRope = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointShovel = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointLadder = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointMachete = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointScript = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemAddWaypointPick = new System.Windows.Forms.ToolStripMenuItem();
            this.contextitemWaypointRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkboxLockToPlayer = new System.Windows.Forms.CheckBox();
            this.timerUpdatePanel = new System.Windows.Forms.Timer(this.components);
            this.picboxMap = new System.Windows.Forms.PictureBox();
            this.contextmenuMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picboxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // checkboxDrawCreatures
            // 
            this.checkboxDrawCreatures.AutoSize = true;
            this.checkboxDrawCreatures.Checked = true;
            this.checkboxDrawCreatures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxDrawCreatures.Location = new System.Drawing.Point(12, 353);
            this.checkboxDrawCreatures.Name = "checkboxDrawCreatures";
            this.checkboxDrawCreatures.Size = new System.Drawing.Size(103, 17);
            this.checkboxDrawCreatures.TabIndex = 1;
            this.checkboxDrawCreatures.Text = "Draw creatures";
            this.checkboxDrawCreatures.UseVisualStyleBackColor = true;
            // 
            // checkboxDrawPlayersNPCs
            // 
            this.checkboxDrawPlayersNPCs.AutoSize = true;
            this.checkboxDrawPlayersNPCs.Checked = true;
            this.checkboxDrawPlayersNPCs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxDrawPlayersNPCs.Location = new System.Drawing.Point(121, 353);
            this.checkboxDrawPlayersNPCs.Name = "checkboxDrawPlayersNPCs";
            this.checkboxDrawPlayersNPCs.Size = new System.Drawing.Size(122, 17);
            this.checkboxDrawPlayersNPCs.TabIndex = 2;
            this.checkboxDrawPlayersNPCs.Text = "Draw players/NPCs";
            this.checkboxDrawPlayersNPCs.UseVisualStyleBackColor = true;
            // 
            // checkboxDrawWaypointPaths
            // 
            this.checkboxDrawWaypointPaths.AutoSize = true;
            this.checkboxDrawWaypointPaths.Checked = true;
            this.checkboxDrawWaypointPaths.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxDrawWaypointPaths.Location = new System.Drawing.Point(249, 353);
            this.checkboxDrawWaypointPaths.Name = "checkboxDrawWaypointPaths";
            this.checkboxDrawWaypointPaths.Size = new System.Drawing.Size(136, 17);
            this.checkboxDrawWaypointPaths.TabIndex = 3;
            this.checkboxDrawWaypointPaths.Text = "Draw waypoint paths";
            this.checkboxDrawWaypointPaths.UseVisualStyleBackColor = true;
            // 
            // contextmenuMap
            // 
            this.contextmenuMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextitemResetPosition,
            this.contextitemFloorUp,
            this.contextitemFloorDown,
            this.toolStripSeperator2,
            this.contextitemWaypointAdd,
            this.contextitemWaypointRemove,
            this.toolStripSeparator1});
            this.contextmenuMap.Name = "contextmenuMap";
            this.contextmenuMap.ShowImageMargin = false;
            this.contextmenuMap.Size = new System.Drawing.Size(145, 148);
            // 
            // contextitemResetPosition
            // 
            this.contextitemResetPosition.Name = "contextitemResetPosition";
            this.contextitemResetPosition.Size = new System.Drawing.Size(144, 22);
            this.contextitemResetPosition.Text = "Reset position";
            this.contextitemResetPosition.Click += new System.EventHandler(this.contextitemResetPosition_Click);
            // 
            // contextitemFloorUp
            // 
            this.contextitemFloorUp.Name = "contextitemFloorUp";
            this.contextitemFloorUp.Size = new System.Drawing.Size(144, 22);
            this.contextitemFloorUp.Text = "Floor up";
            this.contextitemFloorUp.Click += new System.EventHandler(this.contextitemFloorUp_Click);
            // 
            // contextitemFloorDown
            // 
            this.contextitemFloorDown.Name = "contextitemFloorDown";
            this.contextitemFloorDown.Size = new System.Drawing.Size(144, 22);
            this.contextitemFloorDown.Text = "Floor down";
            this.contextitemFloorDown.Click += new System.EventHandler(this.contextitemFloorDown_Click);
            // 
            // toolStripSeperator2
            // 
            this.toolStripSeperator2.Name = "toolStripSeperator2";
            this.toolStripSeperator2.Size = new System.Drawing.Size(141, 6);
            // 
            // contextitemWaypointAdd
            // 
            this.contextitemWaypointAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextitemAddWaypointNode,
            this.contextitemAddWaypointWalk,
            this.contextitemAddWaypointRope,
            this.contextitemAddWaypointShovel,
            this.contextitemAddWaypointLadder,
            this.contextitemAddWaypointMachete,
            this.contextitemAddWaypointScript,
            this.contextitemAddWaypointPick});
            this.contextitemWaypointAdd.Name = "contextitemWaypointAdd";
            this.contextitemWaypointAdd.Size = new System.Drawing.Size(144, 22);
            this.contextitemWaypointAdd.Text = "Add waypoint";
            // 
            // contextitemAddWaypointNode
            // 
            this.contextitemAddWaypointNode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.contextitemAddWaypointNode.Name = "contextitemAddWaypointNode";
            this.contextitemAddWaypointNode.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointNode.Text = "Node";
            this.contextitemAddWaypointNode.Click += new System.EventHandler(this.contextitemAddWaypointNode_Click);
            // 
            // contextitemAddWaypointWalk
            // 
            this.contextitemAddWaypointWalk.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.contextitemAddWaypointWalk.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.contextitemAddWaypointWalk.Name = "contextitemAddWaypointWalk";
            this.contextitemAddWaypointWalk.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointWalk.Text = "Walk";
            this.contextitemAddWaypointWalk.Click += new System.EventHandler(this.contextitemAddWaypointWalk_Click);
            // 
            // contextitemAddWaypointRope
            // 
            this.contextitemAddWaypointRope.Name = "contextitemAddWaypointRope";
            this.contextitemAddWaypointRope.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointRope.Text = "Rope";
            this.contextitemAddWaypointRope.Click += new System.EventHandler(this.contextitemAddWaypointRope_Click);
            // 
            // contextitemAddWaypointShovel
            // 
            this.contextitemAddWaypointShovel.Name = "contextitemAddWaypointShovel";
            this.contextitemAddWaypointShovel.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointShovel.Text = "Shovel";
            this.contextitemAddWaypointShovel.Click += new System.EventHandler(this.contextitemAddWaypointShovel_Click);
            // 
            // contextitemAddWaypointLadder
            // 
            this.contextitemAddWaypointLadder.Name = "contextitemAddWaypointLadder";
            this.contextitemAddWaypointLadder.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointLadder.Text = "Ladder";
            this.contextitemAddWaypointLadder.Click += new System.EventHandler(this.contextitemAddWaypointLadder_Click);
            // 
            // contextitemAddWaypointMachete
            // 
            this.contextitemAddWaypointMachete.Name = "contextitemAddWaypointMachete";
            this.contextitemAddWaypointMachete.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointMachete.Text = "Machete";
            this.contextitemAddWaypointMachete.Click += new System.EventHandler(this.contextitemAddWaypointMachete_Click);
            // 
            // contextitemAddWaypointScript
            // 
            this.contextitemAddWaypointScript.Name = "contextitemAddWaypointScript";
            this.contextitemAddWaypointScript.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointScript.Text = "Script";
            this.contextitemAddWaypointScript.Click += new System.EventHandler(this.contextitemAddWaypointScript_Click);
            // 
            // contextitemAddWaypointPick
            // 
            this.contextitemAddWaypointPick.Name = "contextitemAddWaypointPick";
            this.contextitemAddWaypointPick.Size = new System.Drawing.Size(120, 22);
            this.contextitemAddWaypointPick.Text = "Pick";
            this.contextitemAddWaypointPick.Click += new System.EventHandler(this.contextitemAddWaypointPick_Click);
            // 
            // contextitemWaypointRemove
            // 
            this.contextitemWaypointRemove.Enabled = false;
            this.contextitemWaypointRemove.Name = "contextitemWaypointRemove";
            this.contextitemWaypointRemove.Size = new System.Drawing.Size(144, 22);
            this.contextitemWaypointRemove.Text = "Remove waypoint";
            this.contextitemWaypointRemove.Click += new System.EventHandler(this.contextitemWaypointRemove_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // checkboxLockToPlayer
            // 
            this.checkboxLockToPlayer.AutoSize = true;
            this.checkboxLockToPlayer.Checked = true;
            this.checkboxLockToPlayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxLockToPlayer.Location = new System.Drawing.Point(391, 353);
            this.checkboxLockToPlayer.Name = "checkboxLockToPlayer";
            this.checkboxLockToPlayer.Size = new System.Drawing.Size(97, 17);
            this.checkboxLockToPlayer.TabIndex = 5;
            this.checkboxLockToPlayer.Text = "Lock to player";
            this.checkboxLockToPlayer.UseVisualStyleBackColor = true;
            // 
            // timerUpdatePanel
            // 
            this.timerUpdatePanel.Interval = 1000;
            this.timerUpdatePanel.Tick += new System.EventHandler(this.timerUpdatePanel_Tick);
            // 
            // picboxMap
            // 
            this.picboxMap.BackColor = System.Drawing.Color.Black;
            this.picboxMap.Dock = System.Windows.Forms.DockStyle.Top;
            this.picboxMap.Location = new System.Drawing.Point(0, 0);
            this.picboxMap.Name = "picboxMap";
            this.picboxMap.Size = new System.Drawing.Size(492, 347);
            this.picboxMap.TabIndex = 6;
            this.picboxMap.TabStop = false;
            this.picboxMap.Paint += new System.Windows.Forms.PaintEventHandler(this.picboxMap_Paint);
            this.picboxMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picboxMap_MouseClick);
            this.picboxMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picboxMap_MouseDown);
            this.picboxMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picboxMap_MouseMove);
            this.picboxMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picboxMap_MouseUp);
            // 
            // MapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 373);
            this.Controls.Add(this.checkboxDrawCreatures);
            this.Controls.Add(this.checkboxLockToPlayer);
            this.Controls.Add(this.picboxMap);
            this.Controls.Add(this.checkboxDrawWaypointPaths);
            this.Controls.Add(this.checkboxDrawPlayersNPCs);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MapViewer";
            this.Text = "Map Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapViewer_FormClosing);
            this.Shown += new System.EventHandler(this.MapViewer_Shown);
            this.Resize += new System.EventHandler(this.MapViewer_Resize);
            this.contextmenuMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picboxMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkboxDrawCreatures;
        private System.Windows.Forms.CheckBox checkboxDrawPlayersNPCs;
        private System.Windows.Forms.CheckBox checkboxDrawWaypointPaths;
        private System.Windows.Forms.ContextMenuStrip contextmenuMap;
        private System.Windows.Forms.ToolStripMenuItem contextitemResetPosition;
        private System.Windows.Forms.ToolStripSeparator toolStripSeperator2;
        private System.Windows.Forms.ToolStripMenuItem contextitemWaypointAdd;
        private System.Windows.Forms.ToolStripMenuItem contextitemWaypointRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckBox checkboxLockToPlayer;
        private System.Windows.Forms.Timer timerUpdatePanel;
        private System.Windows.Forms.ToolStripMenuItem contextitemFloorUp;
        private System.Windows.Forms.ToolStripMenuItem contextitemFloorDown;
        private System.Windows.Forms.PictureBox picboxMap;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointNode;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointWalk;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointRope;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointShovel;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointMachete;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointLadder;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointScript;
        private System.Windows.Forms.ToolStripMenuItem contextitemAddWaypointPick;
    }
}