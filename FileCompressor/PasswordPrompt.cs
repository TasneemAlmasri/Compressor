using System;
using System.Windows.Forms;

namespace FileCompressor
{
    public partial class PasswordPrompt : Form
    {
        public string Password => txtPassword.Text;

        public PasswordPrompt()
        {
            InitializeComponent();
            this.Text = "Enter Password";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 150;

            Label lbl = new Label()
            {
                Text = "Enter password:",
                Left = 10,
                Top = 15,
                Width = 260
            };

            txtPassword = new TextBox()
            {
                Left = 10,
                Top = 40,
                Width = 260,
                PasswordChar = '*'
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 110,
                Width = 70,
                Top = 75
            };

            Button btnCancel = new Button()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 190,
                Width = 70,
                Top = 75
            };

            this.Controls.Add(lbl);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private TextBox txtPassword;
    }
}
