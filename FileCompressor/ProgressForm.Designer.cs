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
            this.components = new System.ComponentModel.Container();

            this.progressBar = new ProgressBar();
            this.labelStatus = new Label();
            this.lblCurrentFile = new Label();
            this.btnPause = new Button();
            this.btnResume = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // lblCurrentFile
            this.lblCurrentFile.Location = new Point(20, 15);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new Size(440, 20);
            this.lblCurrentFile.TabIndex = 5;
            this.lblCurrentFile.Text = "Current File: ";
            this.lblCurrentFile.AutoSize = false;

            // progressBar
            this.progressBar.Location = new Point(20, 45);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(440, 23);
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 4;

            // labelStatus
            this.labelStatus.Location = new Point(20, 75);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new Size(440, 20);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Status...";

            // btnPause
            this.btnPause.Location = new Point(40, 105);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new Size(100, 30);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;

            // btnResume
            this.btnResume.Location = new Point(160, 105);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new Size(100, 30);
            this.btnResume.TabIndex = 1;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Enabled = false;

            // btnCancel
            this.btnCancel.Location = new Point(280, 105);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(100, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // ProgressForm
            this.ClientSize = new Size(480, 150);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Compression Progress";

            this.ResumeLayout(false);
        }
    }
}
