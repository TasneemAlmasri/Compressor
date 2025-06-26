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
        public static List<HuffmanNode> BuildFrequencyTable(string text)
        {
            //هون عرفنا ماب مشان نخزن كل حرف كم مرة تكرر 
            Dictionary<char, int> freqMap = new Dictionary<char, int>();
            foreach (char c in text)
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

        public static Dictionary<char, string> BuildCodeTable(HuffmanNode root)
        {
            Dictionary<char, string> table = new Dictionary<char, string>();
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

        public static string Encode(string text, Dictionary<char, string> codeTable)
        {
            StringBuilder encoded = new StringBuilder();
            foreach (char c in text)
            {
                if (codeTable.ContainsKey(c))
                    encoded.Append(codeTable[c]);
                else
                    throw new Exception($"Character '{c}' not found in code table.");
            }
            return encoded.ToString();
        }

        //public static string Decode(string encodedBits, HuffmanNode root)
        //{
        //    StringBuilder decoded = new StringBuilder();
        //    HuffmanNode current = root;

        //    foreach (char bit in encodedBits)
        //    {
        //        if (bit == '0')
        //            current = current.Left;
        //        else if (bit == '1')
        //            current = current.Right;
        //        else
        //            throw new Exception("Invalid bit in encoded string.");

        //        if (current.IsLeaf)
        //        {
        //            decoded.Append(current.Character);
        //            current = root; // نرجع للجذر بعد كل حرف
        //        }
        //    }

        //    return decoded.ToString();
        //}
        public static string Decode(string encodedBits, HuffmanNode root, IProgress<int> progress = null)
        {
            StringBuilder decoded = new StringBuilder();
            HuffmanNode current = root;
            int totalBits = encodedBits.Length;
            int reportInterval = Math.Max(totalBits / 100, 1);

            for (int i = 0; i < totalBits; i++)
            {
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
