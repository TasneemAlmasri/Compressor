using FileCompressorApp.Models;
using System.Text;

namespace FileCompressorApp.Compression
{
    public static class ShannonFanoCompressor
    {
        public static List<Symbol> BuildFrequencyTable(byte[] text) 
        {
            Dictionary<byte, int> freqMap = new Dictionary<byte, int>();
            foreach (byte c in text)
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

        public static void BuildCodes(List<Symbol> symbols)
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
        public static string Encode(byte[] text, List<Symbol> symbols, CancellationToken token, ManualResetEventSlim pauseEvent, IProgress<int> progress = null)
        {
            var codeMap = symbols.ToDictionary(s => s.Character, s => s.Code);
            StringBuilder encodedBuilder = new StringBuilder();

            int totalChars = text.Length;
            int reportInterval = Math.Max(totalChars / 100, 1);

            for (int i = 0; i < totalChars; i++)
            {
                token.ThrowIfCancellationRequested();
                pauseEvent.Wait();

                byte c = text[i];


                if (!codeMap.ContainsKey(c))
                    throw new Exception($"Character '{c}' not found in code map.");

                encodedBuilder.Append(codeMap[c]);

                if (progress != null && (i % reportInterval == 0 || i == totalChars - 1))
                {
                    int percent = (int)((i + 1) * 100 / totalChars);
                    progress.Report(percent);
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }

            return encodedBuilder.ToString();
        }

        public static byte[] Decode(string encoded, List<Symbol> symbols, IProgress<int> progress = null, CancellationToken token = default, ManualResetEventSlim pauseEvent = null)
        {
            var reverseMap = symbols.ToDictionary(s => s.Code, s => s.Character);

            List<byte> result = new List<byte>(); 

            string buffer = "";
            int totalBits = encoded.Length;
            int reportInterval = Math.Max(totalBits / 100, 1);

            for (int i = 0; i < totalBits; i++)
            {
                token.ThrowIfCancellationRequested();
                if (pauseEvent != null) pauseEvent.Wait();

                buffer += encoded[i];
                if (reverseMap.ContainsKey(buffer))
                {
                    result.Add(reverseMap[buffer]);

                    buffer = "";
                }

                if (progress != null && (i % reportInterval == 0 || i == totalBits - 1))
                {
                    int percent = (int)((i + 1L) * 100 / totalBits);
                    progress.Report(percent);
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }

            return result.ToArray(); 
        }
    }
}
