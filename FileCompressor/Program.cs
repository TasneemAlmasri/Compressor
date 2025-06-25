using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using FileCompressor;

namespace FileCompressorApp
{
    //class Symbol
    //{
    //    public char Character;
    //    public int Frequency;e
    //    public string Code = "";
    //}

    internal static class Program
    {
        [STAThread]
        static void Main()
        {

            //string[] filePaths = new string[]
            //{
            //    @"C:\Users\TasneemMasri\Desktop\original.txt",
            //    @"C:\Users\TasneemMasri\Desktop\original 2.txt"
            //};


            //foreach (string filePath in filePaths)
            //{
            //    if (!File.Exists(filePath))
            //    {
            //        Debug.WriteLine($"File not found: {filePath}");
            //        continue;
            //    }

            //    string fileName = Path.GetFileNameWithoutExtension(filePath);
            //    string outputPath = Path.Combine(Path.GetDirectoryName(filePath), fileName + ".bin");
            //    string decodedPath = Path.Combine(Path.GetDirectoryName(filePath), fileName + "_decoded.txt");

            //    string text = File.ReadAllText(filePath);
            //    Debug.WriteLine($"Original Text ({fileName}):\n{text}");

            //    List<Symbol> symbols = BuildFrequencyTable(text);
            //    BuildShannonFanoCodes(symbols);

            //    string encoded = EncodeText(text, symbols);
            //    Debug.WriteLine($"Encoded ({fileName}): {encoded.Length} bits");

            //    SaveEncodedFile(outputPath, symbols, encoded);
            //    DecodeFile(outputPath, decodedPath);
            //}


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


        }

        //public static List<Symbol> BuildFrequencyTable(string text)
        //{
        //    Dictionary<char, int> freqMap = new Dictionary<char, int>();
        //    foreach (char c in text)
        //    {
        //        if (freqMap.ContainsKey(c))
        //            freqMap[c]++;
        //        else
        //            freqMap[c] = 1;
        //    }

        //    return freqMap.Select(kv => new Symbol
        //    {
        //        Character = kv.Key,
        //        Frequency = kv.Value
        //    }).OrderByDescending(s => s.Frequency).ToList();
        //}

        //public static void BuildShannonFanoCodes(List<Symbol> symbols)
        //{
        //    void RecursiveSplit(List<Symbol> list)
        //    {
        //        if (list.Count <= 1) return;

        //        int total = list.Sum(s => s.Frequency);
        //        int half = total / 2;

        //        int sum = 0;
        //        int splitIndex = 0;
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            sum += list[i].Frequency;
        //            if (sum >= half)
        //            {
        //                splitIndex = i;
        //                break;
        //            }
        //        }

        //        for (int i = 0; i <= splitIndex; i++)
        //            list[i].Code += "0";
        //        for (int i = splitIndex + 1; i < list.Count; i++)
        //            list[i].Code += "1";

        //        RecursiveSplit(list.GetRange(0, splitIndex + 1));
        //        RecursiveSplit(list.GetRange(splitIndex + 1, list.Count - (splitIndex + 1)));
        //    }

        //    RecursiveSplit(symbols);
        //}

        //public static string EncodeText(string text, List<Symbol> symbols)
        //{
        //    Dictionary<char, string> codeMap = symbols.ToDictionary(s => s.Character, s => s.Code);
        //    return string.Concat(text.Select(c => codeMap[c]));
        //}


        //// Converts a string of '0' and '1' to a byte array packed with bits
        //public static byte[] PackBits(string bits)
        //{
        //    int byteCount = (bits.Length + 7) / 8;
        //    byte[] bytes = new byte[byteCount];

        //    for (int i = 0; i < bits.Length; i++)
        //    {
        //        if (bits[i] == '1')
        //        {
        //            int byteIndex = i / 8;
        //            int bitIndex = 7 - (i % 8); // store bits from MSB to LSB in each byte
        //            bytes[byteIndex] |= (byte)(1 << bitIndex);
        //        }
        //    }

        //    return bytes;
        //}

        //public static void SaveEncodedFile(string path, List<Symbol> symbols, string encodedBits)
        //{
        //    using (var stream = File.OpenWrite(path))
        //    using (var writer = new BinaryWriter(stream))
        //    {
        //        // Write number of symbols
        //        writer.Write(symbols.Count);

        //        // Write each symbol info:
        //        // char (2 bytes), code length (int), code bits packed as bytes
        //        foreach (var symbol in symbols)
        //        {
        //            writer.Write(symbol.Character);
        //            writer.Write(symbol.Code.Length);

        //            byte[] codeBytes = PackBits(symbol.Code);
        //            writer.Write(codeBytes.Length);  // write how many bytes code takes
        //            writer.Write(codeBytes);         // write packed bits of code
        //        }

        //        // Write length of encoded data in bits
        //        writer.Write(encodedBits.Length);

        //        // Write encoded data bits packed as bytes
        //        byte[] encodedBytes = PackBits(encodedBits);
        //        writer.Write(encodedBytes.Length);
        //        writer.Write(encodedBytes);
        //    }

        //}


        ///// ////////////////////////////////////////////////////////////////////////////////////////////////////

        //public static string DecodeText(string encoded, List<Symbol> symbols)
        //{
        //    Dictionary<string, char> reverseMap = symbols.ToDictionary(s => s.Code, s => s.Character);

        //    string result = "";
        //    string buffer = "";

        //    foreach (char bit in encoded)
        //    {
        //        buffer += bit;

        //        if (reverseMap.ContainsKey(buffer))
        //        {
        //            result += reverseMap[buffer];
        //            buffer = ""; // reset buffer after match
        //        }
        //    }
        //    return result;
        //}

        //public static string UnpackBits(byte[] bytes, int bitCount)
        //{
        //    var bits = new System.Text.StringBuilder();

        //    for (int i = 0; i < bitCount; i++)
        //    {
        //        int byteIndex = i / 8;
        //        int bitIndex = 7 - (i % 8);
        //        bool bit = (bytes[byteIndex] & (1 << bitIndex)) != 0;
        //        bits.Append(bit ? '1' : '0');
        //    }

        //    return bits.ToString();
        //}

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
        //            string code = UnpackBits(codeBytes, codeLength);

        //            symbols.Add(new Symbol { Character = ch, Code = code });
        //        }

        //        int encodedBitCount = reader.ReadInt32();
        //        int encodedByteCount = reader.ReadInt32();
        //        byte[] encodedBytes = reader.ReadBytes(encodedByteCount);

        //        string encodedBits = UnpackBits(encodedBytes, encodedBitCount);
        //        string decodedText = DecodeText(encodedBits, symbols);

        //        File.WriteAllText(outputPath, decodedText);
        //        Debug.WriteLine($"Decoded file saved to: {outputPath}");
        //    }
        //}


    }
}
