using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace KarelazisBot.Forms
{
    public partial class Information : Form
    {
        Objects.Client Client { get; set; }
        TimeSpan timespanCPUUsageOld { get; set; }
        PerformanceCounter perfCounter = null;
        Process CurrentProcess = null;

        internal Information(Objects.Client c)
        {
            this.Client = c;
            InitializeComponent();
            this.Icon = Properties.Resources.icon;
        }

        private void Information_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Information_VisibleChanged(object sender, EventArgs e)
        {
            timerBotInfo.Enabled = this.Visible;
        }

        private void timerBotInfo_Tick(object sender, EventArgs e)
        {
            if (CurrentProcess == null) CurrentProcess = Process.GetCurrentProcess();
            if (perfCounter == null) perfCounter = new PerformanceCounter("Process", "Working Set - Private", CurrentProcess.ProcessName);
            CurrentProcess.Refresh();
            if (timespanCPUUsageOld.TotalMilliseconds == 0) timespanCPUUsageOld = CurrentProcess.TotalProcessorTime;
            int threadsAliveCount = 0;
            foreach (ProcessThread thread in CurrentProcess.Threads) { if (thread.ThreadState == ThreadState.Running) threadsAliveCount++; }

            lblBotInfoPhysicalMemory.Text = "Private memory: " + perfCounter.RawValue / 1024 + " KiB (" + Math.Round((double)perfCounter.RawValue / 1024 / 1024, 2) + " MiB)";
            lblBotInfoProcessorUsage.Text = "CPU usage: " + Math.Round((CurrentProcess.TotalProcessorTime.TotalMilliseconds - timespanCPUUsageOld.TotalMilliseconds) / 100 / Environment.ProcessorCount, 2) + "%";
            lblBotInfoThreadCount.Text = "Thread count: " + threadsAliveCount + " / " + CurrentProcess.Threads.Count;

            timespanCPUUsageOld = CurrentProcess.TotalProcessorTime;
        }
    }
}
