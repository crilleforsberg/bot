namespace KarelazisBot.Forms
{
    partial class Hotkeys
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.datagridHotkeys = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtboxKey = new System.Windows.Forms.TextBox();
            this.txtboxScriptPath = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.checkboxHotkeysStatus = new System.Windows.Forms.CheckBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.timerStatusWatcher = new System.Windows.Forms.Timer(this.components);
            this.contextmenuHotkeys = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolstripRemove = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.datagridHotkeys)).BeginInit();
            this.contextmenuHotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // datagridHotkeys
            // 
            this.datagridHotkeys.AllowUserToAddRows = false;
            this.datagridHotkeys.AllowUserToDeleteRows = false;
            this.datagridHotkeys.AllowUserToResizeColumns = false;
            this.datagridHotkeys.AllowUserToResizeRows = false;
            this.datagridHotkeys.BackgroundColor = System.Drawing.Color.White;
            this.datagridHotkeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridHotkeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.datagridHotkeys.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.datagridHotkeys.Location = new System.Drawing.Point(0, 78);
            this.datagridHotkeys.MultiSelect = false;
            this.datagridHotkeys.Name = "datagridHotkeys";
            this.datagridHotkeys.ReadOnly = true;
            this.datagridHotkeys.RowHeadersVisible = false;
            this.datagridHotkeys.RowHeadersWidth = 24;
            this.datagridHotkeys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridHotkeys.Size = new System.Drawing.Size(241, 273);
            this.datagridHotkeys.TabIndex = 49;
            this.datagridHotkeys.KeyUp += new System.Windows.Forms.KeyEventHandler(this.datagridHotkeys_KeyUp);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn5.HeaderText = "Key";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 75;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn6.HeaderText = "ScriptPath";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // txtboxKey
            // 
            this.txtboxKey.BackColor = System.Drawing.SystemColors.Window;
            this.txtboxKey.Location = new System.Drawing.Point(0, 54);
            this.txtboxKey.Name = "txtboxKey";
            this.txtboxKey.ReadOnly = true;
            this.txtboxKey.Size = new System.Drawing.Size(76, 22);
            this.txtboxKey.TabIndex = 50;
            this.txtboxKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtboxKey_KeyUp);
            // 
            // txtboxScriptPath
            // 
            this.txtboxScriptPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtboxScriptPath.Location = new System.Drawing.Point(78, 54);
            this.txtboxScriptPath.Name = "txtboxScriptPath";
            this.txtboxScriptPath.ReadOnly = true;
            this.txtboxScriptPath.Size = new System.Drawing.Size(160, 22);
            this.txtboxScriptPath.TabIndex = 51;
            this.txtboxScriptPath.Text = "Click here to add a script";
            this.txtboxScriptPath.Click += new System.EventHandler(this.txtboxAction_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(0, 28);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(130, 23);
            this.btnAdd.TabIndex = 52;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(187, 28);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 23);
            this.btnSave.TabIndex = 53;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(136, 28);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(51, 23);
            this.btnLoad.TabIndex = 54;
            this.btnLoad.Text = "Load...";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // checkboxHotkeysStatus
            // 
            this.checkboxHotkeysStatus.AutoSize = true;
            this.checkboxHotkeysStatus.Location = new System.Drawing.Point(12, 7);
            this.checkboxHotkeysStatus.Name = "checkboxHotkeysStatus";
            this.checkboxHotkeysStatus.Size = new System.Drawing.Size(101, 17);
            this.checkboxHotkeysStatus.TabIndex = 55;
            this.checkboxHotkeysStatus.Text = "Hotkeys status";
            this.checkboxHotkeysStatus.UseVisualStyleBackColor = true;
            this.checkboxHotkeysStatus.CheckedChanged += new System.EventHandler(this.checkboxHotkeysStatus_CheckedChanged);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(136, 3);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(102, 23);
            this.btnHelp.TabIndex = 56;
            this.btnHelp.Text = "Help...";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // contextmenuHotkeys
            // 
            this.contextmenuHotkeys.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripRemove});
            this.contextmenuHotkeys.Name = "contextmenuHotkeys";
            this.contextmenuHotkeys.ShowImageMargin = false;
            this.contextmenuHotkeys.Size = new System.Drawing.Size(93, 26);
            // 
            // toolstripRemove
            // 
            this.toolstripRemove.Name = "toolstripRemove";
            this.toolstripRemove.Size = new System.Drawing.Size(92, 22);
            this.toolstripRemove.Text = "Remove";
            this.toolstripRemove.Click += new System.EventHandler(this.toolstripRemove_Click);
            // 
            // Hotkeys
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 351);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.checkboxHotkeysStatus);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtboxScriptPath);
            this.Controls.Add(this.txtboxKey);
            this.Controls.Add(this.datagridHotkeys);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Hotkeys";
            this.Text = "Hotkeys";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Hotkeys_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.datagridHotkeys)).EndInit();
            this.contextmenuHotkeys.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.DataGridView datagridHotkeys;
        private System.Windows.Forms.TextBox txtboxKey;
        private System.Windows.Forms.TextBox txtboxScriptPath;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckBox checkboxHotkeysStatus;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Timer timerStatusWatcher;
        private System.Windows.Forms.ContextMenuStrip contextmenuHotkeys;
        private System.Windows.Forms.ToolStripMenuItem toolstripRemove;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}