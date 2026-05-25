namespace FunctionalTest
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
            PostGpsPositionsFromFilesWithFileName = new Button();
            panel1 = new Panel();
            log = new TextBox();
            panel2 = new Panel();
            tbJoomlaCategoryId = new TextBox();
            label19 = new Label();
            tbJoomlaUserName = new TextBox();
            label18 = new Label();
            tbJoomlaPostUrl = new TextBox();
            label17 = new Label();
            tbJoomlaLoginUrl = new TextBox();
            label16 = new Label();
            label14 = new Label();
            tbDeleteLastKmlPoints = new TextBox();
            label15 = new Label();
            tbDeleteFirstKmlPoints = new TextBox();
            btnAzureAddress = new Button();
            btnLocalAddress = new Button();
            button1 = new Button();
            label13 = new Label();
            tbPrepareForUploadUrl = new TextBox();
            btnDefaultExpectedUrl = new Button();
            label12 = new Label();
            tbExpectedUrl = new TextBox();
            label11 = new Label();
            tbBaseUrl = new TextBox();
            label10 = new Label();
            tbOgImage = new TextBox();
            label9 = new Label();
            tbOgTitle = new TextBox();
            label8 = new Label();
            tbFtpHost = new TextBox();
            label7 = new Label();
            tbFtpPass = new TextBox();
            label6 = new Label();
            tbFtpUser = new TextBox();
            label5 = new Label();
            imagesPath = new TextBox();
            label4 = new Label();
            tbGpsLocationsPath = new TextBox();
            label3 = new Label();
            address = new TextBox();
            folderName = new TextBox();
            label2 = new Label();
            label1 = new Label();
            kmlFileName = new TextBox();
            UploadImage = new Button();
            UploadToBlog = new Button();
            btnCancel = new Button();
            linkLabel1 = new LinkLabel();
            label20 = new Label();
            tbJoomlaPass = new TextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // PostGpsPositionsFromFilesWithFileName
            // 
            PostGpsPositionsFromFilesWithFileName.Dock = DockStyle.Bottom;
            PostGpsPositionsFromFilesWithFileName.Location = new Point(0, 703);
            PostGpsPositionsFromFilesWithFileName.Name = "PostGpsPositionsFromFilesWithFileName";
            PostGpsPositionsFromFilesWithFileName.Size = new Size(926, 23);
            PostGpsPositionsFromFilesWithFileName.TabIndex = 0;
            PostGpsPositionsFromFilesWithFileName.Text = "PostGpsPositionsFromFilesWithFileName";
            PostGpsPositionsFromFilesWithFileName.UseVisualStyleBackColor = true;
            PostGpsPositionsFromFilesWithFileName.Click += PostGpsPositionsFromFilesWithFileName_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(log);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 592);
            panel1.Name = "panel1";
            panel1.Size = new Size(926, 111);
            panel1.TabIndex = 1;
            // 
            // log
            // 
            log.Dock = DockStyle.Fill;
            log.Location = new Point(0, 0);
            log.Multiline = true;
            log.Name = "log";
            log.ScrollBars = ScrollBars.Both;
            log.Size = new Size(926, 111);
            log.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(label20);
            panel2.Controls.Add(tbJoomlaPass);
            panel2.Controls.Add(tbJoomlaCategoryId);
            panel2.Controls.Add(label19);
            panel2.Controls.Add(tbJoomlaUserName);
            panel2.Controls.Add(label18);
            panel2.Controls.Add(tbJoomlaPostUrl);
            panel2.Controls.Add(label17);
            panel2.Controls.Add(tbJoomlaLoginUrl);
            panel2.Controls.Add(label16);
            panel2.Controls.Add(label14);
            panel2.Controls.Add(tbDeleteLastKmlPoints);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(tbDeleteFirstKmlPoints);
            panel2.Controls.Add(btnAzureAddress);
            panel2.Controls.Add(btnLocalAddress);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(tbPrepareForUploadUrl);
            panel2.Controls.Add(btnDefaultExpectedUrl);
            panel2.Controls.Add(label12);
            panel2.Controls.Add(tbExpectedUrl);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(tbBaseUrl);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(tbOgImage);
            panel2.Controls.Add(label9);
            panel2.Controls.Add(tbOgTitle);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(tbFtpHost);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(tbFtpPass);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(tbFtpUser);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(imagesPath);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(tbGpsLocationsPath);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(address);
            panel2.Controls.Add(folderName);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(kmlFileName);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(926, 592);
            panel2.TabIndex = 2;
            // 
            // tbJoomlaCategoryId
            // 
            tbJoomlaCategoryId.Location = new Point(171, 440);
            tbJoomlaCategoryId.Name = "tbJoomlaCategoryId";
            tbJoomlaCategoryId.Size = new Size(555, 23);
            tbJoomlaCategoryId.TabIndex = 41;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(12, 530);
            label19.Name = "label19";
            label19.Size = new Size(109, 15);
            label19.TabIndex = 40;
            label19.Text = "Joomla! user name:";
            // 
            // tbJoomlaUserName
            // 
            tbJoomlaUserName.Location = new Point(171, 527);
            tbJoomlaUserName.Name = "tbJoomlaUserName";
            tbJoomlaUserName.Size = new Size(555, 23);
            tbJoomlaUserName.TabIndex = 39;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(12, 501);
            label18.Name = "label18";
            label18.Size = new Size(94, 15);
            label18.TabIndex = 38;
            label18.Text = "Joomla! post url:";
            // 
            // tbJoomlaPostUrl
            // 
            tbJoomlaPostUrl.Location = new Point(171, 498);
            tbJoomlaPostUrl.Name = "tbJoomlaPostUrl";
            tbJoomlaPostUrl.Size = new Size(555, 23);
            tbJoomlaPostUrl.TabIndex = 37;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(12, 472);
            label17.Name = "label17";
            label17.Size = new Size(98, 15);
            label17.TabIndex = 36;
            label17.Text = "Joomla! login url:";
            // 
            // tbJoomlaLoginUrl
            // 
            tbJoomlaLoginUrl.Location = new Point(171, 469);
            tbJoomlaLoginUrl.Name = "tbJoomlaLoginUrl";
            tbJoomlaLoginUrl.Size = new Size(555, 23);
            tbJoomlaLoginUrl.TabIndex = 35;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(12, 443);
            label16.Name = "label16";
            label16.Size = new Size(113, 15);
            label16.TabIndex = 34;
            label16.Text = "Joomla! category id:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(12, 414);
            label14.Name = "label14";
            label14.Size = new Size(124, 15);
            label14.TabIndex = 32;
            label14.Text = "Delete last Kml points:";
            // 
            // tbDeleteLastKmlPoints
            // 
            tbDeleteLastKmlPoints.Location = new Point(171, 411);
            tbDeleteLastKmlPoints.Name = "tbDeleteLastKmlPoints";
            tbDeleteLastKmlPoints.Size = new Size(555, 23);
            tbDeleteLastKmlPoints.TabIndex = 30;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(12, 385);
            label15.Name = "label15";
            label15.Size = new Size(126, 15);
            label15.TabIndex = 31;
            label15.Text = "Delete first Kml points:";
            // 
            // tbDeleteFirstKmlPoints
            // 
            tbDeleteFirstKmlPoints.Location = new Point(171, 382);
            tbDeleteFirstKmlPoints.Name = "tbDeleteFirstKmlPoints";
            tbDeleteFirstKmlPoints.Size = new Size(555, 23);
            tbDeleteFirstKmlPoints.TabIndex = 29;
            // 
            // btnAzureAddress
            // 
            btnAzureAddress.Location = new Point(813, 11);
            btnAzureAddress.Name = "btnAzureAddress";
            btnAzureAddress.Size = new Size(75, 23);
            btnAzureAddress.TabIndex = 28;
            btnAzureAddress.Text = "Use Azure";
            btnAzureAddress.UseVisualStyleBackColor = true;
            btnAzureAddress.Click += btnAzureAddress_Click;
            // 
            // btnLocalAddress
            // 
            btnLocalAddress.Location = new Point(732, 11);
            btnLocalAddress.Name = "btnLocalAddress";
            btnLocalAddress.Size = new Size(75, 23);
            btnLocalAddress.TabIndex = 27;
            btnLocalAddress.Text = "Use local";
            btnLocalAddress.UseVisualStyleBackColor = true;
            btnLocalAddress.Click += btnLocalAddress_Click;
            // 
            // button1
            // 
            button1.Location = new Point(732, 352);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 24;
            button1.Text = "Use default";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(12, 356);
            label13.Name = "label13";
            label13.Size = new Size(125, 15);
            label13.TabIndex = 26;
            label13.Text = "Prepare for upload url:";
            // 
            // tbPrepareForUploadUrl
            // 
            tbPrepareForUploadUrl.Location = new Point(171, 353);
            tbPrepareForUploadUrl.Name = "tbPrepareForUploadUrl";
            tbPrepareForUploadUrl.Size = new Size(555, 23);
            tbPrepareForUploadUrl.TabIndex = 25;
            // 
            // btnDefaultExpectedUrl
            // 
            btnDefaultExpectedUrl.Location = new Point(732, 323);
            btnDefaultExpectedUrl.Name = "btnDefaultExpectedUrl";
            btnDefaultExpectedUrl.Size = new Size(75, 23);
            btnDefaultExpectedUrl.TabIndex = 8;
            btnDefaultExpectedUrl.Text = "Use default";
            btnDefaultExpectedUrl.UseVisualStyleBackColor = true;
            btnDefaultExpectedUrl.Click += btnDefaultExpectedUrl_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(12, 327);
            label12.Name = "label12";
            label12.Size = new Size(153, 15);
            label12.TabIndex = 23;
            label12.Text = "Url where files are expected:";
            // 
            // tbExpectedUrl
            // 
            tbExpectedUrl.Location = new Point(171, 324);
            tbExpectedUrl.Name = "tbExpectedUrl";
            tbExpectedUrl.Size = new Size(555, 23);
            tbExpectedUrl.TabIndex = 22;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 298);
            label11.Name = "label11";
            label11.Size = new Size(49, 15);
            label11.TabIndex = 21;
            label11.Text = "BaseUrl:";
            // 
            // tbBaseUrl
            // 
            tbBaseUrl.Location = new Point(118, 295);
            tbBaseUrl.Name = "tbBaseUrl";
            tbBaseUrl.Size = new Size(608, 23);
            tbBaseUrl.TabIndex = 20;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(12, 269);
            label10.Name = "label10";
            label10.Size = new Size(57, 15);
            label10.TabIndex = 19;
            label10.Text = "ogImage:";
            // 
            // tbOgImage
            // 
            tbOgImage.Location = new Point(118, 266);
            tbOgImage.Name = "tbOgImage";
            tbOgImage.Size = new Size(608, 23);
            tbOgImage.TabIndex = 9;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 240);
            label9.Name = "label9";
            label9.Size = new Size(47, 15);
            label9.TabIndex = 17;
            label9.Text = "ogTitle:";
            // 
            // tbOgTitle
            // 
            tbOgTitle.Location = new Point(118, 237);
            tbOgTitle.Name = "tbOgTitle";
            tbOgTitle.Size = new Size(608, 23);
            tbOgTitle.TabIndex = 8;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 211);
            label8.Name = "label8";
            label8.Size = new Size(53, 15);
            label8.TabIndex = 15;
            label8.Text = "Ftp host:";
            // 
            // tbFtpHost
            // 
            tbFtpHost.Location = new Point(118, 208);
            tbFtpHost.Name = "tbFtpHost";
            tbFtpHost.Size = new Size(608, 23);
            tbFtpHost.TabIndex = 7;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 182);
            label7.Name = "label7";
            label7.Size = new Size(53, 15);
            label7.TabIndex = 13;
            label7.Text = "Ftp pass:";
            // 
            // tbFtpPass
            // 
            tbFtpPass.Location = new Point(118, 179);
            tbFtpPass.Name = "tbFtpPass";
            tbFtpPass.Size = new Size(608, 23);
            tbFtpPass.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 153);
            label6.Name = "label6";
            label6.Size = new Size(52, 15);
            label6.TabIndex = 11;
            label6.Text = "Ftp user:";
            // 
            // tbFtpUser
            // 
            tbFtpUser.Location = new Point(118, 150);
            tbFtpUser.Name = "tbFtpUser";
            tbFtpUser.Size = new Size(608, 23);
            tbFtpUser.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 124);
            label5.Name = "label5";
            label5.Size = new Size(75, 15);
            label5.TabIndex = 9;
            label5.Text = "Images path:";
            // 
            // imagesPath
            // 
            imagesPath.Location = new Point(118, 121);
            imagesPath.Name = "imagesPath";
            imagesPath.Size = new Size(608, 23);
            imagesPath.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 41);
            label4.Name = "label4";
            label4.Size = new Size(103, 15);
            label4.TabIndex = 7;
            label4.Text = "Gps location path:";
            // 
            // tbGpsLocationsPath
            // 
            tbGpsLocationsPath.Location = new Point(118, 41);
            tbGpsLocationsPath.Name = "tbGpsLocationsPath";
            tbGpsLocationsPath.Size = new Size(608, 23);
            tbGpsLocationsPath.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 15);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 5;
            label3.Text = "Address:";
            // 
            // address
            // 
            address.Location = new Point(118, 12);
            address.Name = "address";
            address.Size = new Size(608, 23);
            address.TabIndex = 0;
            // 
            // folderName
            // 
            folderName.Location = new Point(118, 95);
            folderName.Name = "folderName";
            folderName.Size = new Size(157, 23);
            folderName.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 95);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 2;
            label2.Text = "Folder name:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 69);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 1;
            label1.Text = "Kml file name:";
            // 
            // kmlFileName
            // 
            kmlFileName.Location = new Point(118, 66);
            kmlFileName.Name = "kmlFileName";
            kmlFileName.Size = new Size(608, 23);
            kmlFileName.TabIndex = 2;
            // 
            // UploadImage
            // 
            UploadImage.Dock = DockStyle.Bottom;
            UploadImage.Location = new Point(0, 726);
            UploadImage.Name = "UploadImage";
            UploadImage.Size = new Size(926, 23);
            UploadImage.TabIndex = 3;
            UploadImage.Text = "UploadImage";
            UploadImage.UseVisualStyleBackColor = true;
            UploadImage.Click += UploadImage_Click;
            // 
            // UploadToBlog
            // 
            UploadToBlog.Dock = DockStyle.Bottom;
            UploadToBlog.Location = new Point(0, 749);
            UploadToBlog.Name = "UploadToBlog";
            UploadToBlog.Size = new Size(926, 23);
            UploadToBlog.TabIndex = 4;
            UploadToBlog.Text = "UploadToBlog";
            UploadToBlog.UseVisualStyleBackColor = true;
            UploadToBlog.Click += UploadToBlog_Click;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Bottom;
            btnCancel.Location = new Point(0, 772);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(926, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Dock = DockStyle.Bottom;
            linkLabel1.Location = new Point(0, 795);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(298, 15);
            linkLabel1.TabIndex = 7;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://kanaloa.azurewebsites.net/html/live/index.html";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(12, 559);
            label20.Name = "label20";
            label20.Size = new Size(77, 15);
            label20.TabIndex = 43;
            label20.Text = "Joomla! pass:";
            // 
            // tbJoomlaPass
            // 
            tbJoomlaPass.Location = new Point(171, 556);
            tbJoomlaPass.Name = "tbJoomlaPass";
            tbJoomlaPass.Size = new Size(555, 23);
            tbJoomlaPass.TabIndex = 42;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(926, 810);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(PostGpsPositionsFromFilesWithFileName);
            Controls.Add(UploadImage);
            Controls.Add(UploadToBlog);
            Controls.Add(btnCancel);
            Controls.Add(linkLabel1);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button PostGpsPositionsFromFilesWithFileName;
        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private TextBox kmlFileName;
        private TextBox log;
        private TextBox folderName;
        private Label label2;
        private Label label3;
        private TextBox address;
        private Label label4;
        private TextBox tbGpsLocationsPath;
        private Button UploadImage;
        private Label label5;
        private TextBox imagesPath;
        private Button UploadToBlog;
        private Label label8;
        private TextBox tbFtpHost;
        private Label label7;
        private TextBox tbFtpPass;
        private Label label6;
        private TextBox tbFtpUser;
        private Button btnCancel;
        private LinkLabel linkLabel1;
        private Label label9;
        private TextBox tbOgTitle;
        private Label label10;
        private TextBox tbOgImage;
        private Label label11;
        private TextBox tbBaseUrl;
        private Label label12;
        private TextBox tbExpectedUrl;
        private Button btnDefaultExpectedUrl;
        private Button button1;
        private Label label13;
        private TextBox tbPrepareForUploadUrl;
        private Button btnAzureAddress;
        private Button btnLocalAddress;
        private Label label14;
        private TextBox tbDeleteLastKmlPoints;
        private Label label15;
        private TextBox tbDeleteFirstKmlPoints;
        private Label label16;
        private TextBox textBox1;
        private Label label19;
        private TextBox tbJoomlaUserName;
        private Label label18;
        private TextBox tbJoomlaPostUrl;
        private Label label17;
        private TextBox tbJoomlaLoginUrl;
        private TextBox tbJoomlaCategoryId;
        private Label label20;
        private TextBox tbJoomlaPass;
    }
}
