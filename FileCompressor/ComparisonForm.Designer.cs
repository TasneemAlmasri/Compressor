namespace FileCompressor
{
    partial class ComparisonForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private System.Windows.Forms.ListView listViewComparison;
        private System.Windows.Forms.ColumnHeader columnFileName;
        private System.Windows.Forms.ColumnHeader columnOriginalSize;
        private System.Windows.Forms.ColumnHeader columnShannonSize;
        private System.Windows.Forms.ColumnHeader columnShannonTime;
        private System.Windows.Forms.ColumnHeader columnHuffmanSize;
        private System.Windows.Forms.ColumnHeader columnHuffmanTime;
        private System.Windows.Forms.Button btnClose;

        private void InitializeComponent()
        {
            listViewComparison = new ListView();
            columnFileName = new ColumnHeader();
            columnOriginalSize = new ColumnHeader();
            columnShannonSize = new ColumnHeader();
            columnShannonTime = new ColumnHeader();
            columnHuffmanSize = new ColumnHeader();
            columnHuffmanTime = new ColumnHeader();
            btnClose = new Button();
            SuspendLayout();
            // 
            // listViewComparison
            // 
            listViewComparison.Columns.AddRange(new ColumnHeader[] { columnFileName, columnOriginalSize, columnShannonSize, columnShannonTime, columnHuffmanSize, columnHuffmanTime });
            listViewComparison.FullRowSelect = true;
            listViewComparison.GridLines = true;
            listViewComparison.Location = new Point(12, 15);
            listViewComparison.Margin = new Padding(3, 4, 3, 4);
            listViewComparison.Name = "listViewComparison";
            listViewComparison.Size = new Size(760, 149);
            listViewComparison.TabIndex = 0;
            listViewComparison.UseCompatibleStateImageBehavior = false;
            listViewComparison.View = View.Details;
            // 
            // columnFileName
            // 
            columnFileName.Text = "File Name";
            columnFileName.Width = 150;
            // 
            // columnOriginalSize
            // 
            columnOriginalSize.Text = "Original Size (B)";
            columnOriginalSize.Width = 100;
            // 
            // columnShannonSize
            // 
            columnShannonSize.Text = "Shannon Size (B)";
            columnShannonSize.Width = 120;
            // 
            // columnShannonTime
            // 
            columnShannonTime.Text = "Shannon Time (ms)";
            columnShannonTime.Width = 130;
            // 
            // columnHuffmanSize
            // 
            columnHuffmanSize.Text = "Huffman Size (B)";
            columnHuffmanSize.Width = 120;
            // 
            // columnHuffmanTime
            // 
            columnHuffmanTime.Text = "Huffman Time (ms)";
            columnHuffmanTime.Width = 130;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(680, 172);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(92, 38);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // ComparisonForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 215);
            Controls.Add(btnClose);
            Controls.Add(listViewComparison);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ComparisonForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Compression Comparison";
            ResumeLayout(false);
        }

        #endregion
    }
}
