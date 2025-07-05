using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileCompressorApp.Helpers;

namespace FileCompressor
{
    public partial class ComparisonForm : Form
    {
        public ComparisonForm()
        {
            InitializeComponent();
            SetupListView();
        }

        private void SetupListView()
        {
            listViewComparison.View = View.Details;
            listViewComparison.FullRowSelect = true;
            listViewComparison.GridLines = true;
            listViewComparison.MultiSelect = false;

            listViewComparison.Columns.Clear();
            listViewComparison.Columns.Add("File Name", 150, HorizontalAlignment.Left);
            listViewComparison.Columns.Add("Original Size (B)", 100, HorizontalAlignment.Right);
            listViewComparison.Columns.Add("Shannon Size (B)", 120, HorizontalAlignment.Right);
            listViewComparison.Columns.Add("Shannon Time (ms)", 130, HorizontalAlignment.Right);
            listViewComparison.Columns.Add("Huffman Size (B)", 120, HorizontalAlignment.Right);
            listViewComparison.Columns.Add("Huffman Time (ms)", 130, HorizontalAlignment.Right);
        }

        public void LoadResults(List<CompressionComparisonResult> results)
        {
            listViewComparison.Items.Clear();

            foreach (var res in results)
            {
                var item = new ListViewItem(res.FileName);
                item.SubItems.Add(res.OriginalSize.ToString());
                item.SubItems.Add(res.ShannonSize.ToString());
                item.SubItems.Add(res.ShannonTimeMs.ToString());
                item.SubItems.Add(res.HuffmanSize.ToString());
                item.SubItems.Add(res.HuffmanTimeMs.ToString());

                listViewComparison.Items.Add(item);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
