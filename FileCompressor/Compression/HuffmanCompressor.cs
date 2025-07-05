namespace FileCompressorApp.Compression
{
    using FileCompressorApp.Models;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public static class HuffmanCompressor
    {
        public static List<HuffmanNode> BuildFrequencyTable(byte[] data) // ➕
        {
            //هون عرفنا ماب مشان نخزن كل حرف كم مرة تكرر 
            Dictionary<byte, int> freqMap = new Dictionary<byte, int>(); // ➕
            foreach (byte c in data) // ➕
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
            // ننشئ PriorityQueue بترتيب تصاعدي حسب Frequency
            var pq = new PriorityQueue<HuffmanNode, int>();

            // نضيف كل عقدة إلى الـ PriorityQueue مع تكرارها كأولوية
            foreach (var node in nodes)
            {
                pq.Enqueue(node, node.Frequency);
            }

            // نكرر حتى يتبقى عقدة واحدة فقط
            while (pq.Count > 1)
            {
                // نأخذ العقدتين الأقل تكرارًا
                HuffmanNode left = pq.Dequeue();
                HuffmanNode right = pq.Dequeue();

                // ننشئ عقدة جديدة لا تحتوي على حرف (null)
                HuffmanNode parent = new HuffmanNode
                {
                    Character = null,
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                // نضيف العقدة الجديدة للـ PriorityQueue
                pq.Enqueue(parent, parent.Frequency);
            }

            // العقدة الوحيدة المتبقية هي جذر الشجرة
            return pq.Dequeue();
        }

        public static Dictionary<byte, string> BuildCodeTable(HuffmanNode root)// ➕
        {
            Dictionary<byte, string> table = new Dictionary<byte, string>();// ➕
            //We are using dfs and pre oreder for traversing
            void Traverse(HuffmanNode node, string code)
            {
                if (node == null)
                    return;

                if (node.IsLeaf)
                {
                    table[node.Character.Value] = code;
                    Debug.WriteLine($"Char: '{node.Character}' => Code: {code}");  // للطباعة المؤقتة والتأكد
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
        public static string Encode(byte[] text, Dictionary<byte, string> codeTable, CancellationToken token, ManualResetEventSlim pauseEvent, IProgress<int> progress = null)// ➕
        {
            StringBuilder encoded = new StringBuilder();
            int totalChars = text.Length;
            int reportInterval = Math.Max(totalChars / 100, 1);

            for (int i = 0; i < totalChars; i++)
            {
                token.ThrowIfCancellationRequested();
                pauseEvent.Wait();

                //char c = text[i];
                byte c = text[i]; // ➕

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
