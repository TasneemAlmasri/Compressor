namespace FileCompressorApp.Models
{
    public class Symbol
    {
        //public char Character { get; set; }
        public byte Character { get; set; } // ➕
        public int Frequency { get; set; }
        public double Probability { get; set; } // مثلاً: 0.12
        public string Code { get; set; } = "";
        public int BitCount => Code?.Length ?? 0;
        public int SizeInBits => Frequency * BitCount;
    }
}
