namespace KarelazisBot.Forms
{
    partial class Scripter
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnNewScript = new System.Windows.Forms.Button();
            this.btnRemoveScript = new System.Windows.Forms.Button();
            this.btnStartStopScript = new System.Windows.Forms.Button();
            this.btnStopAllScripts = new System.Windows.Forms.Button();
            this.btnStartAllScripts = new System.Windows.Forms.Button();
            this.txtboxQuickExec = new System.Windows.Forms.TextBox();
            this.datagridScripts = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.datagridScripts)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNewScript
            // 
            this.btnNewScript.Location = new System.Drawing.Point(259, 0);
            this.btnNewScript.Name = "btnNewScript";
            this.btnNewScript.Size = new System.Drawing.Size(151, 23);
            this.btnNewScript.TabIndex = 10;
            this.btnNewScript.TabStop = false;
            this.btnNewScript.Text = "New script";
            this.btnNewScript.UseVisualStyleBackColor = true;
            this.btnNewScript.Click += new System.EventHandler(this.btnNewScript_Click);
            // 
            // btnRemoveScript
            // 
            this.btnRemoveScript.Location = new System.Drawing.Point(259, 23);
            this.btnRemoveScript.Name = "btnRemoveScript";
            this.btnRemoveScript.Size = new System.Drawing.Size(151, 23);
            this.btnRemoveScript.TabIndex = 12;
            this.btnRemoveScript.TabStop = false;
            this.btnRemoveScript.Text = "Remove script";
            this.btnRemoveScript.UseVisualStyleBackColor = true;
            this.btnRemoveScript.Click += new System.EventHandler(this.btnRemoveScript_Click);
            // 
            // btnStartStopScript
            // 
            this.btnStartStopScript.Location = new System.Drawing.Point(259, 46);
            this.btnStartStopScript.Name = "btnStartStopScript";
            this.btnStartStopScript.Size = new System.Drawing.Size(151, 23);
            this.btnStartStopScript.TabIndex = 13;
            this.btnStartStopScript.TabStop = false;
            this.btnStartStopScript.Text = "Start/stop script";
            this.btnStartStopScript.UseVisualStyleBackColor = true;
            this.btnStartStopScript.Click += new System.EventHandler(this.btnStartStopScript_Click);
            // 
            // btnStopAllScripts
            // 
            this.btnStopAllScripts.Location = new System.Drawing.Point(259, 69);
            this.btnStopAllScripts.Name = "btnStopAllScripts";
            this.btnStopAllScripts.Size = new System.Drawing.Size(151, 23);
            this.btnStopAllScripts.TabIndex = 14;
            this.btnStopAllScripts.TabStop = false;
            this.btnStopAllScripts.Text = "Stop all scripts";
            this.btnStopAllScripts.UseVisualStyleBackColor = true;
            this.btnStopAllScripts.Click += new System.EventHandler(this.btnStopAllScripts_Click);
            // 
            // btnStartAllScripts
            // 
            this.btnStartAllScripts.Location = new System.Drawing.Point(259, 92);
            this.btnStartAllScripts.Name = "btnStartAllScripts";
            this.btnStartAllScripts.Size = new System.Drawing.Size(151, 23);
            this.btnStartAllScripts.TabIndex = 15;
            this.btnStartAllScripts.TabStop = false;
            this.btnStartAllScripts.Text = "Start all scripts";
            this.btnStartAllScripts.UseVisualStyleBackColor = true;
            this.btnStartAllScripts.Click += new System.EventHandler(this.btnStartAllScripts_Click);
            // 
            // txtboxQuickExec
            // 
            this.txtboxQuickExec.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtboxQuickExec.Location = new System.Drawing.Point(0, 164);
            this.txtboxQuickExec.Name = "txtboxQuickExec";
            this.txtboxQuickExec.Size = new System.Drawing.Size(410, 22);
            this.txtboxQuickExec.TabIndex = 17;
            this.txtboxQuickExec.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtboxQuickExec_KeyDown);
            // 
            // datagridScripts
            // 
            this.datagridScripts.AllowUserToAddRows = false;
            this.datagridScripts.AllowUserToDeleteRows = false;
            this.datagridScripts.AllowUserToResizeColumns = false;
            this.datagridScripts.AllowUserToResizeRows = false;
            this.datagridScripts.BackgroundColor = System.Drawing.Color.White;
            this.datagridScripts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridScripts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.datagridScripts.Dock = System.Windows.Forms.DockStyle.Left;
            this.datagridScripts.Location = new System.Drawing.Point(0, 0);
            this.datagridScripts.MultiSelect = false;
            this.datagridScripts.Name = "datagridScripts";
            this.datagridScripts.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.datagridScripts.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.datagridScripts.RowHeadersVisible = false;
            this.datagridScripts.RowHeadersWidth = 24;
            this.datagridScripts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridScripts.Size = new System.Drawing.Size(259, 164);
            this.datagridScripts.TabIndex = 49;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn5.HeaderText = "File";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn6.HeaderText = "IsRunning";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // Scripter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 186);
            this.Controls.Add(this.datagridScripts);
            this.Controls.Add(this.txtboxQuickExec);
            this.Controls.Add(this.btnStartAllScripts);
            this.Controls.Add(this.btnStopAllScripts);
            this.Controls.Add(this.btnStartStopScript);
            this.Controls.Add(this.btnRemoveScript);
            this.Controls.Add(this.btnNewScript);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Scripter";
            this.Text = "Scripter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Scripter_FormClosing);
            this.Shown += new System.EventHandler(this.Scripter_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.datagridScripts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewScript;
        private System.Windows.Forms.Button btnRemoveScript;
        private System.Windows.Forms.Button btnStartStopScript;
        private System.Windows.Forms.Button btnStopAllScripts;
        private System.Windows.Forms.Button btnStartAllScripts;
        private System.Windows.Forms.TextBox txtboxQuickExec;
        private System.Windows.Forms.DataGridView datagridScripts;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}