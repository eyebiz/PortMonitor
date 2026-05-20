using PortMonitor.Models;
using PortMonitor.Services;
using System.Globalization;

namespace PortMonitor
{
    public partial class Form1 : Form
    {
        private readonly SettingsService _settingsService;
        private AppSettings _settings;
        private readonly PortMonitorService _monitor;
        private readonly LogService _log;
        private readonly MapService _map;
        private readonly GeoIpService _geo;

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.AppIcon;
            notifyIcon1.Icon = Properties.Resources.AppIcon;
            lblStatus.Text = Strings.StatusIdle;

            _settingsService = new SettingsService();
            _settings = _settingsService.Load();
            _monitor = new PortMonitorService();
            _log = new LogService();
            _map = new MapService();
            _geo = new GeoIpService();

            // Subscribe to service events
            _monitor.ConnectionReceived += OnConnectionReceived;
            _monitor.StatusChanged += OnStatusChanged;
            _monitor.MonitoringStopped += OnMonitoringStopped;

            lstIPs.SelectedIndexChanged += lstIPs_SelectedIndexChanged;
            // Uncomment this for testing 
            //lstIPs.Items.AddRange(["45.198.224.9", "147.185.132.49", "8.216.5.202", "138.68.152.66", "79.124.40.174", "64.227.150.86"]);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_settings.OpenMinimized)
            {
                // We set this to true so it's visible in the tray
                notifyIcon1.Visible = true;

                // Hide the main window immediately
                this.BeginInvoke(new MethodInvoker(delegate { this.Hide(); }));
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (!_monitor.IsRunning)
            {
                if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
                {
                    lblStatus.Text = "Port must be between 1 and 65535";
                    return;
                }

                string startMsg = _log.StartNewLog(port);
                richTextLog.AppendText(startMsg);
                richTextLog.ScrollToCaret();
                await _monitor.StartAsync(port);
                btnStart.Text = "Stop";
                txtPort.Enabled = false;
            }
            else
            {
                string stopMsg = _log.StopLog();
                richTextLog.AppendText(stopMsg);
                richTextLog.ScrollToCaret();
                await _monitor.StopAsync();
                btnStart.Text = "Start";
                txtPort.Enabled = true;
            }
        }

        private void OnStatusChanged(string status)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnStatusChanged(status));
                return;
            }
            lblStatus.Text = status;
        }

        private void OnMonitoringStopped()
        {
            if (InvokeRequired)
            {
                Invoke(OnMonitoringStopped);
                return;
            }
            lblStatus.Text = Strings.StatusIdle;
        }

        private void OnConnectionReceived(ConnectionDetails d)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnConnectionReceived(d));
                return;
            }

            // Format + append to UI
            string formatted = _log.FormatConnection(d);
            richTextLog.AppendText(formatted);
            richTextLog.ScrollToCaret();

            // Add IP to list if new
            if (!lstIPs.Items.Contains(d.RemoteIp))
                lstIPs.Items.Add(d.RemoteIp);

            // Write to file
            _log.AppendConnection(d);
        }

        private async Task ShowTemporaryStatus(string message, int ms = 5000)
        {
            lblStatus.Text = message;
            string snapshot = message; // capture what we set
            await Task.Delay(ms);

            if (_monitor.IsRunning && lblStatus.Text == snapshot)
            {
                lblStatus.Text = string.Format(Strings.StatusListening, txtPort.Text);
            }
        }

        private async void lstIPs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstIPs.SelectedItem is not string ip)
                return;

            try
            {
                var info = await _geo.LookupAsync(ip);
                lstGeoData.Items.Clear();
                lstGeoData.Items.AddRange(GeoDataFormatter.Format(info).ToArray());
                CenterMap(info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching IP info: " + ex.Message);
            }
        }

        private async void CenterMap(IpInfoResponse info)
        {
            if (string.IsNullOrWhiteSpace(info.loc))
                return;

            var parts = info.loc.Split(',');
            if (parts.Length != 2)
                return;

            if (double.TryParse(parts[0], CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(parts[1], CultureInfo.InvariantCulture, out double lon))
            {
                pboxMap.Image = await _map.RenderMapAsync(lat, lon, 9);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the user clicked the [X] and settings allow traying
            if (e.CloseReason == CloseReason.UserClosing && _settings.CloseToTray)
            {
                e.Cancel = true; // Stop the app from actually closing
                this.Hide();     // Hide the window

                // Optional: Show a "balloon" tip the first time it happens
                //notifyIcon1.ShowBalloonTip(2000, "Port Monitor", "Running in background.", ToolTipIcon.Info);
                return;
            }
            if (_monitor.IsRunning) { _ = _monitor.StopAsync(); }
        }

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (FormSettings settingsWindow = new FormSettings())
            {
                if (settingsWindow.ShowDialog() == DialogResult.OK)
                {
                    _settings = _settingsService.Load();
                    await ShowTemporaryStatus("Settings saved.");
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate(); // Bring to front
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settings.CloseToTray = false;
            Application.Exit();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate(); // Bring to front
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // If the window is minimized AND the user has "Minimize to Tray" enabled
            if (this.WindowState == FormWindowState.Minimized && _settings.MinimizeToTray)
            {
                this.Hide();               // Remove from screen
                this.ShowInTaskbar = false; // Remove from taskbar
                notifyIcon1.Visible = true; // Ensure tray icon is visible
            }
        }

        private async void btnCopyIPs_Click(object sender, EventArgs e)
        {
            if (lstIPs.Items.Count > 0)
            {
                string allIPs = string.Join(Environment.NewLine, lstIPs.Items.Cast<string>());
                Clipboard.SetText(allIPs);
                await ShowTemporaryStatus($"{lstIPs.Items.Count} IPs copied to clipboard.");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _settings.CloseToTray = false;
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormSettings settingsWindow = new FormSettings())
            {
                if (settingsWindow.ShowDialog() == DialogResult.OK)
                {
                    _settings = _settingsService.Load();
                }
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _settings.CloseToTray = false;
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormAbout about = new FormAbout())
            {
                about.ShowDialog();
            }
        }
    }
}
