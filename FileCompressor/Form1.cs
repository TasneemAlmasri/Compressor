using FileCompressorApp.Helpers;
using FileCompressorApp.IO;

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

            //openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Filter = "All Files (*.*)|*.*";

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


        private async void btnStart_Click(object sender, EventArgs e)
        {
            string[] inputPaths = txtFilePath.Text.Split(';')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToArray();

            if (inputPaths.Length == 0)
            {
                MessageBox.Show("Please select files or folders to compress.");
                return;
            }

            List<string> allTxtFiles = new List<string>();

            foreach (var path in inputPaths)
            {
                if (Directory.Exists(path))
                {
                    string[] txtFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    allTxtFiles.AddRange(txtFiles);
                }
                else if (File.Exists(path))
                {
                    allTxtFiles.Add(path);
                }
            }

            if (allTxtFiles.Count == 0)
            {
                MessageBox.Show("No valid .txt files found.");
                return;
            }

            string outputPath = txtOutputPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Please select an output file path.");
                return;
            }
            string algo = GetSelectedAlgorithm();
            if (algo == "none")
            {
                var comparisonForm = new ComparisonForm();
                var results = CompressionComparer.CompareCompressionAlgorithms(allTxtFiles);
                comparisonForm.LoadResults(results);
                comparisonForm.ShowDialog();
                return;
            }


            var progressForm = new ProgressForm();
            progressForm.Show();
            progressForm.BringToFront();
            await Task.Delay(100);

            try
            {
                var progressReporter = new Progress<(int fileIndex, int totalFiles, int percent)>((data) =>
                {
                    var (fileIndex, totalFiles, percent) = data;
                    if (fileIndex < allTxtFiles.Count)
                    {
                        progressForm.UpdateProgress(fileIndex, totalFiles, percent, allTxtFiles[fileIndex]);
                    }
                });

                List<string> compressionReport = null;

                await Task.Run(() =>
                {
                    string baseFolder = (inputPaths.Length == 1 && Directory.Exists(inputPaths[0]))
                        ? inputPaths[0]
                        : Path.GetDirectoryName(allTxtFiles[0]);


                    string actualPassword = string.IsNullOrWhiteSpace(password) ? null : password;

                    compressionReport = FileEncoder.SaveEncodedFile(
                        archivePath: outputPath,
                        inputFilePaths: allTxtFiles,
                        algorithm: algo,
                        token: progressForm.Token,
                        pauseEvent: progressForm.PauseEvent,
                        baseFolder: baseFolder,
                        progress: progressReporter,
                        actualPassword
                    );
                    password = "";
                });

                MessageBox.Show("📦 Compression Summary:\n\n" + string.Join("\n", compressionReport));
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(outputPath))
                {
                    try { File.Delete(outputPath); }
                    catch { }
                }
                MessageBox.Show("Compression was cancelled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during compression:\n{ex.Message}");
            }
            finally
            {
                progressForm.AllowClose();
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
                    }
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    detxtOutputPath.Text = folderDialog.SelectedPath;
                }
            }
        }


        private void btnBrowseFolders_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to compress";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;

                    var existingPaths = txtFilePath.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (!existingPaths.Contains(selectedFolder))
                        existingPaths.Add(selectedFolder);

                    txtFilePath.Text = string.Join(";", existingPaths);

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

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnDecompress_Click_Click(object sender, EventArgs e)
        {
            string inputArchive = detxtFilePath.Text.Trim();
            string outputPath = detxtOutputPath.Text.Trim();
            bool decompressedAnyFile = false;

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

            var progressForm = new ProgressForm();
            progressForm.Show();
            progressForm.BringToFront();

            await Task.Delay(100); 

            try
            {
                var progressReporter = new Progress<(int fileIndex, int totalFiles, int percent)>(progress =>
                {
                    var (fileIndex, totalFiles, percent) = progress;
                    string fileName = fileIndex < selectedFileNames.Count ? selectedFileNames[fileIndex] : "Processing...";
                    progressForm.UpdateProgress(fileIndex, totalFiles, percent, fileName);
                });

                await Task.Run(() =>
                {
                    FileEncoder.DecodeFile(
                        archivePath: inputArchive,
                        outputPath: outputPath,
                        filesToExtract: selectedFileNames,
                        progress: progressReporter,
                        token: progressForm.Token,
                        pauseEvent: progressForm.PauseEvent
                    );
                });

                if (decompressedAnyFile)
                {
                    MessageBox.Show("Decompression complete.");
                }

            }
            catch (AggregateException aggEx) 
            {
                aggEx.Handle(e =>
                {
                    if (e is OperationCanceledException)
                    {
                        if (Directory.Exists(outputPath))
                        {
                            try { Directory.Delete(outputPath, true); }
                            catch { }
                        }
                        return true;
                    }
                    return false; 
                });
                MessageBox.Show("Decompression was cancelled."); 
            }
            catch (OperationCanceledException)
            {
                if (Directory.Exists(outputPath))
                {
                    try { Directory.Delete(outputPath, true); }
                    catch {  }
                }
                MessageBox.Show("Decompression was cancelled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during decompression:\n{ex.Message}");
            }
            finally
            {
                progressForm.AllowClose();
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
                item.Tag = entry; 
                item.Checked = true;  
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
                string originalFile = selectedItems[0].SubItems[0].Text;
                string dir = Path.GetDirectoryName(detxtFilePath.Text);
                string nameOnly = Path.GetFileNameWithoutExtension(originalFile);

                string ext = Path.GetExtension(originalFile);
                string outputFile = Path.Combine(dir, nameOnly + "_decompressed" + ext);
                
                detxtOutputPath.Text = outputFile;
            }
            else if (selectedItems.Count > 1)
            {
                string dir = Path.GetDirectoryName(detxtFilePath.Text);
                string folder = Path.Combine(dir, "Decompressed_" + Path.GetFileNameWithoutExtension(detxtFilePath.Text));
                detxtOutputPath.Text = folder;
            }
            btnDecompress_Click.Enabled = selectedItems.Count > 0;
        }


    }
}
