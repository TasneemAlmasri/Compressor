using System;
using System.Windows.Forms;

namespace FileCompressor
{
    public partial class ProgressForm : Form
    {
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        public ProgressForm()
        {
            InitializeComponent();
            IsPaused = false;
            IsCancelled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            IsPaused = true;
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            IsPaused = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCancelled = true;
        }

        public void UpdateProgress(int percent, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(percent, status)));
                return;
            }

            progressBar.Value = percent;
            labelStatus.Text = status;
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent form from closing if user presses X
            e.Cancel = true;
        }
    }
}
