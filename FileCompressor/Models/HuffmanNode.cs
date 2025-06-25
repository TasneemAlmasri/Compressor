using System;

namespace FileCompressorApp.Models
{
    public class HuffmanNode
    {
        public char? Character { get; set; }       // null إذا كانت عقدة داخلية
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;
    }
}
