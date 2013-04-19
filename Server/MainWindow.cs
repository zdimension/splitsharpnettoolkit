using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Server
{
    public partial class MainWindow : Form
    {
        public WebServer server = new WebServer();

        public MainWindow()
        {
            InitializeComponent();

            server.LogSent += this.LogReceivedThread;

            this.FormClosing += new FormClosingEventHandler(MainWindow_Closing);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (server.Started)
            {
                btnStartStop.Enabled = false;
                btnStartStop.Text = "Stopping...";
                server.Stop();
                btnStartStop.Text = "Start";
                btnStartStop.Enabled = true;
            }
            else
            {
                btnStartStop.Enabled = false;
                btnStartStop.Text = "Starting...";
                server.Start();
                btnStartStop.Text = "Stop";
                btnStartStop.Enabled = true;
            }
        }
        string l;
        public void LogReceived(object sender, string log)
        {
            l = log;
            //StreamWriter wr = new StreamWriter(Path.Combine(Application.StartupPath, @"conf\tmp.dat"));
            //wr.Write(l);
            //wr.Close();
            
            //MessageBox.Show(l);
            //if (txLog.Text == "")
            //{
           // Thread.CurrentThread = new Thread(new ThreadStart(LogReceived2));
            //}
            txLog.Text += l;

            //txLog.Text += File.ReadAllText(Path.Combine(Application.StartupPath, @"conf\tmp.dat"));
            txLog.Text += Environment.NewLine;
        }

        public void LogReceivedThread(object sender, string s)
        {
            Invoke(new WebServer.LogSentEventHandler(LogReceived), new object[] {this, s});
        }

        private void txLog_TextChanged(object sender, EventArgs e)
        {
            txLog.SelectionStart = txLog.Text.Length;
            txLog.ScrollToCaret();
        }

        private void MainWindow_Closing(object sender, FormClosingEventArgs e)
        {
            server.Stop();
            Application.Exit();
        }
    }
}
