using System.Windows.Forms;
using System.Drawing;

namespace FileCompressor
{
    partial class ProgressForm
    {
        private System.ComponentModel.IContainer components = null;

        private ProgressBar progressBar;
        private Label labelStatus;
        private Label lblCurrentFile;
        private Button btnPause;
        private Button btnResume;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            progressBar = new ProgressBar();
            labelStatus = new Label();
            lblCurrentFile = new Label();
            btnPause = new Button();
            btnResume = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(20, 45);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(440, 23);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 4;
            // 
            // labelStatus
            // 
            labelStatus.Location = new Point(20, 75);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(440, 20);
            labelStatus.TabIndex = 3;
            labelStatus.Text = "Status...";
            // 
            // lblCurrentFile
            // 
            lblCurrentFile.Location = new Point(20, 15);
            lblCurrentFile.Name = "lblCurrentFile";
            lblCurrentFile.Size = new Size(440, 20);
            lblCurrentFile.TabIndex = 5;
            lblCurrentFile.Text = "Current File: ";
            // 
            // btnPause
            // 
            btnPause.Location = new Point(40, 105);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(100, 30);
            btnPause.TabIndex = 2;
            btnPause.Text = "Pause";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // btnResume
            // 
            btnResume.Enabled = false;
            btnResume.Location = new Point(160, 105);
            btnResume.Name = "btnResume";
            btnResume.Size = new Size(100, 30);
            btnResume.TabIndex = 1;
            btnResume.Text = "Resume";
            btnResume.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(280, 105);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ProgressForm
            // 
            ClientSize = new Size(480, 150);
            Controls.Add(lblCurrentFile);
            Controls.Add(btnCancel);
            Controls.Add(btnResume);
            Controls.Add(btnPause);
            Controls.Add(labelStatus);
            Controls.Add(progressBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Compression Progress";
            ResumeLayout(false);
        }
    }
}
