
namespace PortMonitor
{
    public partial class SettingsForm : Form
    {
        private AppSettings _currentSettings;

        public SettingsForm()
        {
            InitializeComponent();

            // Load existing settings and check the boxes
            _currentSettings = ConfigManager.Load();
            chkCloseToTray.Checked = _currentSettings.CloseToTray;
            chkMinimizeToTray.Checked = _currentSettings.MinimizeToTray;
            chkOpenMinimized.Checked = _currentSettings.OpenMinimized;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            // Update the object with UI values
            _currentSettings.CloseToTray = chkCloseToTray.Checked;
            _currentSettings.MinimizeToTray = chkMinimizeToTray.Checked;
            _currentSettings.OpenMinimized = chkOpenMinimized.Checked;

            // Save to file
            ConfigManager.Save(_currentSettings);

            this.DialogResult = DialogResult.OK; // Signal to Form1 that things changed
            this.Close();
        }
    }
}
