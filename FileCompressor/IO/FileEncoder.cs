using FileCompressorApp.Models;
using FileCompressorApp.Compression;
using FileCompressorApp.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FileCompressorApp.IO
{
    public static class FileEncoder
    {
        //public static void SaveEncodedFile(string path, List<Symbol> symbols, string encodedBits)
        //{
        //    using (var stream = File.OpenWrite(path))
        //    using (var writer = new BinaryWriter(stream))
        //    {
        //        writer.Write(symbols.Count);

        //        foreach (var symbol in symbols)
        //        {
        //            writer.Write(symbol.Character);
        //            writer.Write(symbol.Code.Length);

        //            byte[] codeBytes = BitHelper.PackBits(symbol.Code);
        //            writer.Write(codeBytes.Length);
        //            writer.Write(codeBytes);
        //        }

        //        writer.Write(encodedBits.Length);
        //        byte[] encodedBytes = BitHelper.PackBits(encodedBits);
        //        writer.Write(encodedBytes.Length);
        //        writer.Write(encodedBytes);
        //    }
        //}

        public static void SaveEncodedFile(string archivePath, List<string> inputFilePaths, string baseFolder)
        {
            using (var stream = File.OpenWrite(archivePath))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(inputFilePaths.Count);

                foreach (var filePath in inputFilePaths)
                {
                    string text = File.ReadAllText(filePath);

                    // Compute relative path from baseFolder
                    string relativePath = Path.GetRelativePath(baseFolder, filePath);

                    // Build frequency table & codes
                    List<Symbol> symbols = ShannonFanoCompressor.BuildFrequencyTable(text);
                    ShannonFanoCompressor.BuildCodes(symbols);

                    string encodedBits = ShannonFanoCompressor.Encode(text, symbols);
                    byte[] encodedBytes = BitHelper.PackBits(encodedBits);

                    // Write relative file name
                    writer.Write(relativePath.Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(relativePath));

                    // Write algorithm
                    string algorithm = "shannon";
                    writer.Write(algorithm.Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(algorithm));

                    // Original text size
                    writer.Write(text.Length);

                    // Write symbol table
                    writer.Write(symbols.Count);
                    foreach (var symbol in symbols)
                    {
                        writer.Write(symbol.Character);
                        writer.Write(symbol.Code.Length);
                        byte[] codeBytes = BitHelper.PackBits(symbol.Code);
                        writer.Write(codeBytes.Length);
                        writer.Write(codeBytes);
                    }

                    writer.Write(encodedBits.Length);
                    writer.Write(encodedBytes.Length);
                    writer.Write(encodedBytes);
                }
            }
        }


        //public static void DecodeFile(string inputPath, string outputPath)
        //{
        //    using (var stream = File.OpenRead(inputPath))
        //    using (var reader = new BinaryReader(stream))
        //    {
        //        int symbolCount = reader.ReadInt32();
        //        List<Symbol> symbols = new List<Symbol>();

        //        for (int i = 0; i < symbolCount; i++)
        //        {
        //            char ch = reader.ReadChar();
        //            int codeLength = reader.ReadInt32();
        //            int byteLen = reader.ReadInt32();
        //            byte[] codeBytes = reader.ReadBytes(byteLen);

        //            string code = BitHelper.UnpackBits(codeBytes, codeLength);
        //            symbols.Add(new Symbol { Character = ch, Code = code });
        //        }

        //        int encodedBitCount = reader.ReadInt32();
        //        int encodedByteCount = reader.ReadInt32();
        //        byte[] encodedBytes = reader.ReadBytes(encodedByteCount);

        //        string encodedBits = BitHelper.UnpackBits(encodedBytes, encodedBitCount);
        //        string decodedText = ShannonFanoCompressor.Decode(encodedBits, symbols);

        //        File.WriteAllText(outputPath, decodedText);
        //        Debug.WriteLine($"Decoded file saved to: {outputPath}");
        //    }
        //}
        public static void DecodeFile(string archivePath, string outputPath, List<string> filesToExtract)
        {
            using (var stream = File.OpenRead(archivePath))
            using (var reader = new BinaryReader(stream))
            {
                int fileCount = reader.ReadInt32();

                for (int f = 0; f < fileCount; f++)
                {
                    // Read file name (this includes the relative path)
                    int fileNameLength = reader.ReadInt32();
                    string relativePath = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                    // Read algorithm
                    int algoLength = reader.ReadInt32();
                    string algorithm = Encoding.UTF8.GetString(reader.ReadBytes(algoLength));

                    // Original file size
                    int originalLength = reader.ReadInt32();

                    // Read symbols
                    int symbolCount = reader.ReadInt32();
                    List<Symbol> symbols = new List<Symbol>();
                    for (int i = 0; i < symbolCount; i++)
                    {
                        char ch = reader.ReadChar();
                        int codeLength = reader.ReadInt32();
                        int codeByteLength = reader.ReadInt32();
                        byte[] codeBytes = reader.ReadBytes(codeByteLength);
                        string code = BitHelper.UnpackBits(codeBytes, codeLength);
                        symbols.Add(new Symbol { Character = ch, Code = code });
                    }

                    // Read encoded data
                    int bitCount = reader.ReadInt32();
                    int byteCount = reader.ReadInt32();
                    byte[] encodedBytes = reader.ReadBytes(byteCount);
                    string encodedBits = BitHelper.UnpackBits(encodedBytes, bitCount);

                    // Match against selected files (based on relative path only)
                    if (filesToExtract.Contains(relativePath))
                    {
                        string decodedText = ShannonFanoCompressor.Decode(encodedBits, symbols);

                        // Determine output file path
                        string outputFile = Path.Combine(outputPath, relativePath);
                        string outputDir = Path.GetDirectoryName(outputFile);

                        if (!Directory.Exists(outputDir))
                            Directory.CreateDirectory(outputDir);

                        File.WriteAllText(outputFile, decodedText);
                        Debug.WriteLine($"Extracted: {outputFile}");
                    }
                }
            }
        }


        public static void SaveEncodedFileHuffman(string path, Dictionary<char, string> codeTable, string encodedBits)
        {
            using (var stream = File.OpenWrite(path))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(codeTable.Count);

                foreach (var entry in codeTable)
                {
                    writer.Write(entry.Key);                 // character
                    writer.Write(entry.Value.Length);        // code length

                    byte[] codeBytes = BitHelper.PackBits(entry.Value);
                    writer.Write(codeBytes.Length);          // byte count
                    writer.Write(codeBytes);                 // packed code
                }

                writer.Write(encodedBits.Length);
                byte[] encodedBytes = BitHelper.PackBits(encodedBits);
                writer.Write(encodedBytes.Length);
                writer.Write(encodedBytes);
            }
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

        public static void DecodeFileHuffman(string inputPath, string outputPath)
        {
            using (var stream = File.OpenRead(inputPath))
            using (var reader = new BinaryReader(stream))
            {
                int symbolCount = reader.ReadInt32();
                Dictionary<char, string> codeTable = new Dictionary<char, string>();

                for (int i = 0; i < symbolCount; i++)
                {
                    char ch = reader.ReadChar();
                    int codeLength = reader.ReadInt32();
                    int byteLen = reader.ReadInt32();
                    byte[] codeBytes = reader.ReadBytes(byteLen);
                    string code = BitHelper.UnpackBits(codeBytes, codeLength);

                    codeTable[ch] = code;
                }

                int encodedBitCount = reader.ReadInt32();
                int encodedByteCount = reader.ReadInt32();
                byte[] encodedBytes = reader.ReadBytes(encodedByteCount);

                string encodedBits = BitHelper.UnpackBits(encodedBytes, encodedBitCount);

                // إعادة بناء الشجرة
                HuffmanNode root = RebuildHuffmanTree(codeTable);
                string decodedText = HuffmanCompressor.Decode(encodedBits, root);

                File.WriteAllText(outputPath, decodedText);
                Debug.WriteLine($"Decoded Huffman file saved to: {outputPath}");
            }
        }

    }
}