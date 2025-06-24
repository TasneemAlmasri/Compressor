using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using FileCompressor;

namespace FileCompressorApp
{
    class Symbol
    {
        public char Character;
        public int Frequency;
        public string Code = "";
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static List<Symbol> BuildFrequencyTable(string text)
        {
            Dictionary<char, int> freqMap = new Dictionary<char, int>();
            foreach (char c in text)
            {
                if (freqMap.ContainsKey(c))
                    freqMap[c]++;
                else
                    freqMap[c] = 1;
            }

            return freqMap.Select(kv => new Symbol
            {
                Character = kv.Key,
                Frequency = kv.Value
            }).OrderByDescending(s => s.Frequency).ToList();
        }

        public static void BuildShannonFanoCodes(List<Symbol> symbols)
        {
            void RecursiveSplit(List<Symbol> list)
            {
                if (list.Count <= 1) return;

                int total = list.Sum(s => s.Frequency);
                int half = total / 2;

                int sum = 0;
                int splitIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    sum += list[i].Frequency;
                    if (sum >= half)
                    {
                        splitIndex = i;
                        break;
                    }
                }

                for (int i = 0; i <= splitIndex; i++)
                    list[i].Code += "0";
                for (int i = splitIndex + 1; i < list.Count; i++)
                    list[i].Code += "1";

                RecursiveSplit(list.GetRange(0, splitIndex + 1));
                RecursiveSplit(list.GetRange(splitIndex + 1, list.Count - (splitIndex + 1)));
            }

            RecursiveSplit(symbols);
        }

        public static string EncodeText(string text, List<Symbol> symbols)
        {
            Dictionary<char, string> codeMap = symbols.ToDictionary(s => s.Character, s => s.Code);
            return string.Concat(text.Select(c => codeMap[c]));
        }


        // Converts a string of '0' and '1' to a byte array packed with bits
        public static byte[] PackBits(string bits)
        {
            int byteCount = (bits.Length + 7) / 8;
            byte[] bytes = new byte[byteCount];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    int byteIndex = i / 8;
                    int bitIndex = 7 - (i % 8); // store bits from MSB to LSB in each byte
                    bytes[byteIndex] |= (byte)(1 << bitIndex);
                }
            }

            return bytes;
        }

        public static void SaveEncodedFile(string archivePath, List<string> inputFilePaths)
        {
            using (var stream = File.OpenWrite(archivePath))
            using (var writer = new BinaryWriter(stream))
            {
                // Write total number of files in archive
                writer.Write(inputFilePaths.Count);

                foreach (var filePath in inputFilePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    string text = File.ReadAllText(filePath);

                    // Build frequency table & codes
                    List<Symbol> symbols = BuildFrequencyTable(text);
                    BuildShannonFanoCodes(symbols);

                    // Encode text
                    string encodedBits = EncodeText(text, symbols);
                    byte[] encodedBytes = PackBits(encodedBits);

                    // Write file name length and file name
                    writer.Write(fileName.Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(fileName));

                    // Write algorithm used (for now "shannon")
                    string algorithm = "shannon";
                    writer.Write(algorithm.Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(algorithm));

                    // Write original file length in characters
                    writer.Write(text.Length);

                    // Write number of symbols
                    writer.Write(symbols.Count);

                    // Write each symbol info
                    foreach (var symbol in symbols)
                    {
                        writer.Write(symbol.Character);
                        writer.Write(symbol.Code.Length);
                        byte[] codeBytes = PackBits(symbol.Code);
                        writer.Write(codeBytes.Length);
                        writer.Write(codeBytes);
                    }

                    // Write length of encoded data in bits
                    writer.Write(encodedBits.Length);

                    // Write length of encoded data in bytes
                    writer.Write(encodedBytes.Length);

                    // Write encoded data bytes
                    writer.Write(encodedBytes);
                }
            }
        }


        /// ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string DecodeText(string encoded, List<Symbol> symbols)
        {
            Dictionary<string, char> reverseMap = symbols.ToDictionary(s => s.Code, s => s.Character);

            string result = "";
            string buffer = "";

            foreach (char bit in encoded)
            {
                buffer += bit;

                if (reverseMap.ContainsKey(buffer))
                {
                    result += reverseMap[buffer];
                    buffer = ""; // reset buffer after match
                }
            }
            return result;
        }

        public static string UnpackBits(byte[] bytes, int bitCount)
        {
            var bits = new System.Text.StringBuilder();

            for (int i = 0; i < bitCount; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                bool bit = (bytes[byteIndex] & (1 << bitIndex)) != 0;
                bits.Append(bit ? '1' : '0');
            }

            return bits.ToString();
        }

        public static void DecodeFile(string archivePath, string outputPath, List<string> filesToExtract)
        {
            using (var stream = File.OpenRead(archivePath))
            using (var reader = new BinaryReader(stream))
            {
                int fileCount = reader.ReadInt32();

                for (int f = 0; f < fileCount; f++)
                {
                    // Read file name
                    int fileNameLength = reader.ReadInt32();
                    string fileName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                    // Read algorithm used
                    int algoLength = reader.ReadInt32();
                    string algorithm = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(algoLength));

                    // Read original text length (characters)
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
                        string code = UnpackBits(codeBytes, codeLength);
                        symbols.Add(new Symbol { Character = ch, Code = code });
                    }

                    // Read encoded data lengths
                    int encodedBitCount = reader.ReadInt32();
                    int encodedByteCount = reader.ReadInt32();
                    byte[] encodedBytes = reader.ReadBytes(encodedByteCount);
                    string encodedBits = UnpackBits(encodedBytes, encodedBitCount);

                    if (filesToExtract.Contains(fileName))
                    {
                        string decodedText = DecodeText(encodedBits, symbols);

                        string outputFilePath;

                        if (filesToExtract.Count == 1 && !Directory.Exists(outputPath))
                        {
                            // Single file output, outputPath is file path
                            outputFilePath = outputPath;
                        }
                        else
                        {
                            // Multiple files output, outputPath is folder path
                            if (!Directory.Exists(outputPath))
                                Directory.CreateDirectory(outputPath);

                            outputFilePath = Path.Combine(outputPath, fileName);
                        }

                        File.WriteAllText(outputFilePath, decodedText);
                        Debug.WriteLine($"Extracted: {outputFilePath}");
                    }
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ArchiveEntry
        {
            public string FileName;
            public string Algorithm;
            public int OriginalSize;
            public int CompressedSize;
            public long DataPosition;  // Byte offset in the stream
        }

        public static List<ArchiveEntry> ReadArchiveIndex(string archivePath)
        {
            List<ArchiveEntry> entries = new List<ArchiveEntry>();

            using (var stream = File.OpenRead(archivePath))
            using (var reader = new BinaryReader(stream))
            {
                int fileCount = reader.ReadInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    int fileNameLength = reader.ReadInt32();
                    string fileName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                    int algoLength = reader.ReadInt32();
                    string algorithm = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(algoLength));

                    int originalSize = reader.ReadInt32();
                    int symbolCount = reader.ReadInt32();

                    for (int s = 0; s < symbolCount; s++)
                    {
                        reader.ReadChar();              // char
                        int codeLength = reader.ReadInt32();
                        int codeByteLength = reader.ReadInt32();
                        reader.ReadBytes(codeByteLength);
                    }

                    int bitCount = reader.ReadInt32();
                    int byteCount = reader.ReadInt32();

                    long dataPos = stream.Position;
                    reader.BaseStream.Seek(byteCount, SeekOrigin.Current); // skip encoded data

                    entries.Add(new ArchiveEntry
                    {
                        FileName = fileName,
                        Algorithm = algorithm,
                        OriginalSize = originalSize,
                        CompressedSize = byteCount,
                        DataPosition = dataPos
                    });
                }
            }

            return entries;
        }











    }
}
