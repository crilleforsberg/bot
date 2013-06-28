namespace KarelazisBot.Forms
{
    partial class MainHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainHub));
            this.menustripTop = new System.Windows.Forms.MenuStrip();
            this.menutoolstripFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuAlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menutoolstripHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextmenuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolstriptrayShowHub = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowCavebot = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowHealer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowHotkeys = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowMagebomb = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowPvP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripShowScripter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolstriptrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMapViewer = new System.Windows.Forms.Button();
            this.btnScripter = new System.Windows.Forms.Button();
            this.btnPvP = new System.Windows.Forms.Button();
            this.btnInformation = new System.Windows.Forms.Button();
            this.btnHotkeys = new System.Windows.Forms.Button();
            this.btnHealer = new System.Windows.Forms.Button();
            this.btnCavebot = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.menustripTop.SuspendLayout();
            this.contextmenuTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // menustripTop
            // 
            this.menustripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menutoolstripFile,
            this.menutoolstripHelp});
            this.menustripTop.Location = new System.Drawing.Point(0, 0);
            this.menustripTop.Name = "menustripTop";
            this.menustripTop.Size = new System.Drawing.Size(151, 24);
            this.menustripTop.TabIndex = 2;
            // 
            // menutoolstripFile
            // 
            this.menutoolstripFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuAlwaysOnTop,
            this.toolstripmenuExit});
            this.menutoolstripFile.Name = "menutoolstripFile";
            this.menutoolstripFile.Size = new System.Drawing.Size(37, 20);
            this.menutoolstripFile.Text = "File";
            // 
            // toolstripmenuAlwaysOnTop
            // 
            this.toolstripmenuAlwaysOnTop.Name = "toolstripmenuAlwaysOnTop";
            this.toolstripmenuAlwaysOnTop.Size = new System.Drawing.Size(149, 22);
            this.toolstripmenuAlwaysOnTop.Text = "Always on top";
            this.toolstripmenuAlwaysOnTop.Click += new System.EventHandler(this.toolstripmenuAlwaysOnTop_Click);
            // 
            // toolstripmenuExit
            // 
            this.toolstripmenuExit.Name = "toolstripmenuExit";
            this.toolstripmenuExit.Size = new System.Drawing.Size(149, 22);
            this.toolstripmenuExit.Text = "Exit";
            this.toolstripmenuExit.Click += new System.EventHandler(this.toolstripmenuExit_Click);
            // 
            // menutoolstripHelp
            // 
            this.menutoolstripHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuCheckForUpdates});
            this.menutoolstripHelp.Name = "menutoolstripHelp";
            this.menutoolstripHelp.Size = new System.Drawing.Size(44, 20);
            this.menutoolstripHelp.Text = "Help";
            // 
            // toolstripmenuCheckForUpdates
            // 
            this.toolstripmenuCheckForUpdates.Name = "toolstripmenuCheckForUpdates";
            this.toolstripmenuCheckForUpdates.Size = new System.Drawing.Size(179, 22);
            this.toolstripmenuCheckForUpdates.Text = "Check for updates...";
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.contextmenuTray;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // contextmenuTray
            // 
            this.contextmenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstriptrayShowHub,
            this.toolstripShow,
            this.toolStripSeparator2,
            this.toolstriptrayExit});
            this.contextmenuTray.Name = "contextmenuTray";
            this.contextmenuTray.ShowImageMargin = false;
            this.contextmenuTray.Size = new System.Drawing.Size(103, 76);
            // 
            // toolstriptrayShowHub
            // 
            this.toolstriptrayShowHub.Name = "toolstriptrayShowHub";
            this.toolstriptrayShowHub.Size = new System.Drawing.Size(102, 22);
            this.toolstriptrayShowHub.Text = "Show hub";
            this.toolstriptrayShowHub.Click += new System.EventHandler(this.toolstriptrayShowHub_Click);
            // 
            // toolstripShow
            // 
            this.toolstripShow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripShowCavebot,
            this.toolstripShowHealer,
            this.toolstripShowHotkeys,
            this.toolstripShowInformation,
            this.toolstripShowMagebomb,
            this.toolstripShowPvP,
            this.toolstripShowScripter});
            this.toolstripShow.Name = "toolstripShow";
            this.toolstripShow.Size = new System.Drawing.Size(102, 22);
            this.toolstripShow.Text = "Show";
            // 
            // toolstripShowCavebot
            // 
            this.toolstripShowCavebot.Name = "toolstripShowCavebot";
            this.toolstripShowCavebot.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowCavebot.Text = "Cavebot";
            // 
            // toolstripShowHealer
            // 
            this.toolstripShowHealer.Name = "toolstripShowHealer";
            this.toolstripShowHealer.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowHealer.Text = "Healer";
            // 
            // toolstripShowHotkeys
            // 
            this.toolstripShowHotkeys.Name = "toolstripShowHotkeys";
            this.toolstripShowHotkeys.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowHotkeys.Text = "Hotkeys";
            // 
            // toolstripShowInformation
            // 
            this.toolstripShowInformation.Name = "toolstripShowInformation";
            this.toolstripShowInformation.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowInformation.Text = "Information";
            // 
            // toolstripShowMagebomb
            // 
            this.toolstripShowMagebomb.Name = "toolstripShowMagebomb";
            this.toolstripShowMagebomb.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowMagebomb.Text = "Magebomb";
            // 
            // toolstripShowPvP
            // 
            this.toolstripShowPvP.Name = "toolstripShowPvP";
            this.toolstripShowPvP.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowPvP.Text = "PvP";
            // 
            // toolstripShowScripter
            // 
            this.toolstripShowScripter.Name = "toolstripShowScripter";
            this.toolstripShowScripter.Size = new System.Drawing.Size(137, 22);
            this.toolstripShowScripter.Text = "Scripter";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(99, 6);
            // 
            // toolstriptrayExit
            // 
            this.toolstriptrayExit.Name = "toolstriptrayExit";
            this.toolstriptrayExit.Size = new System.Drawing.Size(102, 22);
            this.toolstriptrayExit.Text = "Exit";
            this.toolstriptrayExit.Click += new System.EventHandler(this.toolstriptrayExit_Click);
            // 
            // btnMapViewer
            // 
            this.btnMapViewer.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnMapViewer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMapViewer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMapViewer.Image = global::KarelazisBot.Properties.Resources.map;
            this.btnMapViewer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMapViewer.Location = new System.Drawing.Point(0, 264);
            this.btnMapViewer.Name = "btnMapViewer";
            this.btnMapViewer.Size = new System.Drawing.Size(151, 40);
            this.btnMapViewer.TabIndex = 7;
            this.btnMapViewer.TabStop = false;
            this.btnMapViewer.Text = "Map Viewer";
            this.btnMapViewer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMapViewer.UseVisualStyleBackColor = false;
            this.btnMapViewer.Click += new System.EventHandler(this.btnMapViewer_Click);
            // 
            // btnScripter
            // 
            this.btnScripter.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnScripter.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnScripter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScripter.Image = global::KarelazisBot.Properties.Resources.scroll;
            this.btnScripter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnScripter.Location = new System.Drawing.Point(0, 224);
            this.btnScripter.Name = "btnScripter";
            this.btnScripter.Size = new System.Drawing.Size(151, 40);
            this.btnScripter.TabIndex = 6;
            this.btnScripter.TabStop = false;
            this.btnScripter.Text = "Scripter";
            this.btnScripter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnScripter.UseVisualStyleBackColor = false;
            this.btnScripter.Click += new System.EventHandler(this.btnScripter_Click);
            // 
            // btnPvP
            // 
            this.btnPvP.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnPvP.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPvP.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPvP.Image = global::KarelazisBot.Properties.Resources.SuddenDeath;
            this.btnPvP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPvP.Location = new System.Drawing.Point(0, 184);
            this.btnPvP.Name = "btnPvP";
            this.btnPvP.Size = new System.Drawing.Size(151, 40);
            this.btnPvP.TabIndex = 5;
            this.btnPvP.TabStop = false;
            this.btnPvP.Text = "PvP";
            this.btnPvP.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPvP.UseVisualStyleBackColor = false;
            this.btnPvP.Click += new System.EventHandler(this.btnPvP_Click);
            // 
            // btnInformation
            // 
            this.btnInformation.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnInformation.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInformation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInformation.Image = global::KarelazisBot.Properties.Resources.Tome_of_Knowledge;
            this.btnInformation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInformation.Location = new System.Drawing.Point(0, 144);
            this.btnInformation.Name = "btnInformation";
            this.btnInformation.Size = new System.Drawing.Size(151, 40);
            this.btnInformation.TabIndex = 3;
            this.btnInformation.TabStop = false;
            this.btnInformation.Text = "Information";
            this.btnInformation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInformation.UseVisualStyleBackColor = false;
            this.btnInformation.Click += new System.EventHandler(this.btnInformation_Click);
            // 
            // btnHotkeys
            // 
            this.btnHotkeys.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnHotkeys.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHotkeys.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHotkeys.Image = global::KarelazisBot.Properties.Resources.key;
            this.btnHotkeys.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHotkeys.Location = new System.Drawing.Point(0, 104);
            this.btnHotkeys.Name = "btnHotkeys";
            this.btnHotkeys.Size = new System.Drawing.Size(151, 40);
            this.btnHotkeys.TabIndex = 2;
            this.btnHotkeys.TabStop = false;
            this.btnHotkeys.Text = "Hotkeys";
            this.btnHotkeys.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHotkeys.UseVisualStyleBackColor = false;
            this.btnHotkeys.Click += new System.EventHandler(this.btnHotkeys_Click);
            // 
            // btnHealer
            // 
            this.btnHealer.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnHealer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHealer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHealer.Image = global::KarelazisBot.Properties.Resources.firstaid;
            this.btnHealer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHealer.Location = new System.Drawing.Point(0, 64);
            this.btnHealer.Name = "btnHealer";
            this.btnHealer.Size = new System.Drawing.Size(151, 40);
            this.btnHealer.TabIndex = 1;
            this.btnHealer.TabStop = false;
            this.btnHealer.Text = "Healer";
            this.btnHealer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHealer.UseVisualStyleBackColor = false;
            this.btnHealer.Click += new System.EventHandler(this.btnHealer_Click);
            // 
            // btnCavebot
            // 
            this.btnCavebot.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCavebot.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCavebot.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCavebot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCavebot.Image = global::KarelazisBot.Properties.Resources.skull_staff;
            this.btnCavebot.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCavebot.Location = new System.Drawing.Point(0, 24);
            this.btnCavebot.Name = "btnCavebot";
            this.btnCavebot.Size = new System.Drawing.Size(151, 40);
            this.btnCavebot.TabIndex = 0;
            this.btnCavebot.TabStop = false;
            this.btnCavebot.Text = "Cavebot";
            this.btnCavebot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCavebot.UseVisualStyleBackColor = false;
            this.btnCavebot.Click += new System.EventHandler(this.btnCavebot_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // MainHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(151, 304);
            this.Controls.Add(this.btnMapViewer);
            this.Controls.Add(this.btnScripter);
            this.Controls.Add(this.btnPvP);
            this.Controls.Add(this.btnInformation);
            this.Controls.Add(this.btnHotkeys);
            this.Controls.Add(this.btnHealer);
            this.Controls.Add(this.btnCavebot);
            this.Controls.Add(this.menustripTop);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menustripTop;
            this.MaximizeBox = false;
            this.Name = "MainHub";
            this.Text = "Karelazi\'s Bot 2.13";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainHub_FormClosing);
            this.Shown += new System.EventHandler(this.MainHub_Shown);
            this.menustripTop.ResumeLayout(false);
            this.menustripTop.PerformLayout();
            this.contextmenuTray.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCavebot;
        private System.Windows.Forms.Button btnHealer;
        private System.Windows.Forms.MenuStrip menustripTop;
        private System.Windows.Forms.ToolStripMenuItem menutoolstripFile;
        private System.Windows.Forms.ToolStripMenuItem menutoolstripHelp;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuExit;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuCheckForUpdates;
        private System.Windows.Forms.Button btnHotkeys;
        private System.Windows.Forms.Button btnInformation;
        private System.Windows.Forms.Button btnPvP;
        private System.Windows.Forms.Button btnScripter;
        private System.Windows.Forms.ContextMenuStrip contextmenuTray;
        private System.Windows.Forms.ToolStripMenuItem toolstriptrayExit;
        private System.Windows.Forms.ToolStripMenuItem toolstriptrayShowHub;
        private System.Windows.Forms.ToolStripMenuItem toolstripShow;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowCavebot;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowHealer;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowHotkeys;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowInformation;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowMagebomb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowPvP;
        private System.Windows.Forms.ToolStripMenuItem toolstripShowScripter;
        private System.Windows.Forms.Button btnMapViewer;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuAlwaysOnTop;
        private System.Windows.Forms.Timer timerUpdate;
    }
}

