using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using WBPlatform.Logging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WBPlatform.StaticClasses;
using System.Collections.Concurrent;

namespace WBPlatform.Database.DBServer
{
    public partial class MainForm : Form
    {
        public static ConcurrentQueue<OnLogChangedEventArgs> eventArgs = new ConcurrentQueue<OnLogChangedEventArgs>();
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            L.OnLog += LogWritter_onLog;
        }

        private void LogWritter_onLog(OnLogChangedEventArgs logchange, object sender)
        {
            eventArgs.Enqueue(logchange);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void clientEnumTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ConcurrentDictionary<string, string> clientConncetionQueryStrings;
                lock (DatabaseSocketsServer.QueryStrings)
                {
                    clientConncetionQueryStrings = DatabaseSocketsServer.QueryStrings;
                }

                listView1.Items.Clear();
                foreach (KeyValuePair<string, string> item in clientConncetionQueryStrings)
                {
                    listView1.Items.Add(new ListViewItem(new string[] { item.Key, item.Value }));
                }
                dbConnections.Text = "1";
                currentClients.Text = listView1.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                L.E("TIMER ERROR: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            if (eventArgs.TryDequeue(out OnLogChangedEventArgs logchange))
            {
                int lenth = logTextBox.Text.Length;
                switch (logchange.LogLevel)
                {
                    case LogLevel.DBG:
                        for (int j = 0; logchange.LogString[j] != '\n'; j++)
                        {
                            logTextBox.Select(lenth + j, 1);
                            logTextBox.SelectionColor = Color.Cyan;
                        }
                        break;
                    case LogLevel.INF:
                        for (int j = 0; logchange.LogString[j] != '\n'; j++)
                        {
                            logTextBox.Select(lenth + j, 1);
                            logTextBox.SelectionColor = Color.Blue;
                        }
                        break;
                    case LogLevel.WRN:
                        for (int j = 0; logchange.LogString[j] != '\n'; j++)
                        {
                            logTextBox.Select(lenth + j, 1);
                            logTextBox.SelectionColor = Color.Yellow;
                        }
                        break;
                    case LogLevel.ERR:
                        for (int j = 0; logchange.LogString[j] != '\n'; j++)
                        {
                            logTextBox.Select(lenth + j, 1);
                            logTextBox.SelectionColor = Color.Red;
                        }
                        break;
                }
                logTextBox.AppendText(logchange.LogString);
                logTextBox.SelectionStart = logTextBox.Text.Length;
                logTextBox.ScrollToCaret();
            }
        }
    }
}
