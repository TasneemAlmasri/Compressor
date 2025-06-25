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
        //done
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = string.Join(";", openFileDialog.FileNames);

                if (openFileDialog.FileNames.Length > 0)
                {
                    string firstFile = openFileDialog.FileNames[0];
                    string dir = Path.GetDirectoryName(firstFile);
                    string archivePath = Path.Combine(dir, "archive.bin");
                    txtOutputPath.Text = archivePath;
                }
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

        // Compression Start button handler:
        //done
        private void btnStart_Click(object sender, EventArgs e)
        {
            string[] inputPaths = txtFilePath.Text.Split(';').Select(p => p.Trim()).ToArray();
            if (inputPaths.Length == 0 || inputPaths.Any(p => !File.Exists(p)))
            {
                MessageBox.Show("Please select valid input files.");
                return;
            }

            string outputArchivePath = txtOutputPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(outputArchivePath))
            {
                MessageBox.Show("Please select an output archive file path.");
                return;
            }

            string algo = GetSelectedAlgorithm();
            if (algo != "shannon")
            {
                MessageBox.Show("Only Shannon is implemented for now.");
                return;
            }

            try
            {
                Program.SaveEncodedFile(outputArchivePath, inputPaths.ToList());
                MessageBox.Show("Compression complete.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during compression:\n{ex.Message}");
            }
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
        //done
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
       
        //done all below
        private void debtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                detxtFilePath.Text = openFileDialog.FileName;

                string dir = Path.GetDirectoryName(openFileDialog.FileName);
                string folder = Path.Combine(dir, "Decompressed_" + Path.GetFileNameWithoutExtension(openFileDialog.FileName));
                detxtOutputPath.Text = folder;

                // Load the archive contents preview
                LoadArchiveIndex(detxtFilePath.Text);

            }
        }

        private void debtnBrowseDistance_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to extract decompressed files";

                if (!string.IsNullOrWhiteSpace(detxtOutputPath.Text))
                {
                    try
                    {
                        folderDialog.SelectedPath = detxtOutputPath.Text;
                    }
                    catch
                    {
                        // Ignore invalid path
                    }
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    detxtOutputPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Decompression button handler:
        private void btnDecompress_Click_Click(object sender, EventArgs e)
        {
            string inputArchive = detxtFilePath.Text.Trim();
            string outputPath = detxtOutputPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputArchive) || !File.Exists(inputArchive))
            {
                MessageBox.Show("Please select a valid archive (.bin) file.");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Please select a valid output path.");
                return;
            }

            // Get selected entries
            List<string> selectedFileNames = new List<string>();
            foreach (ListViewItem item in lvArchiveContents.CheckedItems)
            {
                if (item.Tag is Program.ArchiveEntry entry)
                    selectedFileNames.Add(entry.FileName);
            }

            if (selectedFileNames.Count == 0)
            {
                MessageBox.Show("Please select at least one file to decompress.");
                return;
            }

            try
            {
                // Decompress selected files
                Program.DecodeFile(inputArchive, outputPath, selectedFileNames);
                MessageBox.Show("Decompression complete.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during decompression:\n{ex.Message}");
            }
        }



        ///////////////////////////////////////////////////////////
        private void LoadArchiveIndex(string archivePath)
        {
            lvArchiveContents.Items.Clear();

            List<Program.ArchiveEntry> entries = Program.ReadArchiveIndex(archivePath);

            foreach (var entry in entries)
            {
                ListViewItem item = new ListViewItem(entry.FileName);
                item.SubItems.Add(entry.Algorithm);
                item.SubItems.Add(entry.OriginalSize.ToString());
                item.SubItems.Add(entry.CompressedSize.ToString());
                item.Tag = entry; // ? This is important!
                item.Checked = true;  // Select all by default
                lvArchiveContents.Items.Add(item);
            }
        }

        private void lvArchiveContents_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void listViewArchiveFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var selectedItems = lvArchiveContents.CheckedItems;

            if (selectedItems.Count == 1)
            {
                // One file selected ? suggest a single file output
                string originalFile = selectedItems[0].SubItems[0].Text;
                string dir = Path.GetDirectoryName(detxtFilePath.Text);
                string nameOnly = Path.GetFileNameWithoutExtension(originalFile);
                string outputFile = Path.Combine(dir, nameOnly + "_decompressed.txt");
                detxtOutputPath.Text = outputFile;
            }
            else if (selectedItems.Count > 1)
            {
                // Multiple files ? suggest folder output
                string dir = Path.GetDirectoryName(detxtFilePath.Text);
                string folder = Path.Combine(dir, "Decompressed_" + Path.GetFileNameWithoutExtension(detxtFilePath.Text));
                detxtOutputPath.Text = folder;
            }

            // Optional (Step 6): disable decompress button if nothing is selected
            btnDecompress_Click.Enabled = selectedItems.Count > 0;
        }




    }
}