using System;
using System.IO;
using System.Windows.Forms;

namespace FileCompressor
{
    public partial class ProgressForm : Form
    {
        public bool IsPaused { get; private set; } = false;
        public bool IsCancelled { get; private set; } = false;
        private bool allowClose = false;

        public ProgressForm()
        {
            InitializeComponent();

            // ربط أحداث الأزرار
            btnPause.Click += btnPause_Click;
            btnResume.Click += btnResume_Click;
            btnCancel.Click += btnCancel_Click;

            this.FormClosing += ProgressForm_FormClosing;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            IsPaused = true;
            btnPause.Enabled = false;
            btnResume.Enabled = true;
            labelStatus.Text = "Paused...";
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            IsPaused = false;
            btnPause.Enabled = true;
            btnResume.Enabled = false;
            labelStatus.Text = "Resuming...";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCancelled = true;
            labelStatus.Text = "Cancelling...";
        }

        public void UpdateProgress(int fileIndex, int totalFiles, int percent, string fileName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(fileIndex, totalFiles, percent, fileName)));
                return;
            }

            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            lblCurrentFile.Text = $"File {fileIndex + 1} of {totalFiles}: {Path.GetFileName(fileName)}";
            progressBar.Value = percent;
            labelStatus.Text = $"Processing... {percent}%";
        }

        public void AllowClose()
        {
            allowClose = true;
            this.Invoke(new Action(() => this.Close()));
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
            }
        }
    }
}
