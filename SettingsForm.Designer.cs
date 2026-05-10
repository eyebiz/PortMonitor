namespace PortMonitor
{
    partial class SettingsForm
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
            chkOpenMinimized = new CheckBox();
            btnSaveSettings = new Button();
            chkMinimizeToTray = new CheckBox();
            SuspendLayout();
            // 
            // chkCloseToTray
            // 
            chkCloseToTray.AutoSize = true;
            chkCloseToTray.Location = new Point(12, 22);
            chkCloseToTray.Name = "chkCloseToTray";
            chkCloseToTray.Size = new Size(93, 19);
            chkCloseToTray.TabIndex = 0;
            chkCloseToTray.Text = "Close to Tray";
            chkCloseToTray.UseVisualStyleBackColor = true;
            // 
            // chkOpenMinimized
            // 
            chkOpenMinimized.AutoSize = true;
            chkOpenMinimized.Location = new Point(12, 70);
            chkOpenMinimized.Name = "chkOpenMinimized";
            chkOpenMinimized.Size = new Size(114, 19);
            chkOpenMinimized.TabIndex = 1;
            chkOpenMinimized.Text = "Open Minimized";
            chkOpenMinimized.UseVisualStyleBackColor = true;
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Location = new Point(219, 45);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(75, 23);
            btnSaveSettings.TabIndex = 2;
            btnSaveSettings.Text = "Save";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;
            // 
            // chkMinimizeToTray
            // 
            chkMinimizeToTray.AutoSize = true;
            chkMinimizeToTray.Location = new Point(12, 45);
            chkMinimizeToTray.Name = "chkMinimizeToTray";
            chkMinimizeToTray.Size = new Size(113, 19);
            chkMinimizeToTray.TabIndex = 3;
            chkMinimizeToTray.Text = "Minimize to Tray";
            chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(306, 107);
            Controls.Add(chkMinimizeToTray);
            Controls.Add(btnSaveSettings);
            Controls.Add(chkOpenMinimized);
            Controls.Add(chkCloseToTray);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkCloseToTray;
        private CheckBox chkOpenMinimized;
        private Button btnSaveSettings;
        private CheckBox chkMinimizeToTray;
    }
}