namespace PortMonitor
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
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            btnExit = new Button();
            btnConfig = new Button();
            btnStart = new Button();
            txtPort = new TextBox();
            lblPort = new Label();
            lstIPs = new ListBox();
            panel2 = new Panel();
            btnCopyIPs = new Button();
            tabControl1 = new TabControl();
            tabLog = new TabPage();
            richTextBox1 = new RichTextBox();
            tabMap = new TabPage();
            lstGeoData = new ListBox();
            pboxMap = new PictureBox();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            restoreToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            configToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem1 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabLog.SuspendLayout();
            tabMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pboxMap).BeginInit();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Controls.Add(lstIPs, 1, 2);
            tableLayoutPanel1.Controls.Add(panel2, 1, 1);
            tableLayoutPanel1.Controls.Add(tabControl1, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.Size = new Size(989, 590);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnExit);
            panel1.Controls.Add(btnConfig);
            panel1.Controls.Add(btnStart);
            panel1.Controls.Add(txtPort);
            panel1.Controls.Add(lblPort);
            panel1.Location = new Point(3, 28);
            panel1.Name = "panel1";
            panel1.Size = new Size(794, 34);
            panel1.TabIndex = 1;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(456, 5);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 4;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // btnConfig
            // 
            btnConfig.Location = new Point(375, 5);
            btnConfig.Name = "btnConfig";
            btnConfig.Size = new Size(75, 23);
            btnConfig.TabIndex = 3;
            btnConfig.Text = "Config";
            btnConfig.UseVisualStyleBackColor = true;
            btnConfig.Click += btnConfig_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(202, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 2;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(96, 5);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(100, 23);
            txtPort.TabIndex = 1;
            txtPort.Text = "8080";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(7, 8);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(83, 15);
            lblPort.TabIndex = 0;
            lblPort.Text = "Listen on Port:";
            // 
            // lstIPs
            // 
            lstIPs.Dock = DockStyle.Fill;
            lstIPs.FormattingEnabled = true;
            lstIPs.IntegralHeight = false;
            lstIPs.Location = new Point(832, 68);
            lstIPs.Name = "lstIPs";
            lstIPs.Size = new Size(154, 494);
            lstIPs.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCopyIPs);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(832, 28);
            panel2.Name = "panel2";
            panel2.Size = new Size(154, 34);
            panel2.TabIndex = 8;
            // 
            // btnCopyIPs
            // 
            btnCopyIPs.Location = new Point(17, 4);
            btnCopyIPs.Name = "btnCopyIPs";
            btnCopyIPs.Size = new Size(119, 24);
            btnCopyIPs.TabIndex = 7;
            btnCopyIPs.Text = "Save to Clipboard";
            btnCopyIPs.UseVisualStyleBackColor = true;
            btnCopyIPs.Click += btnCopyIPs_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabLog);
            tabControl1.Controls.Add(tabMap);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(3, 68);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(823, 494);
            tabControl1.TabIndex = 9;
            // 
            // tabLog
            // 
            tabLog.Controls.Add(richTextBox1);
            tabLog.Location = new Point(4, 24);
            tabLog.Name = "tabLog";
            tabLog.Padding = new Padding(3);
            tabLog.Size = new Size(815, 466);
            tabLog.TabIndex = 0;
            tabLog.Text = "Log";
            tabLog.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.Location = new Point(3, 3);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(809, 460);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // tabMap
            // 
            tabMap.Controls.Add(lstGeoData);
            tabMap.Controls.Add(pboxMap);
            tabMap.Location = new Point(4, 24);
            tabMap.Name = "tabMap";
            tabMap.Padding = new Padding(3);
            tabMap.Size = new Size(815, 466);
            tabMap.TabIndex = 1;
            tabMap.Text = "Map";
            tabMap.UseVisualStyleBackColor = true;
            // 
            // lstGeoData
            // 
            lstGeoData.BackColor = SystemColors.Menu;
            lstGeoData.FormattingEnabled = true;
            lstGeoData.Location = new Point(6, 6);
            lstGeoData.Name = "lstGeoData";
            lstGeoData.Size = new Size(267, 154);
            lstGeoData.TabIndex = 1;
            // 
            // pboxMap
            // 
            pboxMap.BackColor = Color.Gainsboro;
            pboxMap.Dock = DockStyle.Fill;
            pboxMap.Location = new Point(3, 3);
            pboxMap.Name = "pboxMap";
            pboxMap.Size = new Size(809, 460);
            pboxMap.SizeMode = PictureBoxSizeMode.CenterImage;
            pboxMap.TabIndex = 0;
            pboxMap.TabStop = false;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "Port Monitor";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { restoreToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(114, 48);
            // 
            // restoreToolStripMenuItem
            // 
            restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            restoreToolStripMenuItem.Size = new Size(113, 22);
            restoreToolStripMenuItem.Text = "Restore";
            restoreToolStripMenuItem.Click += restoreToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(113, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 568);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(989, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(26, 17);
            lblStatus.Text = "Idle";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(989, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { configToolStripMenuItem, exitToolStripMenuItem1 });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // configToolStripMenuItem
            // 
            configToolStripMenuItem.Name = "configToolStripMenuItem";
            configToolStripMenuItem.Size = new Size(110, 22);
            configToolStripMenuItem.Text = "Config";
            configToolStripMenuItem.Click += configToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem1
            // 
            exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            exitToolStripMenuItem1.Size = new Size(110, 22);
            exitToolStripMenuItem1.Text = "Exit";
            exitToolStripMenuItem1.Click += exitToolStripMenuItem1_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(989, 590);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new Size(800, 500);
            Name = "Form1";
            Text = "Port Monitor";
            FormClosing += Form1_FormClosing;
            Resize += Form1_Resize;
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabLog.ResumeLayout(false);
            tabMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pboxMap).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Label lblPort;
        private TextBox txtPort;
        private Button btnConfig;
        private Button btnStart;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem restoreToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ListBox lstIPs;
        private Panel panel2;
        private Button btnCopyIPs;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private Button btnExit;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabLog;
        private RichTextBox richTextBox1;
        private TabPage tabMap;
        private PictureBox pboxMap;
        private ListBox lstGeoData;
    }
}
