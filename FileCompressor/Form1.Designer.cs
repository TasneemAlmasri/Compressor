namespace FileCompressor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtFilePath = new TextBox();
            btnBrowse = new Button();
            groupBox1 = new GroupBox();
            btnBrowseFolders = new Button();
            groupBox2 = new GroupBox();
            radioHuffman = new RadioButton();
            radioShannon = new RadioButton();
            btnSetPassword = new Button();
            btnStart = new Button();
            btnCancel = new Button();
            groupBox3 = new GroupBox();
            btnBrowseDistance = new Button();
            txtOutputPath = new TextBox();
            tabCompress = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            lvArchiveContents = new ListView();
            colFileName = new ColumnHeader();
            colOriginalSize = new ColumnHeader();
            colCompressedSize = new ColumnHeader();
            colAlgorithm = new ColumnHeader();
            groupBox4 = new GroupBox();
            debtnBrowse = new Button();
            detxtFilePath = new TextBox();
            btnCancel2 = new Button();
            groupBox6 = new GroupBox();
            debtnBrowseDistance = new Button();
            detxtOutputPath = new TextBox();
            btnDecompress_Click = new Button();
            compareButton = new RadioButton();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            tabCompress.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.BackColor = SystemColors.ControlLightLight;
            txtFilePath.Location = new Point(12, 26);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(312, 27);
            txtFilePath.TabIndex = 0;
            txtFilePath.TextChanged += txtFilePath_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(330, 26);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(76, 29);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Files";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnBrowseFolders);
            groupBox1.Controls.Add(btnBrowse);
            groupBox1.Controls.Add(txtFilePath);
            groupBox1.Location = new Point(10, 16);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(495, 77);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Source";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // btnBrowseFolders
            // 
            btnBrowseFolders.Location = new Point(412, 26);
            btnBrowseFolders.Name = "btnBrowseFolders";
            btnBrowseFolders.Size = new Size(76, 29);
            btnBrowseFolders.TabIndex = 2;
            btnBrowseFolders.Text = "Folder";
            btnBrowseFolders.UseVisualStyleBackColor = true;
            btnBrowseFolders.Click += btnBrowseFolders_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(compareButton);
            groupBox2.Controls.Add(radioHuffman);
            groupBox2.Controls.Add(radioShannon);
            groupBox2.Controls.Add(btnSetPassword);
            groupBox2.Location = new Point(10, 182);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(495, 120);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Algorithm";
            // 
            // radioHuffman
            // 
            radioHuffman.AutoSize = true;
            radioHuffman.Location = new Point(12, 56);
            radioHuffman.Name = "radioHuffman";
            radioHuffman.Size = new Size(88, 24);
            radioHuffman.TabIndex = 1;
            radioHuffman.Text = "Huffman";
            radioHuffman.UseVisualStyleBackColor = true;
            radioHuffman.CheckedChanged += radioHuffman_CheckedChanged;
            // 
            // radioShannon
            // 
            radioShannon.AutoSize = true;
            radioShannon.Checked = true;
            radioShannon.Location = new Point(12, 26);
            radioShannon.Name = "radioShannon";
            radioShannon.Size = new Size(124, 24);
            radioShannon.TabIndex = 0;
            radioShannon.TabStop = true;
            radioShannon.Text = "Shannon-Fano";
            radioShannon.UseVisualStyleBackColor = true;
            radioShannon.CheckedChanged += radioShannon_CheckedChanged;
            // 
            // btnSetPassword
            // 
            btnSetPassword.Location = new Point(377, 80);
            btnSetPassword.Name = "btnSetPassword";
            btnSetPassword.Size = new Size(112, 30);
            btnSetPassword.TabIndex = 2;
            btnSetPassword.Text = "Set Password";
            btnSetPassword.UseVisualStyleBackColor = true;
            btnSetPassword.Click += btnSetPassword_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(10, 308);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(221, 29);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(256, 308);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(249, 29);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnBrowseDistance);
            groupBox3.Controls.Add(txtOutputPath);
            groupBox3.Location = new Point(10, 99);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(495, 77);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Distance";
            groupBox3.Enter += groupBox3_Enter;
            // 
            // btnBrowseDistance
            // 
            btnBrowseDistance.Location = new Point(386, 26);
            btnBrowseDistance.Name = "btnBrowseDistance";
            btnBrowseDistance.Size = new Size(103, 29);
            btnBrowseDistance.TabIndex = 1;
            btnBrowseDistance.Text = "Browse";
            btnBrowseDistance.UseVisualStyleBackColor = true;
            btnBrowseDistance.Click += btnBrowseDistance_Click;
            // 
            // txtOutputPath
            // 
            txtOutputPath.BackColor = SystemColors.ControlLightLight;
            txtOutputPath.Location = new Point(12, 26);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new Size(368, 27);
            txtOutputPath.TabIndex = 0;
            txtOutputPath.TextChanged += txtOutputPath_TextChanged;
            // 
            // tabCompress
            // 
            tabCompress.Controls.Add(tabPage1);
            tabCompress.Controls.Add(tabPage2);
            tabCompress.Location = new Point(-2, -2);
            tabCompress.Name = "tabCompress";
            tabCompress.SelectedIndex = 0;
            tabCompress.Size = new Size(522, 381);
            tabCompress.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(btnCancel);
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Controls.Add(btnStart);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(514, 348);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Compress";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lvArchiveContents);
            tabPage2.Controls.Add(groupBox4);
            tabPage2.Controls.Add(btnCancel2);
            tabPage2.Controls.Add(groupBox6);
            tabPage2.Controls.Add(btnDecompress_Click);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(514, 334);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Decompress";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvArchiveContents
            // 
            lvArchiveContents.CheckBoxes = true;
            lvArchiveContents.Columns.AddRange(new ColumnHeader[] { colFileName, colOriginalSize, colCompressedSize, colAlgorithm });
            lvArchiveContents.FullRowSelect = true;
            lvArchiveContents.Location = new Point(10, 96);
            lvArchiveContents.Name = "lvArchiveContents";
            lvArchiveContents.Size = new Size(495, 107);
            lvArchiveContents.TabIndex = 10;
            lvArchiveContents.UseCompatibleStateImageBehavior = false;
            lvArchiveContents.View = View.Details;
            lvArchiveContents.ItemChecked += listViewArchiveFiles_ItemChecked;
            lvArchiveContents.SelectedIndexChanged += lvArchiveContents_SelectedIndexChanged;
            // 
            // colFileName
            // 
            colFileName.Text = "File Name";
            colFileName.Width = 180;
            // 
            // colOriginalSize
            // 
            colOriginalSize.Text = "Algorithm";
            colOriginalSize.Width = 100;
            // 
            // colCompressedSize
            // 
            colCompressedSize.Text = "Original Size (B)";
            colCompressedSize.Width = 120;
            // 
            // colAlgorithm
            // 
            colAlgorithm.Text = "Compressed Size (B)";
            colAlgorithm.Width = 100;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(debtnBrowse);
            groupBox4.Controls.Add(detxtFilePath);
            groupBox4.Location = new Point(10, 13);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(495, 77);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Source";
            // 
            // debtnBrowse
            // 
            debtnBrowse.Location = new Point(386, 26);
            debtnBrowse.Name = "debtnBrowse";
            debtnBrowse.Size = new Size(103, 29);
            debtnBrowse.TabIndex = 1;
            debtnBrowse.Text = "Browse";
            debtnBrowse.UseVisualStyleBackColor = true;
            debtnBrowse.Click += debtnBrowse_Click;
            // 
            // detxtFilePath
            // 
            detxtFilePath.BackColor = SystemColors.ControlLightLight;
            detxtFilePath.Location = new Point(12, 26);
            detxtFilePath.Name = "detxtFilePath";
            detxtFilePath.Size = new Size(368, 27);
            detxtFilePath.TabIndex = 0;
            // 
            // btnCancel2
            // 
            btnCancel2.Location = new Point(256, 292);
            btnCancel2.Name = "btnCancel2";
            btnCancel2.Size = new Size(249, 29);
            btnCancel2.TabIndex = 9;
            btnCancel2.Text = "Cancel";
            btnCancel2.UseVisualStyleBackColor = true;
            btnCancel2.Click += btnCancel2_Click;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(debtnBrowseDistance);
            groupBox6.Controls.Add(detxtOutputPath);
            groupBox6.Location = new Point(10, 209);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(495, 77);
            groupBox6.TabIndex = 7;
            groupBox6.TabStop = false;
            groupBox6.Text = "Distance";
            // 
            // debtnBrowseDistance
            // 
            debtnBrowseDistance.Location = new Point(386, 26);
            debtnBrowseDistance.Name = "debtnBrowseDistance";
            debtnBrowseDistance.Size = new Size(103, 29);
            debtnBrowseDistance.TabIndex = 1;
            debtnBrowseDistance.Text = "Browse";
            debtnBrowseDistance.UseVisualStyleBackColor = true;
            debtnBrowseDistance.Click += debtnBrowseDistance_Click;
            // 
            // detxtOutputPath
            // 
            detxtOutputPath.BackColor = SystemColors.ControlLightLight;
            detxtOutputPath.Location = new Point(12, 26);
            detxtOutputPath.Name = "detxtOutputPath";
            detxtOutputPath.Size = new Size(368, 27);
            detxtOutputPath.TabIndex = 0;
            // 
            // btnDecompress_Click
            // 
            btnDecompress_Click.Location = new Point(10, 292);
            btnDecompress_Click.Name = "btnDecompress_Click";
            btnDecompress_Click.Size = new Size(221, 29);
            btnDecompress_Click.TabIndex = 8;
            btnDecompress_Click.Text = "Start";
            btnDecompress_Click.UseVisualStyleBackColor = true;
            btnDecompress_Click.Click += btnDecompress_Click_Click;
            // 
            // compareButton
            // 
            compareButton.AutoSize = true;
            compareButton.Location = new Point(12, 86);
            compareButton.Name = "compareButton";
            compareButton.Size = new Size(91, 24);
            compareButton.TabIndex = 3;
            compareButton.Text = "Compare";
            compareButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(519, 376);
            Controls.Add(tabCompress);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tabCompress.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        //private TextBox txtFilePath;
        //private Button btnBrowse;
        //private GroupBox groupBox1;
        //private GroupBox groupBox2;
        //private RadioButton radioHuffman;
        //private RadioButton radioShannon;
        //private Button btnSetPassword;
        //private Button btnStart;
        //private Button btnCancel;
        //private GroupBox groupBox3;
        //private Button btnBrowseDistance;
        //private TextBox txtOutputPath;
        private TextBox txtFilePath;
        private Button btnBrowse;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private RadioButton radioHuffman;
        private RadioButton radioShannon;
        private Button btnSetPassword;
        private Button btnStart;
        private Button btnCancel;
        private GroupBox groupBox3;
        private Button btnBrowseDistance;
        private TextBox txtOutputPath;
        private TabControl tabCompress;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox4;
        private Button debtnBrowse;
        private TextBox detxtFilePath;
        private Button btnCancel2;
        private GroupBox groupBox5;
        private RadioButton compareButton;
        private RadioButton radioButton2;
        private Button button3;
        private GroupBox groupBox6;
        private Button debtnBrowseDistance;
        private TextBox detxtOutputPath;
        private Button btnDecompress_Click;
        private ListView lvArchiveContents;
        private ColumnHeader colFileName;
        private ColumnHeader colOriginalSize;
        private ColumnHeader colCompressedSize;
        private ColumnHeader colAlgorithm;
        private Button button1;
        private ListBox listArchiveContents;
        private Button btnBrowseFolders;
    }
}
