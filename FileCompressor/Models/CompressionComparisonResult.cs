namespace FileCompressorApp.Helpers
{
    public class CompressionComparisonResult
    {
        public string FileName { get; set; }
        public int OriginalSize { get; set; }
        public int HuffmanSize { get; set; }
        public long HuffmanTimeMs { get; set; }
        public int ShannonSize { get; set; }
        public long ShannonTimeMs { get; set; }
        public double HuffmanRatio => 100.0 * (OriginalSize - HuffmanSize) / OriginalSize;
        public double ShannonRatio => 100.0 * (OriginalSize - ShannonSize) / OriginalSize;
    }
}
