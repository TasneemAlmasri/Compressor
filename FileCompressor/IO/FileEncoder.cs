using FileCompressor;
using FileCompressorApp.Compression;
using FileCompressorApp.Helpers;
using FileCompressorApp.Models;
using System.Diagnostics;
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
            List<Task> compressionTasks = new List<Task>(); 


            using (var stream = File.OpenWrite(archivePath))
            using (var writer = new BinaryWriter(stream))
            {
                int totalFiles = inputFilePaths.Count;
                writer.Write(totalFiles);
                writer.Write(isEncrypted);

                object writeLock = new object(); 

                for (int i = 0; i < totalFiles; i++)
                {
                    int fileIndex = i;
                    string filePath = inputFilePaths[i];
                    var task = Task.Run(() => 
                    {
                        try  
                        {
                            token.ThrowIfCancellationRequested();
                            pauseEvent.Wait();

                            //string text = File.ReadAllText(filePath);
                            byte[] text = File.ReadAllBytes(filePath); // ➕fileBytes

                            string relativePath = !string.IsNullOrEmpty(baseFolder)
                                ? Path.GetRelativePath(baseFolder, filePath)
                                : Path.GetFileName(filePath);

                            string encodedBits = "";
                            byte[] encodedBytes = null;

                            List<Symbol> symbols = null;
                            Dictionary<byte, string> codeTable = null;// ➕

                            using var memStream = new MemoryStream();
                            using var memWriter = new BinaryWriter(memStream);

                            memWriter.Write(relativePath.Length);
                            memWriter.Write(Encoding.UTF8.GetBytes(relativePath));
                            memWriter.Write(algorithm.Length);
                            memWriter.Write(Encoding.UTF8.GetBytes(algorithm));
                            memWriter.Write(text.Length);


                            if (algorithm == "shannon")
                            {
                                symbols = ShannonFanoCompressor.BuildFrequencyTable(text);
                                ShannonFanoCompressor.BuildCodes(symbols);
                                memWriter.Write(symbols.Count);

                                StringBuilder builder = new StringBuilder();
                                int totalChars = text.Length;

                                for (int c = 0; c < totalChars; c++)
                                {
                                    token.ThrowIfCancellationRequested();
                                    pauseEvent.Wait();

                                    //char ch = text[c];
                                    byte ch = text[c];// ➕

                                    string code = symbols.First(s => s.Character == ch).Code;
                                    builder.Append(code);

                                    if (progress != null && (c % 100 == 0 || c == totalChars - 1))
                                    {
                                        int percent = (int)((c + 1) / (float)totalChars * 100);
                                        progress.Report((fileIndex, totalFiles, percent));
                                        Thread.Sleep(1);
                                    }
                                }

                                encodedBits = builder.ToString();
                                encodedBytes = BitHelper.PackBits(encodedBits);

                                foreach (var symbol in symbols)
                                {
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
                                memWriter.Write(codeTable.Count); 

                                StringBuilder builder = new StringBuilder();

                                //int totalChars = text.Length;
                                int totalChars = text.Length; // ➕totalBytes

                                for (int c = 0; c < totalChars; c++)
                                {
                                    token.ThrowIfCancellationRequested();
                                    pauseEvent.Wait();

                                    //char ch = text[c];
                                    byte ch = text[c];// ➕

                                    builder.Append(codeTable[ch]);

                                    if (progress != null && (c % 100 == 0 || c == totalChars - 1))
                                    {
                                        int percent = (int)((c + 1) / (float)totalChars * 100);
                                        progress.Report((fileIndex, totalFiles, percent));
                                        Thread.Sleep(1);
                                    }
                                }

                                encodedBits = builder.ToString();
                                encodedBytes = BitHelper.PackBits(encodedBits);

                                foreach (var pair in codeTable)
                                {
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

                            memWriter.Write(encodedBits.Length);

                            if (isEncrypted)
                            {
                                encodedBytes = AesEncryptionHelper.Encrypt(encodedBytes, password);
                            }
                            memWriter.Write(encodedBytes.Length);
                            memWriter.Write(encodedBytes);

                            //int originalSize = Encoding.UTF8.GetByteCount(text);
                            int originalSize = text.Length; // ➕
                            int compressedSize = encodedBytes.Length;
                            double ratio = 100.0 * (originalSize - compressedSize) / originalSize;
                            string summary = $"{relativePath}: {originalSize} → {compressedSize} bytes | Saved: {ratio:F2}%";

                            lock (writeLock)
                            {
                                writer.Write(memStream.ToArray()); 
                                compressionSummaries.Add(summary);
                            }

                            progress?.Report((fileIndex, totalFiles, 100));
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                    }); 
                    compressionTasks.Add(task);
                }

                try
                {
                    Task.WaitAll(compressionTasks.ToArray());
                }
                catch (AggregateException ex) 
                {
                    ex.Handle(e => e is OperationCanceledException);
                    throw new OperationCanceledException();
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

            List<Task> tasks = new List<Task>();
            object writeLock = new object(); 

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

                long[] entryPositions = new long[fileCount]; 

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
                        //reader.ReadChar();
                        reader.ReadByte(); // ➕
                        int len = reader.ReadInt32();
                        int byteLen = reader.ReadInt32();
                        reader.BaseStream.Seek(byteLen, SeekOrigin.Current);
                    }

                    reader.ReadInt32(); // encodedBitCount
                    int encodedByteCount = reader.ReadInt32();
                    reader.BaseStream.Seek(encodedByteCount, SeekOrigin.Current);
                }

                for (int i = 0; i < fileCount; i++)
                {
                    int fileIndex = i;
                    long filePosition = entryPositions[i];

                    var task = Task.Run(() =>
                    {

                        token.ThrowIfCancellationRequested();
                        pauseEvent.Wait();

                        using var fs = File.OpenRead(archivePath); 
                        using var localReader = new BinaryReader(fs); 
                        fs.Seek(filePosition, SeekOrigin.Begin); 


                        int fileNameLength = localReader.ReadInt32();
                        string relativePath = Encoding.UTF8.GetString(localReader.ReadBytes(fileNameLength));

                        int algoLength = localReader.ReadInt32();
                        string algorithm = Encoding.UTF8.GetString(localReader.ReadBytes(algoLength));

                        int originalLength = localReader.ReadInt32();

                        int symbolCount = localReader.ReadInt32();
                        List<Symbol> symbols = new List<Symbol>();

                        //Dictionary<char, string> codeTable = new Dictionary<char, string>();
                        Dictionary<byte, string> codeTable = new Dictionary<byte, string>(); // ➕

                        for (int j = 0; j < symbolCount; j++)
                        {
                            //char ch = localReader.ReadChar();
                            byte ch = localReader.ReadByte();

                            int codeLength = localReader.ReadInt32();               
                            int codeByteLength = localReader.ReadInt32();
                            byte[] codeBytes = localReader.ReadBytes(codeByteLength);
                            string code = BitHelper.UnpackBits(codeBytes, codeLength);

                            if (algorithm == "huffman")
                                codeTable[ch] = code;
                            else
                                symbols.Add(new Symbol { Character = ch, Code = code });
                        }
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
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Wrong password or corrupted data for file: {relativePath}");
                                return;
                            }
                        }

                        string encodedBits = BitHelper.UnpackBits(encodedBytes, encodedBitCount);

                        if (filesToExtract.Contains(relativePath))
                        {
                            //string decodedText = "";
                            byte[] decodedText = null; // ➕ decodedBytes 

                            if (algorithm == "huffman")
                            {
                                HuffmanNode root = RebuildHuffmanTree(codeTable);

                                //StringBuilder decodedBuilder = new StringBuilder();
                                List<byte> byteList = new List<byte>();// ➕

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
                                        //decodedBuilder.Append(current.Character);
                                        byteList.Add(current.Character.Value); // ➕

                                        current = root;
                                    }

                                    if (progress != null && (i % 100 == 0 || i == totalBits - 1))
                                    {
                                        int percent = (int)((i + 1) / (float)totalBits * 100);
                                        progress.Report((fileIndex, fileCount, percent));
                                    }
                                }

                                //decodedText = decodedBuilder.ToString();
                                decodedText = byteList.ToArray(); // ➕decodedBytes 
                            }
                            else if (algorithm == "shannon")
                            {
                                var progressForFile = new Progress<int>(percent =>
                                {
                                    progress?.Report((fileIndex, fileCount, percent));
                                });

                                //string decodedStr = ShannonFanoCompressor.Decode(encodedBits, symbols, progressForFile, token, pauseEvent);
                                //decodedText = decodedStr.Select(c => (byte)c).ToArray(); 
                                decodedText = ShannonFanoCompressor.Decode(encodedBits, symbols, progressForFile, token, pauseEvent);// ➕
                            }
                            else
                            {
                                throw new Exception("Unsupported algorithm: " + algorithm);
                            }

                            string fullOutputPath;
                            lock (writeLock)      
                            {

                                fullOutputPath = Path.Combine(outputPath, relativePath);
                                string outputDir = Path.GetDirectoryName(fullOutputPath);
                                if (!Directory.Exists(outputDir))
                                    Directory.CreateDirectory(outputDir);
                            }


                            //File.WriteAllText(fullOutputPath, decodedText);
                            File.WriteAllBytes(fullOutputPath, decodedText); // ➕

                            Debug.WriteLine($"Extracted: {fullOutputPath}");

                            lock (writeLock) successAny = true;
                        }
                        else
                        {
                            progress?.Report((fileIndex, fileCount, 100));
                        }
                    }); 
                    tasks.Add(task); 
                }

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

            }
            return successAny;
        }



        private static HuffmanNode RebuildHuffmanTree(Dictionary<byte, string> codeTable)// ➕
        {
            HuffmanNode root = new HuffmanNode();

            foreach (var kv in codeTable)
            {
                //char ch = kv.Key;
                byte ch = kv.Key;// ➕

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