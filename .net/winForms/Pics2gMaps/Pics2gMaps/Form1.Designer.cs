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
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Dock = DockStyle.Bottom;
            btnStart.Location = new Point(0, 427);
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
            tbSaveToPath.Text = "html\\blog\\www";
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
            tbLog.Location = new Point(0, 52);
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.Size = new Size(800, 375);
            tbLog.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tbLog);
            Controls.Add(panel4);
            Controls.Add(panel1);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
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
    }
}
