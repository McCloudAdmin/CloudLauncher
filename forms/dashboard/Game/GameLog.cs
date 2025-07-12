using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CloudLauncher.Properties;

namespace CloudLauncher.forms.dashboard.Game
{
    public partial class GameLog : Form
    {
        public GameLog()
        {
            InitializeComponent();
        }

        private void GameLog_Load(object sender, EventArgs e)
        {
            _instance = this;
            this.Text = "Minecraft Log - CloudLauncher";

            // Auto-scroll to bottom
            txtLog.SelectionStart = txtLog.Text.Length;
        }
        static ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
        private static GameLog _instance;

        internal static void AddLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                logQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}] {msg}");
            }
        }

        internal static void ClearLog()
        {
            // Clear the queue
            while (logQueue.TryDequeue(out _)) { }

            // Clear the UI if instance exists
            if (_instance != null && _instance.txtLog != null)
            {
                if (_instance.txtLog.InvokeRequired)
                {
                    _instance.txtLog.Invoke(new Action(() => _instance.txtLog.Clear()));
                }
                else
                {
                    _instance.txtLog.Clear();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string msg;
            bool hasNewMessages = false;

            while (logQueue.TryDequeue(out msg))
            {
                txtLog.AppendText(msg + "\n");
                hasNewMessages = true;
            }

            // Auto-scroll to bottom if new messages were added
            if (hasNewMessages)
            {
                txtLog.SelectionStart = txtLog.Text.Length;
            }
        }

        private void GameLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't actually close, just hide the form
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void GameLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _instance = this;
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ClearLog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtLog.Text))
                {
                    Clipboard.SetText(txtLog.Text);
                    MessageBox.Show("Log copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No log data to copy.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
                    saveFileDialog.Title = "Save Minecraft Log";
                    saveFileDialog.FileName = $"minecraft_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveFileDialog.FileName, txtLog.Text);
                        MessageBox.Show($"Log saved to: {saveFileDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = !this.TopMost;
                alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling always on top: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
