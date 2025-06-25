using System.Text;

namespace FileCompressorApp.Helpers
{
    public static class BitHelper
    {
        public static byte[] PackBits(string bits)
        {
            int byteCount = (bits.Length + 7) / 8;
            byte[] bytes = new byte[byteCount];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    int byteIndex = i / 8;
                    int bitIndex = 7 - (i % 8);
                    bytes[byteIndex] |= (byte)(1 << bitIndex);
                }
            }

            return bytes;
        }

        public static string UnpackBits(byte[] bytes, int bitCount)
        {
            var bits = new StringBuilder();

            for (int i = 0; i < bitCount; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                bool bit = (bytes[byteIndex] & (1 << bitIndex)) != 0;
                bits.Append(bit ? '1' : '0');
            }

            return bits.ToString();
        }
    }
}