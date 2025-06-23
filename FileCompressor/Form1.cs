using FileCompressorApp;

namespace FileCompressor
{
    public partial class Form1 : Form
    {

        private string password = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;

                // Auto-fill output path with same directory and filename + ".bin"
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string directory = Path.GetDirectoryName(openFileDialog.FileName);
                txtOutputPath.Text = Path.Combine(directory, fileNameWithoutExt + ".bin");
            }
        }
        

        private void radioShannon_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioHuffman_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetPassword_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a password for encryption (optional):", "Set Password", "");

            if (!string.IsNullOrWhiteSpace(input))
            {
                password = input;
                MessageBox.Show("Password set.");
            }
            else
            {
                password = "";
                MessageBox.Show("Password cleared.");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string path = txtFilePath.Text;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MessageBox.Show("Please select a valid file.");
                return;
            }

            string outputPath = txtOutputPath.Text;
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Please specify an output path.");
                return;
            }

            string algo = GetSelectedAlgorithm();
            if (algo == "none")
            {
                MessageBox.Show("Please select a compression algorithm.");
                return;
            }

            string decodedPath = Path.Combine(Path.GetDirectoryName(outputPath),
                                              Path.GetFileNameWithoutExtension(outputPath) + "_decoded.txt");

            string text = File.ReadAllText(path);

            if (algo == "shannon")
            {
                var symbols = Program.BuildFrequencyTable(text);
                Program.BuildShannonFanoCodes(symbols);
                string encoded = Program.EncodeText(text, symbols);
                Program.SaveEncodedFile(outputPath, symbols, encoded);
                Program.DecodeFile(outputPath, decodedPath);
            }
            else if (algo == "huffman")
            {
                MessageBox.Show("Huffman not implemented yet.");
                return;
            }

            MessageBox.Show("Compression complete.");
        }



        private string GetSelectedAlgorithm()
        {
            if (radioShannon.Checked)
                return "shannon";
            else if (radioHuffman.Checked)
                return "huffman";
            else
                return "none";
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void txtOutputPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowseDistance_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "bin";

            // If output path textbox already has a path, start dialog there
            if (!string.IsNullOrEmpty(txtOutputPath.Text))
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(txtOutputPath.Text);
                saveFileDialog.FileName = Path.GetFileName(txtOutputPath.Text);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = saveFileDialog.FileName;
            }
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
