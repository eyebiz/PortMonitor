
using PortMonitor.Models;
using PortMonitor.Services;

namespace PortMonitor
{
    public partial class FormSettings : Form
    {
        private readonly SettingsService _settingsService;
        private AppSettings _currentSettings;

        public FormSettings()
        {
            InitializeComponent();

            // Load existing settings and check the boxes
            _settingsService = new SettingsService();
            _currentSettings = _settingsService.Load();
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
            _settingsService.Save(_currentSettings);

            this.DialogResult = DialogResult.OK; // Signal to Form1 that things changed
            this.Close();
        }
    }
}
