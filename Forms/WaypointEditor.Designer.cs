namespace KarelazisBot.Forms
{
    partial class WaypointEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboboxType = new System.Windows.Forms.ComboBox();
            this.numericLocX = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericLocY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericLocZ = new System.Windows.Forms.NumericUpDown();
            this.comboboxWaypoint = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPlayerLocation = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numericLocX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLocY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLocZ)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type:";
            // 
            // comboboxType
            // 
            this.comboboxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxType.FormattingEnabled = true;
            this.comboboxType.Items.AddRange(new object[] {
            "Node",
            "Walk",
            "Rope",
            "Shovel",
            "Ladder",
            "Pick",
            "Machete",
            "Script"});
            this.comboboxType.Location = new System.Drawing.Point(72, 38);
            this.comboboxType.Name = "comboboxType";
            this.comboboxType.Size = new System.Drawing.Size(116, 21);
            this.comboboxType.TabIndex = 1;
            this.comboboxType.SelectedIndexChanged += new System.EventHandler(this.comboboxType_SelectedIndexChanged);
            // 
            // numericLocX
            // 
            this.numericLocX.Location = new System.Drawing.Point(12, 80);
            this.numericLocX.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericLocX.Name = "numericLocX";
            this.numericLocX.Size = new System.Drawing.Size(70, 22);
            this.numericLocX.TabIndex = 2;
            this.numericLocX.ValueChanged += new System.EventHandler(this.numericLocX_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y";
            // 
            // numericLocY
            // 
            this.numericLocY.Location = new System.Drawing.Point(88, 80);
            this.numericLocY.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericLocY.Name = "numericLocY";
            this.numericLocY.Size = new System.Drawing.Size(70, 22);
            this.numericLocY.TabIndex = 4;
            this.numericLocY.ValueChanged += new System.EventHandler(this.numericLocY_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(173, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Z";
            // 
            // numericLocZ
            // 
            this.numericLocZ.Location = new System.Drawing.Point(164, 80);
            this.numericLocZ.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericLocZ.Name = "numericLocZ";
            this.numericLocZ.Size = new System.Drawing.Size(30, 22);
            this.numericLocZ.TabIndex = 6;
            this.numericLocZ.ValueChanged += new System.EventHandler(this.numericLocZ_ValueChanged);
            // 
            // comboboxWaypoint
            // 
            this.comboboxWaypoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxWaypoint.FormattingEnabled = true;
            this.comboboxWaypoint.Location = new System.Drawing.Point(72, 11);
            this.comboboxWaypoint.Name = "comboboxWaypoint";
            this.comboboxWaypoint.Size = new System.Drawing.Size(116, 21);
            this.comboboxWaypoint.TabIndex = 9;
            this.comboboxWaypoint.SelectedIndexChanged += new System.EventHandler(this.comboboxWaypoint_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Waypoint:";
            // 
            // lblPlayerLocation
            // 
            this.lblPlayerLocation.AutoSize = true;
            this.lblPlayerLocation.Location = new System.Drawing.Point(9, 108);
            this.lblPlayerLocation.Name = "lblPlayerLocation";
            this.lblPlayerLocation.Size = new System.Drawing.Size(96, 13);
            this.lblPlayerLocation.TabIndex = 11;
            this.lblPlayerLocation.Text = "Player location: --";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 500;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // WaypointEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 127);
            this.Controls.Add(this.lblPlayerLocation);
            this.Controls.Add(this.comboboxWaypoint);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericLocZ);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericLocY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericLocX);
            this.Controls.Add(this.comboboxType);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WaypointEditor";
            this.Text = "Waypoint Editor";
            ((System.ComponentModel.ISupportInitialize)(this.numericLocX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLocY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLocZ)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboboxType;
        private System.Windows.Forms.NumericUpDown numericLocX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericLocY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericLocZ;
        private System.Windows.Forms.ComboBox comboboxWaypoint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPlayerLocation;
        private System.Windows.Forms.Timer timerUpdate;
    }
}