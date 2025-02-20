namespace Pics2gMaps
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
            btnStart = new Button();
            panel1 = new Panel();
            panel3 = new Panel();
            tbTemplateRootFolder = new TextBox();
            panel2 = new Panel();
            label1 = new Label();
            panel4 = new Panel();
            panel5 = new Panel();
            tbSaveToPath = new TextBox();
            panel6 = new Panel();
            label2 = new Label();
            tbLog = new TextBox();
            panel7 = new Panel();
            panel8 = new Panel();
            tbJsonFile = new TextBox();
            panel9 = new Panel();
            label3 = new Label();
            btnLoadOld = new Button();
            splitter1 = new Splitter();
            dgvGalleryConfiguration = new DataGridView();
            btnSaveConfig = new Button();
            btnLoadNew = new Button();
            statusStrip1 = new StatusStrip();
            tsslRecordCount = new ToolStripStatusLabel();
            tsslElapsedTime = new ToolStripStatusLabel();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            panel8.SuspendLayout();
            panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGalleryConfiguration).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Dock = DockStyle.Bottom;
            btnStart.Location = new Point(0, 405);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(800, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 26);
            panel1.TabIndex = 1;
            // 
            // panel3
            // 
            panel3.Controls.Add(tbTemplateRootFolder);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(124, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(676, 26);
            panel3.TabIndex = 1;
            // 
            // tbTemplateRootFolder
            // 
            tbTemplateRootFolder.Dock = DockStyle.Fill;
            tbTemplateRootFolder.Location = new Point(0, 0);
            tbTemplateRootFolder.Name = "tbTemplateRootFolder";
            tbTemplateRootFolder.Size = new Size(676, 23);
            tbTemplateRootFolder.TabIndex = 0;
            tbTemplateRootFolder.Text = "..\\..\\..\\..\\..\\..\\..\\html\\templateForBlog";
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(124, 26);
            panel2.TabIndex = 0;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(124, 26);
            label1.TabIndex = 0;
            label1.Text = "Template root folder:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            panel4.Controls.Add(panel5);
            panel4.Controls.Add(panel6);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 26);
            panel4.Name = "panel4";
            panel4.Size = new Size(800, 26);
            panel4.TabIndex = 2;
            // 
            // panel5
            // 
            panel5.Controls.Add(tbSaveToPath);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(124, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(676, 26);
            panel5.TabIndex = 1;
            // 
            // tbSaveToPath
            // 
            tbSaveToPath.Dock = DockStyle.Fill;
            tbSaveToPath.Location = new Point(0, 0);
            tbSaveToPath.Name = "tbSaveToPath";
            tbSaveToPath.Size = new Size(676, 23);
            tbSaveToPath.TabIndex = 0;
            tbSaveToPath.Text = "html\\blog";
            // 
            // panel6
            // 
            panel6.Controls.Add(label2);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(124, 26);
            panel6.TabIndex = 0;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(124, 26);
            label2.TabIndex = 0;
            label2.Text = "Save to:";
            // 
            // tbLog
            // 
            tbLog.Dock = DockStyle.Fill;
            tbLog.Location = new Point(0, 238);
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ScrollBars = ScrollBars.Both;
            tbLog.Size = new Size(800, 98);
            tbLog.TabIndex = 3;
            // 
            // panel7
            // 
            panel7.Controls.Add(panel8);
            panel7.Controls.Add(panel9);
            panel7.Dock = DockStyle.Top;
            panel7.Location = new Point(0, 52);
            panel7.Name = "panel7";
            panel7.Size = new Size(800, 26);
            panel7.TabIndex = 3;
            // 
            // panel8
            // 
            panel8.Controls.Add(tbJsonFile);
            panel8.Dock = DockStyle.Fill;
            panel8.Location = new Point(124, 0);
            panel8.Name = "panel8";
            panel8.Size = new Size(676, 26);
            panel8.TabIndex = 1;
            // 
            // tbJsonFile
            // 
            tbJsonFile.Dock = DockStyle.Fill;
            tbJsonFile.Location = new Point(0, 0);
            tbJsonFile.Name = "tbJsonFile";
            tbJsonFile.Size = new Size(676, 23);
            tbJsonFile.TabIndex = 0;
            tbJsonFile.Text = "listOfKeyValuesToReplaceInFilesAry.json";
            tbJsonFile.Leave += tbJsonFile_Leave;
            // 
            // panel9
            // 
            panel9.Controls.Add(label3);
            panel9.Dock = DockStyle.Left;
            panel9.Location = new Point(0, 0);
            panel9.Name = "panel9";
            panel9.Size = new Size(124, 26);
            panel9.TabIndex = 0;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(124, 26);
            label3.TabIndex = 0;
            label3.Text = "Json file with keys:";
            // 
            // btnLoadOld
            // 
            btnLoadOld.Dock = DockStyle.Bottom;
            btnLoadOld.Location = new Point(0, 382);
            btnLoadOld.Name = "btnLoadOld";
            btnLoadOld.Size = new Size(800, 23);
            btnLoadOld.TabIndex = 4;
            btnLoadOld.Text = "Load old configuration";
            btnLoadOld.UseVisualStyleBackColor = true;
            btnLoadOld.Click += btnLoadOld_Click;
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Top;
            splitter1.Location = new Point(0, 228);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(800, 10);
            splitter1.TabIndex = 6;
            splitter1.TabStop = false;
            // 
            // dgvGalleryConfiguration
            // 
            dgvGalleryConfiguration.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGalleryConfiguration.Dock = DockStyle.Top;
            dgvGalleryConfiguration.Location = new Point(0, 78);
            dgvGalleryConfiguration.Name = "dgvGalleryConfiguration";
            dgvGalleryConfiguration.Size = new Size(800, 150);
            dgvGalleryConfiguration.TabIndex = 7;
            // 
            // btnSaveConfig
            // 
            btnSaveConfig.Dock = DockStyle.Bottom;
            btnSaveConfig.Location = new Point(0, 359);
            btnSaveConfig.Name = "btnSaveConfig";
            btnSaveConfig.Size = new Size(800, 23);
            btnSaveConfig.TabIndex = 8;
            btnSaveConfig.Text = "Save configuration";
            btnSaveConfig.UseVisualStyleBackColor = true;
            btnSaveConfig.Click += btnSaveConfig_Click;
            // 
            // btnLoadNew
            // 
            btnLoadNew.Dock = DockStyle.Bottom;
            btnLoadNew.Location = new Point(0, 336);
            btnLoadNew.Name = "btnLoadNew";
            btnLoadNew.Size = new Size(800, 23);
            btnLoadNew.TabIndex = 9;
            btnLoadNew.Text = "Load new configuration";
            btnLoadNew.UseVisualStyleBackColor = true;
            btnLoadNew.Click += btnLoadNew_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslRecordCount, tsslElapsedTime });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsslRecordCount
            // 
            tsslRecordCount.Name = "tsslRecordCount";
            tsslRecordCount.Size = new Size(98, 17);
            tsslRecordCount.Text = "Files processed: 0";
            // 
            // tsslElapsedTime
            // 
            tsslElapsedTime.Name = "tsslElapsedTime";
            tsslElapsedTime.Size = new Size(122, 17);
            tsslElapsedTime.Text = "Elapsed time: 00:00:00";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tbLog);
            Controls.Add(btnLoadNew);
            Controls.Add(btnSaveConfig);
            Controls.Add(splitter1);
            Controls.Add(dgvGalleryConfiguration);
            Controls.Add(btnLoadOld);
            Controls.Add(panel7);
            Controls.Add(panel4);
            Controls.Add(panel1);
            Controls.Add(btnStart);
            Controls.Add(statusStrip1);
            Name = "Form1";
            Text = "Main";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            panel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGalleryConfiguration).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Panel panel1;
        private Panel panel3;
        private TextBox tbTemplateRootFolder;
        private Panel panel2;
        private Label label1;
        private Panel panel4;
        private Panel panel5;
        private TextBox tbSaveToPath;
        private Panel panel6;
        private Label label2;
        private TextBox tbLog;
        private Panel panel7;
        private Panel panel8;
        private TextBox tbJsonFile;
        private Panel panel9;
        private Label label3;
        private Button btnLoadOld;
        private Splitter splitter1;
        private DataGridView dgvGalleryConfiguration;
        private Button btnSaveConfig;
        private Button btnLoadNew;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tsslRecordCount;
        private ToolStripStatusLabel tsslElapsedTime;
    }
}
