using FileCompressor;
using FileCompressorApp.Compression;
using FileCompressorApp.Helpers;
using FileCompressorApp.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FileCompressorApp.IO
{
    public static class FileEncoder
    {
        public static List<string> SaveEncodedFile(
     string archivePath,
     List<string> inputFilePaths,
     string algorithm,
     CancellationToken token,
     ManualResetEventSlim pauseEvent,
     string baseFolder = null,
     IProgress<(int fileIndex, int totalFiles, int percent)> progress = null,
     string password = null
     )
        {
            bool isEncrypted = !string.IsNullOrEmpty(password);
            List<string> compressionSummaries = new List<string>();

            using (var stream = File.OpenWrite(archivePath))
            using (var writer = new BinaryWriter(stream))
            {
                int totalFiles = inputFilePaths.Count;
                writer.Write(totalFiles);
                writer.Write(isEncrypted); 

                for (int i = 0; i < totalFiles; i++)
                {
                    token.ThrowIfCancellationRequested();
                    pauseEvent.Wait();

                    string filePath = inputFilePaths[i];
                    string text = File.ReadAllText(filePath);

                    string relativePath = !string.IsNullOrEmpty(baseFolder)
                        ? Path.GetRelativePath(baseFolder, filePath)
                        : Path.GetFileName(filePath);

                    writer.Write(relativePath.Length);
                    writer.Write(Encoding.UTF8.GetBytes(relativePath));

                    writer.Write(algorithm.Length);
                    writer.Write(Encoding.UTF8.GetBytes(algorithm));

                    writer.Write(text.Length);

                    string encodedBits = "";
                    byte[] encodedBytes = null;

                    if (algorithm == "shannon")
                    {
                        var symbols = ShannonFanoCompressor.BuildFrequencyTable(text);
                        ShannonFanoCompressor.BuildCodes(symbols);
                        writer.Write(symbols.Count);

                        StringBuilder builder = new StringBuilder();
                        int totalChars = text.Length;

                        for (int c = 0; c < totalChars; c++)
                        {
                            token.ThrowIfCancellationRequested();
                            pauseEvent.Wait();

                            char ch = text[c];
                            string code = symbols.First(s => s.Character == ch).Code;
                            builder.Append(code);

                            if (progress != null && (c % 100 == 0 || c == totalChars - 1))
                            {
                                int percent = (int)((c + 1) / (float)totalChars * 100);
                                progress.Report((i, totalFiles, percent));
                                Thread.Sleep(1);
                            }
                        }

                        encodedBits = builder.ToString();
                        encodedBytes = BitHelper.PackBits(encodedBits);

                        foreach (var symbol in symbols)
                        {
                            writer.Write(symbol.Character);
                            writer.Write(symbol.Code.Length);
                            var codeBytes = BitHelper.PackBits(symbol.Code);
                            writer.Write(codeBytes.Length);
                            writer.Write(codeBytes);
                        }
                    }
                    else if (algorithm == "huffman")
                    {
                        var freqTable = HuffmanCompressor.BuildFrequencyTable(text);
                        var root = HuffmanCompressor.BuildTree(freqTable);
                        var codeTable = HuffmanCompressor.BuildCodeTable(root);
                        writer.Write(codeTable.Count);

                        StringBuilder builder = new StringBuilder();
                        int totalChars = text.Length;

                        for (int c = 0; c < totalChars; c++)
                        {
                            token.ThrowIfCancellationRequested();
                            pauseEvent.Wait();

                            char ch = text[c];
                            builder.Append(codeTable[ch]);

                            if (progress != null && (c % 100 == 0 || c == totalChars - 1))
                            {
                                int percent = (int)((c + 1) / (float)totalChars * 100);
                                progress.Report((i, totalFiles, percent));
                                Thread.Sleep(1);
                            }
                        }

                        encodedBits = builder.ToString();
                        encodedBytes = BitHelper.PackBits(encodedBits);

                        foreach (var pair in codeTable)
                        {
                            writer.Write(pair.Key);
                            writer.Write(pair.Value.Length);
                            var codeBytes = BitHelper.PackBits(pair.Value);
                            writer.Write(codeBytes.Length);
                            writer.Write(codeBytes);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported algorithm: " + algorithm);
                    }

                    writer.Write(encodedBits.Length);

                    if (isEncrypted)
                    {
                        encodedBytes = AesEncryptionHelper.Encrypt(encodedBytes, password); // ➕
                    }
                    writer.Write(encodedBytes.Length);
                    writer.Write(encodedBytes);

                    int originalSize = Encoding.UTF8.GetByteCount(text);
                    int compressedSize = encodedBytes.Length;
                    double ratio = 100.0 * (originalSize - compressedSize) / originalSize;
                    string summary = $"{relativePath}: {originalSize} → {compressedSize} bytes | Saved: {ratio:F2}%";
                    compressionSummaries.Add(summary);

                    progress?.Report((i, totalFiles, 100));
                }
            }

            return compressionSummaries;
        }

        public static bool DecodeFile(
            string archivePath,
            string outputPath,
            List<string> filesToExtract,
            CancellationToken token,
            ManualResetEventSlim pauseEvent,
            IProgress<(int fileIndex, int totalFiles, int percent)> progress = null)
        {
            bool successAny = false;
            using (var stream = File.OpenRead(archivePath))
            using (var reader = new BinaryReader(stream))
            {
                int fileCount = reader.ReadInt32();

                bool isEncrypted = reader.ReadBoolean();

                string password = null;
                if (isEncrypted)
                {
                    var passwordForm = new PasswordPromptForm();
                    passwordForm.TopMost = true;
                    if (passwordForm.ShowDialog() == DialogResult.OK)
                    {
                        password = passwordForm.Password;
                        if (string.IsNullOrEmpty(password))
                        {
                            MessageBox.Show("Password required.");
                            return successAny;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Operation cancelled.");
                        return successAny;
                    }
                }

                for (int f = 0; f < fileCount; f++)
                {
                    token.ThrowIfCancellationRequested();
                    pauseEvent.Wait();

                    int fileNameLength = reader.ReadInt32();
                    string relativePath = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                    int algoLength = reader.ReadInt32();
                    string algorithm = Encoding.UTF8.GetString(reader.ReadBytes(algoLength));

                    int originalLength = reader.ReadInt32();

                    int symbolCount = reader.ReadInt32();
                    List<Symbol> symbols = new List<Symbol>();
                    Dictionary<char, string> codeTable = new Dictionary<char, string>();

                    for (int i = 0; i < symbolCount; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        pauseEvent.Wait();

                        char ch = reader.ReadChar();
                        int codeLength = reader.ReadInt32();
                        int codeByteLength = reader.ReadInt32();
                        byte[] codeBytes = reader.ReadBytes(codeByteLength);
                        string code = BitHelper.UnpackBits(codeBytes, codeLength);

                        if (algorithm == "huffman")
                            codeTable[ch] = code;
                        else
                            symbols.Add(new Symbol { Character = ch, Code = code });
                    }

                    int encodedBitCount = reader.ReadInt32();
                    int encodedByteCount = reader.ReadInt32();
                    byte[] encodedBytes = reader.ReadBytes(encodedByteCount);

                    //string encodedBits = BitHelper.UnpackBits(encodedBytes, encodedBitCount);
                    if (isEncrypted)
                    {
                        try
                        {
                            encodedBytes = AesEncryptionHelper.Decrypt(encodedBytes, password, out bool success);
                            if (!success)
                            {
                                MessageBox.Show($"Wrong password for file: {relativePath}");
                                continue; // Skip this file
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"Wrong password or corrupted data for file: {relativePath}");
                            continue;
                        }
                    }

                    string encodedBits = BitHelper.UnpackBits(encodedBytes, encodedBitCount);

                    if (filesToExtract.Contains(relativePath))
                    {
                        string decodedText = "";

                        if (algorithm == "huffman")
                        {
                            HuffmanNode root = RebuildHuffmanTree(codeTable);
                            StringBuilder decodedBuilder = new StringBuilder();
                            HuffmanNode current = root;
                            int totalBits = encodedBits.Length;

                            for (int i = 0; i < totalBits; i++)
                            {
                                token.ThrowIfCancellationRequested();
                                pauseEvent.Wait();

                                char bit = encodedBits[i];
                                current = (bit == '0') ? current.Left : current.Right;

                                if (current.Left == null && current.Right == null)
                                {
                                    decodedBuilder.Append(current.Character);
                                    current = root;
                                }

                                if (progress != null && (i % 100 == 0 || i == totalBits - 1))
                                {
                                    int percent = (int)((i + 1) / (float)totalBits * 100);
                                    progress.Report((f, fileCount, percent));
                                }
                            }

                            decodedText = decodedBuilder.ToString();
                        }
                        else if (algorithm == "shannon")
                        {
                            var progressForFile = new Progress<int>(percent =>
                            {
                                progress?.Report((f, fileCount, percent));
                            });

                            decodedText = ShannonFanoCompressor.Decode(encodedBits, symbols, progressForFile, token, pauseEvent);
                        }
                        else
                        {
                            throw new Exception("Unsupported algorithm: " + algorithm);
                        }

                        string fullOutputPath = Path.Combine(outputPath, relativePath);
                        string outputDir = Path.GetDirectoryName(fullOutputPath);
                        if (!Directory.Exists(outputDir))
                            Directory.CreateDirectory(outputDir);

                        File.WriteAllText(fullOutputPath, decodedText);
                        Debug.WriteLine($"Extracted: {fullOutputPath}");
                        successAny = true;
                    }
                    else
                    {
                        progress?.Report((f, fileCount, 100));
                    }
                }
            }
            return successAny;
        }


        private static HuffmanNode RebuildHuffmanTree(Dictionary<char, string> codeTable)
        {
            HuffmanNode root = new HuffmanNode();

            foreach (var kv in codeTable)
            {
                char ch = kv.Key;
                string code = kv.Value;
                HuffmanNode current = root;

                foreach (char bit in code)
                {
                    if (bit == '0')
                    {
                        if (current.Left == null)
                            current.Left = new HuffmanNode();
                        current = current.Left;
                    }
                    else if (bit == '1')
                    {
                        if (current.Right == null)
                            current.Right = new HuffmanNode();
                        current = current.Right;
                    }
                }

                current.Character = ch;
            }

            return root;
        }

  
    }
}