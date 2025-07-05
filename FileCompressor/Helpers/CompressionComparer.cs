using FileCompressorApp.Compression;
using FileCompressorApp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FileCompressorApp.Helpers
{
    public static class CompressionComparer
    {
        public static List<CompressionComparisonResult> CompareCompressionAlgorithms(List<string> inputFilePaths)
        {
            List<CompressionComparisonResult> results = new List<CompressionComparisonResult>();

            foreach (var path in inputFilePaths)
            {
                byte[] data = File.ReadAllBytes(path);
                int originalSize = data.Length;
                string fileName = Path.GetFileName(path);

                var huffmanResult = CompressAndMeasure(data, "huffman");
                var shannonResult = CompressAndMeasure(data, "shannon");

                results.Add(new CompressionComparisonResult
                {
                    FileName = fileName,
                    OriginalSize = originalSize,
                    HuffmanSize = huffmanResult.packedSize,
                    HuffmanTimeMs = huffmanResult.timeMs,
                    ShannonSize = shannonResult.packedSize,
                    ShannonTimeMs = shannonResult.timeMs,
                });
            }

            return results;
        }

        private static (int packedSize, long timeMs) CompressAndMeasure(byte[] data, string algorithm)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string bitString = "";

            if (algorithm == "huffman")
            {
                var freqTable = HuffmanCompressor.BuildFrequencyTable(data);
                var root = HuffmanCompressor.BuildTree(freqTable);
                var codeTable = HuffmanCompressor.BuildCodeTable(root);
                bitString = HuffmanCompressor.Encode(data, codeTable, CancellationToken.None, new ManualResetEventSlim(true));
            }
            else if (algorithm == "shannon")
            {
                var symbols = ShannonFanoCompressor.BuildFrequencyTable(data);
                ShannonFanoCompressor.BuildCodes(symbols);
                bitString = ShannonFanoCompressor.Encode(data, symbols, CancellationToken.None, new ManualResetEventSlim(true));
            }
            else
            {
                throw new InvalidOperationException("Unknown algorithm: " + algorithm);
            }

            byte[] packedBytes = BitHelper.PackBits(bitString);
            sw.Stop();
            return (packedBytes.Length, sw.ElapsedMilliseconds);
        }
    }
}
