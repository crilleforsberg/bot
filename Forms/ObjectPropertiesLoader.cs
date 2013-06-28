using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KarelazisBot.Forms
{
    public partial class ObjectPropertiesLoader : Form
    {
        public ObjectPropertiesLoader(Objects.Client client)
        {
            InitializeComponent();

            this.Icon = Properties.Resources.icon;
            this.Client = client;
            this.Client.ObjectPropertyRead += new Objects.Client.ObjectPropertyReadHandler(Client_ObjectPropertyRead);
            this.Client.ObjectPropertiesFinishedReading += new Objects.Client.ObjectPropertiesFinishedReadingHandler(Client_ObjectPropertiesFinishedReading);
            this.Client.ObjectPropertiesFailed += new Objects.Client.ObjectPropertiesFailedHandler(delegate(Exception ex)
                {
                    MessageBox.Show("Something went wrong when processing object properties.\n\n" +
                        ex.Message + "\n" + ex.StackTrace,
                        "Error",
                        MessageBoxButtons.OK);
                    this.Close();
                });
        }

        public Objects.Client Client { get; private set; }
        public bool Finished { get; private set; }
        private System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();
        private int objPropsReadStep = 20, objPropsOldIndex = 0;

        void Client_ObjectPropertiesFinishedReading(int length)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                this.Finished = true;
                this.Close();
            });
        }

        void Client_ObjectPropertyRead(int index, int length)
        {
            if (!this.Stopwatch.IsRunning) this.Stopwatch.Start();
            
            if (this.Stopwatch.Elapsed.TotalSeconds > 5 && objPropsOldIndex + objPropsReadStep < index)
            {
                int percentDone = (int)(((double)index / (double)length) * 100);
                progressBarLoading.Invoke((MethodInvoker)delegate()
                {
                    progressBarLoading.Value = percentDone;
                });
                int propertiesPerSecond = index / (int)this.Stopwatch.Elapsed.TotalSeconds;
                TimeSpan time = TimeSpan.FromSeconds((int)((length - index) / propertiesPerSecond));
                lblEstimatedTimeLeft.Invoke((MethodInvoker)delegate()
                {
                    lblEstimatedTimeLeft.Text = "Estimated time left: " + time.ToString("c") + " [" + index + "/" + length + "]";
                });
                this.objPropsOldIndex = index;
            }
        }


        private void ObjectPropertiesLoader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.Finished) Environment.Exit(Environment.ExitCode);
        }

        private void ObjectPropertiesLoader_Shown(object sender, EventArgs e)
        {
            this.Client.LoadProperties();
        }
    }
}
