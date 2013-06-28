namespace KarelazisBot.Forms
{
    partial class ClientChooser
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboboxClients = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtboxClassName = new System.Windows.Forms.TextBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please choose a Tibia client:";
            // 
            // comboboxClients
            // 
            this.comboboxClients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxClients.FormattingEnabled = true;
            this.comboboxClients.Location = new System.Drawing.Point(12, 56);
            this.comboboxClients.Name = "comboboxClients";
            this.comboboxClients.Size = new System.Drawing.Size(146, 21);
            this.comboboxClients.TabIndex = 1;
            this.comboboxClients.DropDown += new System.EventHandler(this.comboboxClients_DropDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Class name of Tibia:";
            // 
            // txtboxClassName
            // 
            this.txtboxClassName.Location = new System.Drawing.Point(117, 6);
            this.txtboxClassName.Name = "txtboxClassName";
            this.txtboxClassName.Size = new System.Drawing.Size(163, 22);
            this.txtboxClassName.TabIndex = 3;
            this.txtboxClassName.Text = "TibiaClient";
            this.txtboxClassName.TextChanged += new System.EventHandler(this.txtboxClassName_TextChanged);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(164, 41);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(116, 37);
            this.btnContinue.TabIndex = 4;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // ClientChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 84);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.txtboxClassName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboboxClients);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ClientChooser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Karelazi\'s Bot 2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientChooser_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboboxClients;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtboxClassName;
        private System.Windows.Forms.Button btnContinue;
    }
}