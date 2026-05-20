namespace PortMonitor
{
    partial class FormAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pboxAboutLogo = new PictureBox();
            lblAboutTitle = new Label();
            lblAboutSubtitle = new Label();
            rtbAboutInfo = new RichTextBox();
            pnlHeader = new Panel();
            pnlFooter = new Panel();
            lblGithub = new LinkLabel();
            lblAboutCopyright = new Label();
            btnClose = new Button();
            ((System.ComponentModel.ISupportInitialize)pboxAboutLogo).BeginInit();
            pnlHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pboxAboutLogo
            // 
            pboxAboutLogo.Location = new Point(12, 12);
            pboxAboutLogo.Name = "pboxAboutLogo";
            pboxAboutLogo.Size = new Size(64, 64);
            pboxAboutLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pboxAboutLogo.TabIndex = 0;
            pboxAboutLogo.TabStop = false;
            // 
            // lblAboutTitle
            // 
            lblAboutTitle.AutoSize = true;
            lblAboutTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblAboutTitle.Location = new Point(90, 12);
            lblAboutTitle.Name = "lblAboutTitle";
            lblAboutTitle.Size = new Size(164, 32);
            lblAboutTitle.TabIndex = 1;
            lblAboutTitle.Text = "Port Monitor";
            // 
            // lblAboutSubtitle
            // 
            lblAboutSubtitle.AutoSize = true;
            lblAboutSubtitle.Font = new Font("Segoe UI", 10F);
            lblAboutSubtitle.Location = new Point(92, 50);
            lblAboutSubtitle.Name = "lblAboutSubtitle";
            lblAboutSubtitle.Size = new Size(221, 19);
            lblAboutSubtitle.TabIndex = 2;
            lblAboutSubtitle.Text = "Lightweight port monitoring utility";
            // 
            // rtbAboutInfo
            // 
            rtbAboutInfo.BorderStyle = BorderStyle.FixedSingle;
            rtbAboutInfo.Font = new Font("Segoe UI", 10F);
            rtbAboutInfo.Location = new Point(12, 100);
            rtbAboutInfo.Name = "rtbAboutInfo";
            rtbAboutInfo.ReadOnly = true;
            rtbAboutInfo.Size = new Size(515, 150);
            rtbAboutInfo.TabIndex = 0;
            rtbAboutInfo.Text = "";
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);
            pnlHeader.Controls.Add(pboxAboutLogo);
            pnlHeader.Controls.Add(lblAboutTitle);
            pnlHeader.Controls.Add(lblAboutSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(12);
            pnlHeader.Size = new Size(539, 90);
            pnlHeader.TabIndex = 1;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(245, 245, 245);
            pnlFooter.Controls.Add(lblGithub);
            pnlFooter.Controls.Add(lblAboutCopyright);
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 260);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(12);
            pnlFooter.Size = new Size(539, 40);
            pnlFooter.TabIndex = 2;
            // 
            // lblGithub
            // 
            lblGithub.AutoSize = true;
            lblGithub.Location = new Point(120, 12);
            lblGithub.Name = "lblGithub";
            lblGithub.Size = new Size(104, 15);
            lblGithub.TabIndex = 2;
            lblGithub.TabStop = true;
            lblGithub.Text = "GitHub Repository";
            lblGithub.LinkClicked += lblGithub_LinkClicked;
            // 
            // lblAboutCopyright
            // 
            lblAboutCopyright.AutoSize = true;
            lblAboutCopyright.Location = new Point(12, 12);
            lblAboutCopyright.Name = "lblAboutCopyright";
            lblAboutCopyright.Size = new Size(81, 15);
            lblAboutCopyright.TabIndex = 0;
            lblAboutCopyright.Text = "© 2026 eyebiz";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(440, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // FormAbout
            // 
            ClientSize = new Size(539, 300);
            Controls.Add(rtbAboutInfo);
            Controls.Add(pnlHeader);
            Controls.Add(pnlFooter);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAbout";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            ((System.ComponentModel.ISupportInitialize)pboxAboutLogo).EndInit();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pboxAboutLogo;
        private Label lblAboutTitle;
        private Label lblAboutCopyright;
        private Label lblAboutSubtitle;
        private RichTextBox rtbAboutInfo;
        private Panel pnlHeader;
        private Panel pnlFooter;
        private Button btnClose;
        private LinkLabel lblGithub;
    }
}