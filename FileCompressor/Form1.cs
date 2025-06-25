using FileCompressorApp;
using FileCompressorApp.Compression;
using FileCompressorApp.IO;
using FileCompressorApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

                if (openFileDialog.FileNames.Length > 0)
                {
                    string firstFile = openFileDialog.FileNames[0];
                    string dir = Path.GetDirectoryName(firstFile);
                    string archivePath = Path.Combine(dir, "archive.bin");
                    txtOutputPath.Text = archivePath;
                }
            }
        }

        private void radioShannon_CheckedChanged(object sender, EventArgs e) { }
        private void radioHuffman_CheckedChanged(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void groupBox3_Enter(object sender, EventArgs e) { }
        private void txtOutputPath_TextChanged(object sender, EventArgs e) { }
        private void txtFilePath_TextChanged(object sender, EventArgs e) { }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetPassword_Click(object sender, EventArgs e)
        {
            Form passwordForm = new Form()
            {
                Width = 300,
                Height = 250,
                Text = "Set Password",
                StartPosition = FormStartPosition.CenterParent
            };

            Label lbl1 = new Label() { Left = 10, Top = 20, Text = "Enter password:", Width = 250 };
            TextBox txt1 = new TextBox() { Left = 10, Top = 45, Width = 250, PasswordChar = '*' };

            Label lbl2 = new Label() { Left = 10, Top = 80, Text = "Confirm password:", Width = 250 };
            TextBox txt2 = new TextBox() { Left = 10, Top = 105, Width = 250, PasswordChar = '*' };

            Button btnOk = new Button() { Text = "OK", Left = 100, Width = 80, Top = 140, DialogResult = DialogResult.OK };
            passwordForm.AcceptButton = btnOk;

            passwordForm.Controls.Add(lbl1);
            passwordForm.Controls.Add(txt1);
            passwordForm.Controls.Add(lbl2);
            passwordForm.Controls.Add(txt2);
            passwordForm.Controls.Add(btnOk);

            if (passwordForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txt1.Text))
                {
                    password = "";
                    MessageBox.Show("Password cleared.");
                }
                else if (txt1.Text != txt2.Text)
                {
                    MessageBox.Show("Passwords do not match!");
                }
                else
                {
                    password = txt1.Text;
                    MessageBox.Show("Password set.");
                }
            }
        }

        //private async void btnStart_Click(object sender, EventArgs e)
        //{
        //    string[] inputPaths = txtFilePath.Text.Split(';').Select(p => p.Trim()).ToArray();
        //    if (inputPaths.Length == 0 || inputPaths.Any(p => !File.Exists(p)))
        //    {
        //        MessageBox.Show("Please select valid input files.");
        //        return;
        //    }

        //    string outputPath = txtOutputPath.Text.Trim();
        //    if (string.IsNullOrWhiteSpace(outputPath))
        //    {
        //        MessageBox.Show("Please select an output file path.");
        //        return;
        //    }

        //    string algo = GetSelectedAlgorithm();
        //    if (algo == "none")
        //    {
        //        MessageBox.Show("Please select a compression algorithm.");
        //        return;
        //    }

        //    var progressForm = new ProgressForm();
        //    progressForm.Show();
        //    progressForm.BringToFront();

        //    try
        //    {
        //        await Task.Run(() =>
        //        {
        //            //if (inputPaths.Length == 1)
        //            //{
        //            //    string text = File.ReadAllText(inputPaths[0]);
        //            //    string decodedPath = Path.Combine(Path.GetDirectoryName(outputPath),
        //            //                            Path.GetFileNameWithoutExtension(outputPath) + "_decoded.txt");

        //            if (algo == "shannon")
        //            {

        //                // Gather all .txt files from both files and folders
        //                List<string> allTxtFiles = new List<string>();

        //                foreach (var path in inputPaths)
        //                {
        //                    if (Directory.Exists(path))
        //                    {
        //                        // Recursively get all .txt files from the folder
        //                        var txts = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
        //                        allTxtFiles.AddRange(txts);
        //                    }
        //                    else if (File.Exists(path) && Path.GetExtension(path).ToLower() == ".txt")
        //                    {
        //                        allTxtFiles.Add(path);
        //                    }
        //                }

        //                // Compute base folder (used to make relative paths)
        //                string baseFolder = allTxtFiles.Count > 0 ? Path.GetDirectoryName(allTxtFiles[0]) ?? "" : "";

        //                FileEncoder.SaveEncodedFile(outputPath, allTxtFiles, baseFolder);


        //            }
        //            else if (algo == "huffman")
        //            {
        //                string text = File.ReadAllText(inputPaths[0]);
        //                string decodedPath = Path.Combine(Path.GetDirectoryName(outputPath),
        //                                        Path.GetFileNameWithoutExtension(outputPath) + "_decoded.txt");
        //                var nodes = HuffmanCompressor.BuildFrequencyTable(text);
        //                var root = HuffmanCompressor.BuildTree(nodes);
        //                var codeTable = HuffmanCompressor.BuildCodeTable(root);
        //                string encoded = HuffmanCompressor.Encode(text, codeTable);
        //                FileEncoder.SaveEncodedFileHuffman(outputPath, codeTable, encoded);
        //                FileEncoder.DecodeFileHuffman(outputPath, decodedPath);
        //            }
        //        }
        //    //else
        //    //{
        //    //    if (algo != "shannon")
        //    //        throw new Exception("Multi-file archiving is only supported for Shannon-Fano for now.");

        //    //    Program.SaveEncodedFile(outputPath, inputPaths.ToList());
        //    //}
        //    //}
        //    );

        //        MessageBox.Show("Compression complete.");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error during compression:\n{ex.Message}");
        //    }
        //    finally
        //    {
        //        progressForm.Close();  // إغلاق النافذة بعد انتهاء العملية
        //    }
        //}

        private async void btnStart_Click(object sender, EventArgs e)
        {
            // Step 1: Read selected paths from txtFilePath (split by ;)
            string[] inputPaths = txtFilePath.Text.Split(';')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToArray();

            if (inputPaths.Length == 0)
            {
                MessageBox.Show("Please select files or folders to compress.");
                return;
            }

            // Step 2: Gather all valid .txt files
            List<string> allTxtFiles = new List<string>();

            foreach (var path in inputPaths)
            {
                if (Directory.Exists(path))
                {
                    // Recursively add all .txt files in this folder
                    string[] txtsInFolder = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
                    allTxtFiles.AddRange(txtsInFolder);
                }
                else if (File.Exists(path) && Path.GetExtension(path).ToLower() == ".txt")
                {
                    allTxtFiles.Add(path);
                }
            }

            if (allTxtFiles.Count == 0)
            {
                MessageBox.Show("No valid .txt files found in the selected files/folders.");
                return;
            }

            // Step 3: Output path
            string outputPath = txtOutputPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Please select an output path.");
                return;
            }

            // Step 4: Algorithm
            string algo = GetSelectedAlgorithm();
            if (algo != "shannon")
            {
                MessageBox.Show("Only Shannon-Fano compression is currently supported for multiple files/folders.");
                return;
            }

            // Step 5: Show progress
            var progressForm = new ProgressForm();
            progressForm.Show();
            progressForm.BringToFront();

            try
            {
                await Task.Run(() =>
                {
                    // Base folder: use the common parent directory
                    string baseFolder = Path.GetDirectoryName(allTxtFiles[0]) ?? "";

                    // Compress all files
                    FileEncoder.SaveEncodedFile(outputPath, allTxtFiles, baseFolder);
                });

                MessageBox.Show("Compression complete.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during compression:\n{ex.Message}");
            }
            finally
            {
                progressForm.Close();
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
                if (item.Tag is ArchiveEntry entry)
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
                FileEncoder.DecodeFile(inputArchive, outputPath, selectedFileNames);
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

            List<ArchiveEntry> entries = ArchiveReader.ReadArchiveIndex(archivePath);

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

        private void RunWithProgress(Action<ProgressForm> action)
        {
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();

            Thread thread = new Thread(() =>
            {
                try
                {
                    action(progressForm);

                    progressForm.Invoke(new Action(() =>
                    {
                        progressForm.Close();
                        MessageBox.Show("Operation completed.");
                    }));
                }
                catch (Exception ex)
                {
                    progressForm.Invoke(new Action(() =>
                    {
                        progressForm.Close();
                        MessageBox.Show("Error: " + ex.Message);
                    }));
                }
            });

            thread.Start();
        }

        private void btnBrowseFolders_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to compress";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;

                    // Get existing paths from txtFilePath
                    var existingPaths = txtFilePath.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // Add the selected folder path if not already there
                    if (!existingPaths.Contains(selectedFolder))
                        existingPaths.Add(selectedFolder);

                    // Update txtFilePath with folder paths separated by semicolon
                    txtFilePath.Text = string.Join(";", existingPaths);

                    // Set the output path based on the first selected folder
                    if (existingPaths.Count > 0)
                    {
                        string firstFolder = existingPaths[0];
                        string dir = Path.GetDirectoryName(firstFolder.TrimEnd(Path.DirectorySeparatorChar));
                        string folderName = Path.GetFileName(firstFolder.TrimEnd(Path.DirectorySeparatorChar));

                        // Compose output file name: folderName.bin inside same parent directory
                        string archivePath = Path.Combine(dir ?? "", folderName + ".bin");
                        txtOutputPath.Text = archivePath;
                    }
                }
            }
        }


    }
}
