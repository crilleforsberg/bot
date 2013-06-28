namespace KarelazisBot.Forms
{
    partial class ObjectPropertiesLoader
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
            this.progressBarLoading = new System.Windows.Forms.ProgressBar();
            this.lblEstimatedTimeLeft = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBarLoading
            // 
            this.progressBarLoading.Location = new System.Drawing.Point(12, 12);
            this.progressBarLoading.Name = "progressBarLoading";
            this.progressBarLoading.Size = new System.Drawing.Size(294, 23);
            this.progressBarLoading.TabIndex = 0;
            // 
            // lblEstimatedTimeLeft
            // 
            this.lblEstimatedTimeLeft.AutoSize = true;
            this.lblEstimatedTimeLeft.Location = new System.Drawing.Point(9, 38);
            this.lblEstimatedTimeLeft.Name = "lblEstimatedTimeLeft";
            this.lblEstimatedTimeLeft.Size = new System.Drawing.Size(116, 13);
            this.lblEstimatedTimeLeft.TabIndex = 1;
            this.lblEstimatedTimeLeft.Text = "Estimated time left: --";
            // 
            // ObjectPropertiesLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 58);
            this.Controls.Add(this.lblEstimatedTimeLeft);
            this.Controls.Add(this.progressBarLoading);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ObjectPropertiesLoader";
            this.Text = "Loading object properties...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectPropertiesLoader_FormClosing);
            this.Shown += new System.EventHandler(this.ObjectPropertiesLoader_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarLoading;
        private System.Windows.Forms.Label lblEstimatedTimeLeft;
    }
}