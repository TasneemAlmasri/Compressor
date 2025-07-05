namespace FileCompressor
{
    public partial class ProgressForm : Form
    {
        public bool IsPaused => _pauseEvent != null && !_pauseEvent.IsSet;
        public bool IsCancelled => _cts?.IsCancellationRequested ?? false;

        private bool allowClose = false;

        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;

        public ProgressForm()
        {
            InitializeComponent();

            btnPause.Click += btnPause_Click;
            btnResume.Click += btnResume_Click;
            btnCancel.Click += btnCancel_Click;

            this.FormClosing += ProgressForm_FormClosing;

            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true);
        }

        public CancellationToken Token => _cts.Token;
        public ManualResetEventSlim PauseEvent => _pauseEvent;

        private void btnPause_Click(object sender, EventArgs e)
        {
            _pauseEvent.Reset();
            btnPause.Enabled = false;
            btnResume.Enabled = true;
            labelStatus.Text = "Paused...";
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _pauseEvent.Set();
            btnPause.Enabled = true;
            btnResume.Enabled = false;
            labelStatus.Text = "Resuming...";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
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
