using FileCompressor;
using FileCompressorApp.Helpers;
using FileCompressorApp.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FileCompressorApp
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 2)
            {
                string action = args[0].ToLower();
                string filePath = args[1];

                if (!File.Exists(filePath) && !Directory.Exists(filePath))
                {
                    MessageBox.Show("🚫 الملف غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var progressForm = new ProgressForm();
                progressForm.Show();
                progressForm.BringToFront();

                try
                {
                    if (action == "compress")
                    {
                        string archivePath = Path.ChangeExtension(filePath, ".bin");

                        List<string> filesToCompress = new List<string>();

                        if (File.Exists(filePath))
                        {
                           
                            filesToCompress.Add(filePath);
                        }
                        else if (Directory.Exists(filePath))
                        {
                        
                            filesToCompress.AddRange(Directory.GetFiles(filePath, "*", SearchOption.AllDirectories));
                        }

                        var progressReporter = new Progress<(int fileIndex, int totalFiles, int percent)>(progress =>
                        {
                            var (fileIndex, totalFiles, percent) = progress;
                            string currentFile = fileIndex < filesToCompress.Count ? filesToCompress[fileIndex] : filePath;
                            progressForm.UpdateProgress(fileIndex, totalFiles, percent, currentFile);
                        });

                        List<string> compressionReport = null;

                        Task.Run(() =>
                        {
                            compressionReport = FileEncoder.SaveEncodedFile(
                                archivePath,
                                filesToCompress,
                                "huffman",
                                CancellationToken.None,
                                progressForm.PauseEvent,
                                baseFolder: filePath,  
                                progressReporter
                            );
                        }).Wait();

                        MessageBox.Show("📦 Compression Summary:\n\n" + string.Join("\n", compressionReport));
                    }

                    else if (action == "decompress")
                    {
                        string outputDir = Path.Combine(Path.GetDirectoryName(filePath),
                            Path.GetFileNameWithoutExtension(filePath) + "_extracted");

                        if (!Directory.Exists(outputDir))
                            Directory.CreateDirectory(outputDir);

                        var progressReporter = new Progress<(int fileIndex, int totalFiles, int percent)>(progress =>
                        {
                            var (fileIndex, totalFiles, percent) = progress;
                            progressForm.UpdateProgress(fileIndex, totalFiles, percent, filePath);
                        });

                        bool success = false;
                        Task.Run(() =>
                        {
                            success = FileEncoder.DecodeFile(
                                filePath,
                                outputDir,
                                filesToExtract: null,
                                CancellationToken.None,
                                progressForm.PauseEvent,
                                progressReporter
                            );
                        }).Wait();

                        if (success)
                        {
                            MessageBox.Show("Decompression complete.");
                            Process.Start("explorer.exe", outputDir);
                        }
                        else
                        {
                            MessageBox.Show("⚠️ لم يتم فك أي ملف. تأكد من أن الملف صالح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("🚫 أمر غير معروف. استخدم compress أو decompress", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (OperationCanceledException)
                {
                 
                    string outputPath = action == "compress"
                        ? Path.ChangeExtension(filePath, ".bin")
                        : Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_extracted");

                    if (action == "compress" && File.Exists(outputPath))
                    {
                        try { File.Delete(outputPath); }
                        catch { }
                        MessageBox.Show("Compression was cancelled.");
                    }
                    else if (action == "decompress" && Directory.Exists(outputPath))
                    {
                        try { Directory.Delete(outputPath, true); }
                        catch { }
                        MessageBox.Show("Decompression was cancelled.");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ حدث خطأ غير متوقع:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.AllowClose();
                    progressForm.Close();
                }

                return;
            }
            Application.Run(new Form1());
        }

    }
}
