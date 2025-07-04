using FileCompressor;
using FileCompressorApp.Compression;
using FileCompressorApp.Helpers;
using FileCompressorApp.Models;
using System.Collections.Concurrent;
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
            List<Task> compressionTasks = new List<Task>(); // ➕


            using (var stream = File.OpenWrite(archivePath))
            using (var writer = new BinaryWriter(stream))
            {
                int totalFiles = inputFilePaths.Count;
                writer.Write(totalFiles);
                writer.Write(isEncrypted);

                object writeLock = new object(); // ➕ to avoid race on writer

                for (int i = 0; i < totalFiles; i++)
                {
                    int fileIndex = i;// ➕
                    string filePath = inputFilePaths[i];// ➕
                    var task = Task.Run(() => // ➕ Offload each file to a thread
                    {
                        try // ➕ 
                        {
                            token.ThrowIfCancellationRequested();
                            pauseEvent.Wait();

                            //string filePath = inputFilePaths[i];// ➕
                            string text = File.ReadAllText(filePath);

                            string relativePath = !string.IsNullOrEmpty(baseFolder)
                                ? Path.GetRelativePath(baseFolder, filePath)
                                : Path.GetFileName(filePath);

                            // ➕
                            //writer.Write(relativePath.Length);
                            //writer.Write(Encoding.UTF8.GetBytes(relativePath));

                            //writer.Write(algorithm.Length);
                            //writer.Write(Encoding.UTF8.GetBytes(algorithm));

                            //writer.Write(text.Length);
                            //done

                            string encodedBits = "";
                            byte[] encodedBytes = null;

                            //➕
                            List<Symbol> symbols = null;
                            Dictionary<char, string> codeTable = null;

                            using var memStream = new MemoryStream();
                            using var memWriter = new BinaryWriter(memStream);

                            memWriter.Write(relativePath.Length);
                            memWriter.Write(Encoding.UTF8.GetBytes(relativePath));
                            memWriter.Write(algorithm.Length);
                            memWriter.Write(Encoding.UTF8.GetBytes(algorithm));
                            memWriter.Write(text.Length);
                            //done


                            if (algorithm == "shannon")
                            {
                                symbols = ShannonFanoCompressor.BuildFrequencyTable(text);
                                ShannonFanoCompressor.BuildCodes(symbols);
                                memWriter.Write(symbols.Count);// ➕

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
                                        progress.Report((fileIndex, totalFiles, percent));// ➕
                                        Thread.Sleep(1);
                                    }
                                }

                                encodedBits = builder.ToString();
                                encodedBytes = BitHelper.PackBits(encodedBits);

                                foreach (var symbol in symbols)
                                {
                                    // ➕ memWriter 
                                    memWriter.Write(symbol.Character);
                                    memWriter.Write(symbol.Code.Length);
                                    var codeBytes = BitHelper.PackBits(symbol.Code);
                                    memWriter.Write(codeBytes.Length);
                                    memWriter.Write(codeBytes);
                                }
                            }
                            else if (algorithm == "huffman")
                            {
                                var freqTable = HuffmanCompressor.BuildFrequencyTable(text);
                                var root = HuffmanCompressor.BuildTree(freqTable);
                                codeTable = HuffmanCompressor.BuildCodeTable(root);
                                memWriter.Write(codeTable.Count);// ➕ 

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
                                        progress.Report((fileIndex, totalFiles, percent));// ➕
                                        Thread.Sleep(1);
                                    }
                                }

                                encodedBits = builder.ToString();
                                encodedBytes = BitHelper.PackBits(encodedBits);

                                foreach (var pair in codeTable)
                                {
                                    // ➕ mem
                                    memWriter.Write(pair.Key);
                                    memWriter.Write(pair.Value.Length);
                                    var codeBytes = BitHelper.PackBits(pair.Value);
                                    memWriter.Write(codeBytes.Length);
                                    memWriter.Write(codeBytes);
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Unsupported algorithm: " + algorithm);
                            }

                            memWriter.Write(encodedBits.Length);// ➕ 

                            if (isEncrypted)
                            {
                                encodedBytes = AesEncryptionHelper.Encrypt(encodedBytes, password); // ➕
                            }
                            // ➕  meme
                            memWriter.Write(encodedBytes.Length);
                            memWriter.Write(encodedBytes);

                            int originalSize = Encoding.UTF8.GetByteCount(text);
                            int compressedSize = encodedBytes.Length;
                            double ratio = 100.0 * (originalSize - compressedSize) / originalSize;
                            string summary = $"{relativePath}: {originalSize} → {compressedSize} bytes | Saved: {ratio:F2}%";

                            lock (writeLock) // ➕
                            {
                                writer.Write(memStream.ToArray());// ➕ 
                                compressionSummaries.Add(summary);
                            }

                            progress?.Report((fileIndex, totalFiles, 100));// ➕
                        }
                        // ➕ 
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                    });// ➕ 
                    compressionTasks.Add(task); // ➕ 
                }

                // ➕
                try
                {
                    Task.WaitAll(compressionTasks.ToArray());
                }
                catch (AggregateException ex) // ➕
                {
                    ex.Handle(e => e is OperationCanceledException); // ➕
                    throw new OperationCanceledException(); // ➕
                }
                //done


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

            List<Task> tasks = new List<Task>(); // ➕
            object writeLock = new object(); // ➕

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

                long[] entryPositions = new long[fileCount]; // ➕

                // ➕ new block below , Store each file block's starting position
                for (int i = 0; i < fileCount; i++)
                {
                    entryPositions[i] = reader.BaseStream.Position;

                    int fileNameLength = reader.ReadInt32();
                    reader.BaseStream.Seek(fileNameLength, SeekOrigin.Current);

                    int algoLength = reader.ReadInt32();
                    reader.BaseStream.Seek(algoLength, SeekOrigin.Current);

                    reader.ReadInt32(); // originalLength
                    int symbolCount = reader.ReadInt32();

                    for (int j = 0; j < symbolCount; j++)
                    {
                        reader.ReadChar();
                        int len = reader.ReadInt32();
                        int byteLen = reader.ReadInt32();
                        reader.BaseStream.Seek(byteLen, SeekOrigin.Current);
                    }

                    reader.ReadInt32(); // encodedBitCount
                    int encodedByteCount = reader.ReadInt32();
                    reader.BaseStream.Seek(encodedByteCount, SeekOrigin.Current);
                }
                // done

                for (int i = 0; i < fileCount; i++)
                {
                    // ➕
                    int fileIndex = i;
                    long filePosition = entryPositions[i];

                    var task = Task.Run(() =>
                    {
                        //done

                        token.ThrowIfCancellationRequested();
                        pauseEvent.Wait();

                        using var fs = File.OpenRead(archivePath); // ➕
                        using var localReader = new BinaryReader(fs); // ➕
                        fs.Seek(filePosition, SeekOrigin.Begin); // ➕


                        int fileNameLength = localReader.ReadInt32();// ➕ localReader
                        string relativePath = Encoding.UTF8.GetString(localReader.ReadBytes(fileNameLength));// ➕ localReader

                        int algoLength = localReader.ReadInt32();// ➕ localReader
                        string algorithm = Encoding.UTF8.GetString(localReader.ReadBytes(algoLength));// ➕ localReader

                        int originalLength = localReader.ReadInt32();// ➕ localReader

                        int symbolCount = localReader.ReadInt32();//➕ localReader
                        List<Symbol> symbols = new List<Symbol>();
                        Dictionary<char, string> codeTable = new Dictionary<char, string>();

                        for (int j = 0; j < symbolCount; j++)
                        {
                            //token.ThrowIfCancellationRequested();// ➕
                            //pauseEvent.Wait();// ➕

                            char ch = localReader.ReadChar();//➕ localReader
                            int codeLength = localReader.ReadInt32();//➕ localReader
                            int codeByteLength = localReader.ReadInt32();//➕ localReader
                            byte[] codeBytes = localReader.ReadBytes(codeByteLength);//➕ localReader
                            string code = BitHelper.UnpackBits(codeBytes, codeLength);

                            if (algorithm == "huffman")
                                codeTable[ch] = code;
                            else
                                symbols.Add(new Symbol { Character = ch, Code = code });
                        }
                        //➕ localReader
                        int encodedBitCount = localReader.ReadInt32();
                        int encodedByteCount = localReader.ReadInt32();
                        byte[] encodedBytes = localReader.ReadBytes(encodedByteCount);

                        if (isEncrypted)
                        {
                            try
                            {
                                encodedBytes = AesEncryptionHelper.Decrypt(encodedBytes, password, out bool success);
                                if (!success)
                                {
                                    MessageBox.Show($"Wrong password for file: {relativePath}");
                                    return;// ➕
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Wrong password or corrupted data for file: {relativePath}");
                                return;// ➕
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
                                        progress.Report((fileIndex, fileCount, percent));
                                    }
                                }

                                decodedText = decodedBuilder.ToString();
                            }
                            else if (algorithm == "shannon")
                            {
                                var progressForFile = new Progress<int>(percent =>
                                {
                                    progress?.Report((fileIndex, fileCount, percent));
                                });

                                decodedText = ShannonFanoCompressor.Decode(encodedBits, symbols, progressForFile, token, pauseEvent);
                            }
                            else
                            {
                                throw new Exception("Unsupported algorithm: " + algorithm);
                            }

                            string fullOutputPath;// ➕
                            lock (writeLock) // ➕
                            {

                                fullOutputPath = Path.Combine(outputPath, relativePath);
                                string outputDir = Path.GetDirectoryName(fullOutputPath);
                                if (!Directory.Exists(outputDir))
                                    Directory.CreateDirectory(outputDir);
                            }
                            File.WriteAllText(fullOutputPath, decodedText);
                            Debug.WriteLine($"Extracted: {fullOutputPath}");

                            //successAny = true;// ➕
                            lock (writeLock) successAny = true; // ➕
                        }
                        else
                        {
                            progress?.Report((fileIndex, fileCount, 100));
                        }
                    }); // ➕ end of task
                    tasks.Add(task); // ➕
                }

                // ➕
                try
                {
                    Task.WaitAll(tasks.ToArray()); 
                }
                catch (AggregateException aggEx)
                {
                    aggEx.Handle(e => 
                    {
                        if (e is OperationCanceledException)
                            return true; 
                        return false;  
                    });
                    throw; 
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                //done

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