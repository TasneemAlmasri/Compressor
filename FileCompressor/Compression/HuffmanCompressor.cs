namespace FileCompressorApp.Compression
{
    using FileCompressorApp.Models;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public static class HuffmanCompressor
    {
        public static List<HuffmanNode> BuildFrequencyTable(byte[] data) 
        {
            Dictionary<byte, int> freqMap = new Dictionary<byte, int>(); 
            foreach (byte c in data)
            {
                if (freqMap.ContainsKey(c))
                    freqMap[c]++;
                else
                    freqMap[c] = 1;
            }

            return freqMap.Select(kv => new HuffmanNode
            {
                Character = kv.Key,
                Frequency = kv.Value
            }).OrderBy(node => node.Frequency).ToList();
        }

        public static HuffmanNode BuildTree(List<HuffmanNode> nodes)
        {
            var pq = new PriorityQueue<HuffmanNode, int>();

            foreach (var node in nodes)
            {
                pq.Enqueue(node, node.Frequency);
            }

            while (pq.Count > 1)
            {
                HuffmanNode left = pq.Dequeue();
                HuffmanNode right = pq.Dequeue();

                HuffmanNode parent = new HuffmanNode
                {
                    Character = null,
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                pq.Enqueue(parent, parent.Frequency);
            }
            return pq.Dequeue();
        }

        public static Dictionary<byte, string> BuildCodeTable(HuffmanNode root)
        {
            Dictionary<byte, string> table = new Dictionary<byte, string>();
            void Traverse(HuffmanNode node, string code)
            {
                if (node == null)
                    return;

                if (node.IsLeaf)
                {
                    table[node.Character.Value] = code;
                    Debug.WriteLine($"Char: '{node.Character}' => Code: {code}"); 
                }
                else
                {
                    Traverse(node.Left, code + "0");
                    Traverse(node.Right, code + "1");
                }
            }

            Traverse(root, "");
            return table;
        }

        public static string Encode(byte[] text, Dictionary<byte, string> codeTable, CancellationToken token, ManualResetEventSlim pauseEvent, IProgress<int> progress = null)
        {
            StringBuilder encoded = new StringBuilder();
            int totalChars = text.Length;
            int reportInterval = Math.Max(totalChars / 100, 1);

            for (int i = 0; i < totalChars; i++)
            {
                token.ThrowIfCancellationRequested();
                pauseEvent.Wait();

                //char c = text[i];
                byte c = text[i]; 

                if (codeTable.ContainsKey(c))
                    encoded.Append(codeTable[c]);
                else
                    throw new Exception($"Character '{c}' not found in code table.");

                if (progress != null && (i % reportInterval == 0 || i == totalChars - 1))
                {
                    int percent = (int)((i + 1) * 100 / totalChars);
                    progress.Report(percent);
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }

            return encoded.ToString();
        }
        public static string Decode(string encodedBits, HuffmanNode root, CancellationToken token, ManualResetEventSlim pauseEvent, IProgress<int> progress = null)
        {
            StringBuilder decoded = new StringBuilder();
            HuffmanNode current = root;
            int totalBits = encodedBits.Length;
            int reportInterval = Math.Max(totalBits / 100, 1);

            for (int i = 0; i < totalBits; i++)
            {
                token.ThrowIfCancellationRequested();
                pauseEvent.Wait();

                current = encodedBits[i] == '0' ? current.Left : current.Right;

                if (current.IsLeaf)
                {
                    decoded.Append(current.Character);
                    current = root;
                }

                if (progress != null && (i % reportInterval == 0 || i == totalBits - 1))
                {
                    int percent = (int)((i + 1L) * 100 / totalBits);
                    progress.Report(percent);
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }

            return decoded.ToString();
        }

    }
}
