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
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = string.Join(";", openFileDialog.FileNames);

                // Generate corresponding default .bin paths for output
                List<string> outputPaths = new List<string>();

                foreach (var path in openFileDialog.FileNames)
                {
                    string dir = Path.GetDirectoryName(path);
                    string name = Path.GetFileNameWithoutExtension(path);
                    string outputPath = Path.Combine(dir, name + ".bin");
                    outputPaths.Add(outputPath);
                }

                txtOutputPath.Text = string.Join(";", outputPaths);
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
            string[] inputPaths = txtFilePath.Text.Split(';');
            string[] outputPaths = txtOutputPath.Text.Split(';');

            if (inputPaths.Length == 0 || inputPaths.Any(p => !File.Exists(p)))
            {
                MessageBox.Show("Please select valid input files.");
                return;
            }

            if (outputPaths.Length != inputPaths.Length)
            {
                MessageBox.Show("Mismatch between number of input and output files.");
                return;
            }

            string algo = GetSelectedAlgorithm();
            if (algo == "none")
            {
                MessageBox.Show("Please select a compression algorithm.");
                return;
            }

            for (int i = 0; i < inputPaths.Length; i++)
            {
                string inputPath = inputPaths[i].Trim();
                string outputPath = outputPaths[i].Trim();

                try
                {
                    string text = File.ReadAllText(inputPath);

                    if (algo == "shannon")
                    {
                        var symbols = Program.BuildFrequencyTable(text);
                        Program.BuildShannonFanoCodes(symbols);
                        string encoded = Program.EncodeText(text, symbols);
                        Program.SaveEncodedFile(outputPath, symbols, encoded);
                    }
                    else if (algo == "huffman")
                    {
                        MessageBox.Show("Huffman not implemented yet.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing {inputPath}:\n{ex.Message}");
                }
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

        private void debtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                detxtFilePath.Text = openFileDialog.FileName;

                // Optional: auto-suggest output .txt file path
                string dir = Path.GetDirectoryName(openFileDialog.FileName);
                string name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                detxtOutputPath.Text = Path.Combine(dir, name + "_decoded.txt");
            }
        }

        private void debtnBrowseDistance_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                detxtOutputPath.Text = saveFileDialog.FileName;
            }
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDecompress_Click_Click(object sender, EventArgs e)
        {
            string inputPath = detxtFilePath.Text;
            string outputPath = detxtOutputPath.Text;

            if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
            {
                MessageBox.Show("Please select a valid compressed (.bin) file.");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Please select an output file path.");
                return;
            }

            try
            {
                Program.DecodeFile(inputPath, outputPath);
                MessageBox.Show("Decompression complete.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during decompression:\n{ex.Message}");
            }
        }
    }
}
