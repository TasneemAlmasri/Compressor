using System;

namespace FileCompressorApp.Models
{
    public class HuffmanNode
    {
        //public char? Character { get; set; } 
        public byte? Character { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;
    }
}
