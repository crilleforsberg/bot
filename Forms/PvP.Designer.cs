namespace KarelazisBot.Forms
{
    partial class PvP
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtboxCombobotClientActionEx = new System.Windows.Forms.TextBox();
            this.comboboxCombobotClientAction = new System.Windows.Forms.ComboBox();
            this.btnCombobotClientConnect = new System.Windows.Forms.Button();
            this.txtboxCombobotClientIPPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCombobotServerTotal = new System.Windows.Forms.Label();
            this.btnCombobotServerStart = new System.Windows.Forms.Button();
            this.listboxCombobotServerConnectedCharacters = new System.Windows.Forms.ListBox();
            this.numericCombobotServerPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCombobotServerGlobalIP = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericCombobotServerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(292, 262);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(284, 236);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Combobot";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtboxCombobotClientActionEx);
            this.groupBox2.Controls.Add(this.comboboxCombobotClientAction);
            this.groupBox2.Controls.Add(this.btnCombobotClientConnect);
            this.groupBox2.Controls.Add(this.txtboxCombobotClientIPPort);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(278, 98);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Client";
            // 
            // txtboxCombobotClientActionEx
            // 
            this.txtboxCombobotClientActionEx.Location = new System.Drawing.Point(9, 68);
            this.txtboxCombobotClientActionEx.Name = "txtboxCombobotClientActionEx";
            this.txtboxCombobotClientActionEx.Size = new System.Drawing.Size(86, 22);
            this.txtboxCombobotClientActionEx.TabIndex = 9;
            this.txtboxCombobotClientActionEx.TextChanged += new System.EventHandler(this.txtboxCombobotClientActionEx_TextChanged);
            // 
            // comboboxCombobotClientAction
            // 
            this.comboboxCombobotClientAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxCombobotClientAction.FormattingEnabled = true;
            this.comboboxCombobotClientAction.Items.AddRange(new object[] {
            "Say",
            "Custom rune",
            "SD",
            "HMM",
            "Explosion",
            "Paralyze"});
            this.comboboxCombobotClientAction.Location = new System.Drawing.Point(9, 43);
            this.comboboxCombobotClientAction.Name = "comboboxCombobotClientAction";
            this.comboboxCombobotClientAction.Size = new System.Drawing.Size(86, 21);
            this.comboboxCombobotClientAction.TabIndex = 8;
            this.comboboxCombobotClientAction.SelectedIndexChanged += new System.EventHandler(this.comboboxCombobotClientAction_SelectedIndexChanged);
            // 
            // btnCombobotClientConnect
            // 
            this.btnCombobotClientConnect.Location = new System.Drawing.Point(101, 43);
            this.btnCombobotClientConnect.Name = "btnCombobotClientConnect";
            this.btnCombobotClientConnect.Size = new System.Drawing.Size(171, 47);
            this.btnCombobotClientConnect.TabIndex = 6;
            this.btnCombobotClientConnect.Text = "Connect";
            this.btnCombobotClientConnect.UseVisualStyleBackColor = true;
            this.btnCombobotClientConnect.Click += new System.EventHandler(this.btnCombobotClientConnect_Click);
            // 
            // txtboxCombobotClientIPPort
            // 
            this.txtboxCombobotClientIPPort.Location = new System.Drawing.Point(101, 15);
            this.txtboxCombobotClientIPPort.Name = "txtboxCombobotClientIPPort";
            this.txtboxCombobotClientIPPort.Size = new System.Drawing.Size(171, 22);
            this.txtboxCombobotClientIPPort.TabIndex = 7;
            this.txtboxCombobotClientIPPort.Text = "127.0.0.1:3000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server IP/port:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCombobotServerTotal);
            this.groupBox1.Controls.Add(this.btnCombobotServerStart);
            this.groupBox1.Controls.Add(this.listboxCombobotServerConnectedCharacters);
            this.groupBox1.Controls.Add(this.numericCombobotServerPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblCombobotServerGlobalIP);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // lblCombobotServerTotal
            // 
            this.lblCombobotServerTotal.AutoSize = true;
            this.lblCombobotServerTotal.Location = new System.Drawing.Point(6, 113);
            this.lblCombobotServerTotal.Name = "lblCombobotServerTotal";
            this.lblCombobotServerTotal.Size = new System.Drawing.Size(103, 13);
            this.lblCombobotServerTotal.TabIndex = 5;
            this.lblCombobotServerTotal.Text = "Total connected: --";
            // 
            // btnCombobotServerStart
            // 
            this.btnCombobotServerStart.Location = new System.Drawing.Point(6, 62);
            this.btnCombobotServerStart.Name = "btnCombobotServerStart";
            this.btnCombobotServerStart.Size = new System.Drawing.Size(114, 48);
            this.btnCombobotServerStart.TabIndex = 4;
            this.btnCombobotServerStart.Text = "Start server";
            this.btnCombobotServerStart.UseVisualStyleBackColor = true;
            this.btnCombobotServerStart.Click += new System.EventHandler(this.btnCombobotServerStart_Click);
            // 
            // listboxCombobotServerConnectedCharacters
            // 
            this.listboxCombobotServerConnectedCharacters.FormattingEnabled = true;
            this.listboxCombobotServerConnectedCharacters.Location = new System.Drawing.Point(126, 18);
            this.listboxCombobotServerConnectedCharacters.Name = "listboxCombobotServerConnectedCharacters";
            this.listboxCombobotServerConnectedCharacters.Size = new System.Drawing.Size(146, 108);
            this.listboxCombobotServerConnectedCharacters.TabIndex = 3;
            // 
            // numericCombobotServerPort
            // 
            this.numericCombobotServerPort.Location = new System.Drawing.Point(47, 34);
            this.numericCombobotServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericCombobotServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCombobotServerPort.Name = "numericCombobotServerPort";
            this.numericCombobotServerPort.Size = new System.Drawing.Size(71, 22);
            this.numericCombobotServerPort.TabIndex = 2;
            this.numericCombobotServerPort.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            // 
            // lblCombobotServerGlobalIP
            // 
            this.lblCombobotServerGlobalIP.AutoSize = true;
            this.lblCombobotServerGlobalIP.Location = new System.Drawing.Point(2, 18);
            this.lblCombobotServerGlobalIP.Name = "lblCombobotServerGlobalIP";
            this.lblCombobotServerGlobalIP.Size = new System.Drawing.Size(56, 13);
            this.lblCombobotServerGlobalIP.TabIndex = 0;
            this.lblCombobotServerGlobalIP.Text = "Your IP: --";
            // 
            // PvP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 262);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PvP";
            this.Text = "PvP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PvP_FormClosing);
            this.Shown += new System.EventHandler(this.PvP_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericCombobotServerPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtboxCombobotClientIPPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCombobotServerTotal;
        private System.Windows.Forms.Button btnCombobotServerStart;
        private System.Windows.Forms.ListBox listboxCombobotServerConnectedCharacters;
        private System.Windows.Forms.NumericUpDown numericCombobotServerPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCombobotServerGlobalIP;
        private System.Windows.Forms.TextBox txtboxCombobotClientActionEx;
        private System.Windows.Forms.ComboBox comboboxCombobotClientAction;
        private System.Windows.Forms.Button btnCombobotClientConnect;
    }
}