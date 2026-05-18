namespace PortMonitor
{
    partial class FormSettings
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
            chkCloseToTray = new CheckBox();
            chkMinimizeToTray = new CheckBox();
            chkOpenMinimized = new CheckBox();
            btnSaveSettings = new Button();
            groupBoxGeneral = new GroupBox();
            SuspendLayout();

            // groupBoxGeneral
            groupBoxGeneral.Text = "General";
            groupBoxGeneral.Location = new Point(12, 12);
            groupBoxGeneral.Size = new Size(260, 120);

            // chkCloseToTray
            chkCloseToTray.AutoSize = true;
            chkCloseToTray.Location = new Point(15, 25);
            chkCloseToTray.Text = "Close to Tray";

            // chkMinimizeToTray
            chkMinimizeToTray.AutoSize = true;
            chkMinimizeToTray.Location = new Point(15, 50);
            chkMinimizeToTray.Text = "Minimize to Tray";

            // chkOpenMinimized
            chkOpenMinimized.AutoSize = true;
            chkOpenMinimized.Location = new Point(15, 75);
            chkOpenMinimized.Text = "Open Minimized";

            // btnSaveSettings
            btnSaveSettings.Location = new Point(197, 140);
            btnSaveSettings.Size = new Size(75, 23);
            btnSaveSettings.Text = "Save";
            btnSaveSettings.Click += btnSaveSettings_Click;

            // Add controls
            groupBoxGeneral.Controls.Add(chkCloseToTray);
            groupBoxGeneral.Controls.Add(chkMinimizeToTray);
            groupBoxGeneral.Controls.Add(chkOpenMinimized);

            Controls.Add(groupBoxGeneral);
            Controls.Add(btnSaveSettings);

            // Form
            ClientSize = new Size(284, 180);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
        }

        #endregion

        private CheckBox chkCloseToTray;
        private CheckBox chkOpenMinimized;
        private Button btnSaveSettings;
        private CheckBox chkMinimizeToTray;
        private GroupBox groupBoxGeneral;
    }
}