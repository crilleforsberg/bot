namespace KarelazisBot.Forms
{
    partial class Healer
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
            this.datagridHealer = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.comboboxAction = new System.Windows.Forms.ComboBox();
            this.txtboxActionEx = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericHealth = new System.Windows.Forms.NumericUpDown();
            this.numericMana = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.numericCheckInterval = new System.Windows.Forms.NumericUpDown();
            this.numericMinSleep = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericMaxSleep = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.checkboxHealerStatus = new System.Windows.Forms.CheckBox();
            this.contextmenuHealer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolstripRemove = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.datagridHealer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHealth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMana)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCheckInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinSleep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSleep)).BeginInit();
            this.contextmenuHealer.SuspendLayout();
            this.SuspendLayout();
            // 
            // datagridHealer
            // 
            this.datagridHealer.AllowUserToAddRows = false;
            this.datagridHealer.AllowUserToDeleteRows = false;
            this.datagridHealer.AllowUserToResizeColumns = false;
            this.datagridHealer.AllowUserToResizeRows = false;
            this.datagridHealer.BackgroundColor = System.Drawing.Color.White;
            this.datagridHealer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridHealer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.datagridHealer.Dock = System.Windows.Forms.DockStyle.Left;
            this.datagridHealer.Location = new System.Drawing.Point(0, 0);
            this.datagridHealer.MultiSelect = false;
            this.datagridHealer.Name = "datagridHealer";
            this.datagridHealer.ReadOnly = true;
            this.datagridHealer.RowHeadersVisible = false;
            this.datagridHealer.RowHeadersWidth = 24;
            this.datagridHealer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridHealer.Size = new System.Drawing.Size(201, 329);
            this.datagridHealer.TabIndex = 48;
            this.datagridHealer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.datagridHealer_KeyUp);
            this.datagridHealer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.datagridHealer_MouseDown);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn5.HeaderText = "Action";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn6.HeaderText = "Health/Mana";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Action:";
            // 
            // comboboxAction
            // 
            this.comboboxAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxAction.FormattingEnabled = true;
            this.comboboxAction.Items.AddRange(new object[] {
            "Say",
            "UH",
            "IH",
            "Mana",
            "MP",
            "SMP",
            "GMP",
            "HP",
            "SHP",
            "GHP",
            "UHP",
            "GSP"});
            this.comboboxAction.Location = new System.Drawing.Point(207, 25);
            this.comboboxAction.Name = "comboboxAction";
            this.comboboxAction.Size = new System.Drawing.Size(103, 21);
            this.comboboxAction.TabIndex = 50;
            this.comboboxAction.SelectedIndexChanged += new System.EventHandler(this.comboboxAction_SelectedIndexChanged);
            // 
            // txtboxActionEx
            // 
            this.txtboxActionEx.Location = new System.Drawing.Point(207, 48);
            this.txtboxActionEx.Name = "txtboxActionEx";
            this.txtboxActionEx.Size = new System.Drawing.Size(103, 22);
            this.txtboxActionEx.TabIndex = 51;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "Health:";
            // 
            // numericHealth
            // 
            this.numericHealth.Location = new System.Drawing.Point(207, 89);
            this.numericHealth.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericHealth.Name = "numericHealth";
            this.numericHealth.Size = new System.Drawing.Size(103, 22);
            this.numericHealth.TabIndex = 53;
            // 
            // numericMana
            // 
            this.numericMana.Location = new System.Drawing.Point(207, 130);
            this.numericMana.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericMana.Name = "numericMana";
            this.numericMana.Size = new System.Drawing.Size(103, 22);
            this.numericMana.TabIndex = 55;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(239, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Mana:";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(207, 158);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(103, 23);
            this.btnAdd.TabIndex = 56;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(206, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 57;
            this.label4.Text = "Check interval (ms):";
            // 
            // numericCheckInterval
            // 
            this.numericCheckInterval.Location = new System.Drawing.Point(207, 200);
            this.numericCheckInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericCheckInterval.Name = "numericCheckInterval";
            this.numericCheckInterval.Size = new System.Drawing.Size(103, 22);
            this.numericCheckInterval.TabIndex = 58;
            this.numericCheckInterval.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericCheckInterval.ValueChanged += new System.EventHandler(this.numericCheckInterval_ValueChanged);
            // 
            // numericMinSleep
            // 
            this.numericMinSleep.Location = new System.Drawing.Point(207, 241);
            this.numericMinSleep.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericMinSleep.Name = "numericMinSleep";
            this.numericMinSleep.Size = new System.Drawing.Size(103, 22);
            this.numericMinSleep.TabIndex = 60;
            this.numericMinSleep.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericMinSleep.ValueChanged += new System.EventHandler(this.numericMinSleep_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 59;
            this.label5.Text = "Min. sleep (ms):";
            // 
            // numericMaxSleep
            // 
            this.numericMaxSleep.Location = new System.Drawing.Point(207, 282);
            this.numericMaxSleep.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericMaxSleep.Name = "numericMaxSleep";
            this.numericMaxSleep.Size = new System.Drawing.Size(103, 22);
            this.numericMaxSleep.TabIndex = 62;
            this.numericMaxSleep.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericMaxSleep.ValueChanged += new System.EventHandler(this.numericMaxSleep_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(217, 266);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 61;
            this.label6.Text = "Max. sleep (ms):";
            // 
            // checkboxHealerStatus
            // 
            this.checkboxHealerStatus.AutoSize = true;
            this.checkboxHealerStatus.Location = new System.Drawing.Point(207, 310);
            this.checkboxHealerStatus.Name = "checkboxHealerStatus";
            this.checkboxHealerStatus.Size = new System.Drawing.Size(93, 17);
            this.checkboxHealerStatus.TabIndex = 63;
            this.checkboxHealerStatus.Text = "Healer status";
            this.checkboxHealerStatus.UseVisualStyleBackColor = true;
            this.checkboxHealerStatus.CheckedChanged += new System.EventHandler(this.checkboxHealerStatus_CheckedChanged);
            // 
            // contextmenuHealer
            // 
            this.contextmenuHealer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripRemove});
            this.contextmenuHealer.Name = "contextmenuHealer";
            this.contextmenuHealer.ShowImageMargin = false;
            this.contextmenuHealer.Size = new System.Drawing.Size(128, 48);
            // 
            // toolstripRemove
            // 
            this.toolstripRemove.Name = "toolstripRemove";
            this.toolstripRemove.Size = new System.Drawing.Size(127, 22);
            this.toolstripRemove.Text = "Remove";
            this.toolstripRemove.Click += new System.EventHandler(this.toolstripRemove_Click);
            // 
            // Healer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 329);
            this.Controls.Add(this.checkboxHealerStatus);
            this.Controls.Add(this.numericMaxSleep);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericMinSleep);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericCheckInterval);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.numericMana);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericHealth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtboxActionEx);
            this.Controls.Add(this.comboboxAction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.datagridHealer);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Healer";
            this.Text = "Healer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Healer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.datagridHealer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHealth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMana)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCheckInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinSleep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSleep)).EndInit();
            this.contextmenuHealer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.DataGridView datagridHealer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboboxAction;
        private System.Windows.Forms.TextBox txtboxActionEx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericHealth;
        private System.Windows.Forms.NumericUpDown numericMana;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericCheckInterval;
        private System.Windows.Forms.NumericUpDown numericMinSleep;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericMaxSleep;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.CheckBox checkboxHealerStatus;
        private System.Windows.Forms.ContextMenuStrip contextmenuHealer;
        private System.Windows.Forms.ToolStripMenuItem toolstripRemove;
    }
}