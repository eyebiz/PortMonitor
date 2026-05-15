namespace PortMonitor
{
    partial class AboutForm
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
            lstAboutInfo = new ListBox();
            lblAboutCopyright = new Label();
            ((System.ComponentModel.ISupportInitialize)pboxAboutLogo).BeginInit();
            SuspendLayout();
            // 
            // pboxAboutLogo
            // 
            pboxAboutLogo.Location = new Point(12, 12);
            pboxAboutLogo.Name = "pboxAboutLogo";
            pboxAboutLogo.Size = new Size(66, 66);
            pboxAboutLogo.TabIndex = 0;
            pboxAboutLogo.TabStop = false;
            // 
            // lblAboutTitle
            // 
            lblAboutTitle.AutoSize = true;
            lblAboutTitle.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAboutTitle.Location = new Point(101, 12);
            lblAboutTitle.Name = "lblAboutTitle";
            lblAboutTitle.Size = new Size(142, 30);
            lblAboutTitle.TabIndex = 1;
            lblAboutTitle.Text = "Port Monitor";
            // 
            // lstAboutInfo
            // 
            lstAboutInfo.FormattingEnabled = true;
            lstAboutInfo.Location = new Point(101, 55);
            lstAboutInfo.Name = "lstAboutInfo";
            lstAboutInfo.Size = new Size(417, 214);
            lstAboutInfo.TabIndex = 2;
            // 
            // lblAboutCopyright
            // 
            lblAboutCopyright.AutoSize = true;
            lblAboutCopyright.Location = new Point(22, 95);
            lblAboutCopyright.Name = "lblAboutCopyright";
            lblAboutCopyright.Size = new Size(45, 15);
            lblAboutCopyright.TabIndex = 3;
            lblAboutCopyright.Text = "© 2026";
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(539, 290);
            Controls.Add(lblAboutCopyright);
            Controls.Add(lstAboutInfo);
            Controls.Add(lblAboutTitle);
            Controls.Add(pboxAboutLogo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            ((System.ComponentModel.ISupportInitialize)pboxAboutLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pboxAboutLogo;
        private Label lblAboutTitle;
        private ListBox lstAboutInfo;
        private Label lblAboutCopyright;
    }
}