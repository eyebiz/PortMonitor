using Microsoft.VisualBasic.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace PortMonitor
{
    public partial class Form1 : Form
    {
        private TcpListener? _listener;
        private bool _isMonitoring = false;
        private CancellationTokenSource? _cts;
        private string _currentLogPath = "";
        private AppSettings _settings;

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.AppIcon;
            notifyIcon1.Icon = Properties.Resources.AppIcon;
            _settings = ConfigManager.Load();
            lstIPs.SelectedIndexChanged += lstIPs_SelectedIndexChanged;
            // Uncomment this for testing 
            lstIPs.Items.AddRange(["45.198.224.9", "147.185.132.49", "8.216.5.202", "138.68.152.66", "79.124.40.174", "64.227.150.86"]);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_settings.OpenMinimized)
            {
                // We set this to true so it's visible in the tray
                notifyIcon1.Visible = true;

                // Hide the main window immediately
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    this.Hide();
                }));
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                if (!int.TryParse(txtPort.Text, out int port))
                {
                    lblStatus.Text = "Invalid Port Number";
                    return;
                }

                // 1. Setup the Log Directory
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                // 2. Setup the Filename: Log_2026-05-10.txt
                string fileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
                _currentLogPath = Path.Combine(logDir, fileName);

                // 3. Overwrite/Create the file immediately
                string logEntry = $"--- Monitor Started on Port {txtPort.Text} at {DateTime.Now} ---{Environment.NewLine}";
                richTextBox1.AppendText(logEntry);
                File.AppendAllText(_currentLogPath, logEntry);

                try
                {
                    _isMonitoring = true;
                    btnStart.Text = "Stop";
                    txtPort.Enabled = false;
                    lblStatus.Text = $"Listening on port {port}...";

                    _cts = new CancellationTokenSource();
                    _listener = new TcpListener(IPAddress.Any, port);
                    _listener.Start();

                    // Start the background listening loop
                    _ = Task.Run(() => ListenLoop(_cts.Token));
                }
                catch (Exception ex)
                {
                    StopMonitoring();
                    lblStatus.Text = $"Error: {ex.Message}";
                }
            }
            else
            {
                StopMonitoring();
            }
        }

        private async Task ListenLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using var client = await _listener!.AcceptTcpClientAsync(token);

                    string remoteIp = client.Client.RemoteEndPoint is IPEndPoint ep
                        ? ep.Address.ToString()
                        : "Unknown";

                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                    timeoutCts.CancelAfter(3000);

                    byte[] buffer = new byte[4096]; // Increased buffer to capture more headers
                    int bytesRead = 0;
                    string rawData = "";

                    try
                    {
                        var stream = client.GetStream();
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, timeoutCts.Token);
                        if (bytesRead > 0)
                        {
                            rawData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    }
                    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                    {
                        rawData = "[Timeout]";
                    }

                    // 1. Sanitize and Extract Request Line (First line)
                    string[] lines = rawData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    string firstLine = lines.Length > 0 ? lines[0] : "[No Data]";
                    string tool = Regex.Replace(firstLine, @"[^\x20-\x7E]", ".");
                    if (tool.Length > 100) tool = tool.Substring(0, 100) + "...";

                    // 2. Extract specific Headers
                    string userAgent = "Not Found";
                    string referer = "Not Found";

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("User-Agent:", StringComparison.OrdinalIgnoreCase))
                            userAgent = line.Substring(11).Trim();
                        if (line.StartsWith("Referer:", StringComparison.OrdinalIgnoreCase))
                            referer = line.Substring(8).Trim();
                    }

                    // 3. Build Log Entry
                    string logEntry = $"[{DateTime.Now:HH:mm:ss}] Connection from {remoteIp}{Environment.NewLine}";
                    logEntry += $"   Request: {tool}{Environment.NewLine}";
                    logEntry += $"   Agent:   {userAgent}{Environment.NewLine}";
                    if (referer != "Not Found") logEntry += $"   Referer: {referer}{Environment.NewLine}";
                    string separator = new string('-', 40) + Environment.NewLine;

                    // 4. Update UI (Thread-safe)
                    Invoke(() =>
                    {
                        richTextBox1.AppendText(logEntry + separator);
                        richTextBox1.ScrollToCaret();

                        if (!lstIPs.Items.Contains(remoteIp))
                        {
                            lstIPs.Items.Add(remoteIp);
                        }
                    });

                    // 5. Write to File
                    try { await File.AppendAllTextAsync(_currentLogPath, logEntry + separator); }
                    catch { /* Handle file locks */ }
                }
            }
            catch (OperationCanceledException) { /* App stopped */ }
            catch (Exception ex)
            {
                Invoke(() => lblStatus.Text = $"Error: {ex.Message}");
            }
        }

        private void StopMonitoring()
        {
            _isMonitoring = false;
            _cts?.Cancel();
            _listener?.Stop();
            btnStart.Text = "Start";
            txtPort.Enabled = true;
            lblStatus.Text = "Idle";
            if (!string.IsNullOrEmpty(_currentLogPath))
            {
                string logEntry = $"--- Monitor Stopped at {DateTime.Now} ---{Environment.NewLine}";
                richTextBox1.AppendText(logEntry);
                File.AppendAllText(_currentLogPath, logEntry);
            }
        }

        private async Task ShowTemporaryStatus(string message, int ms = 5000)
        {
            lblStatus.Text = message;
            string snapshot = message; // capture what we set
            await Task.Delay(ms);

            if (_isMonitoring && lblStatus.Text == snapshot)
            {
                lblStatus.Text = $"Listening on port {txtPort.Text}...";
            }
        }

        public async Task<Image> RenderMapAsync(double lat, double lon, int zoom)
        {
            var (cx, cy) = LatLonToTile(lat, lon, zoom);

            const int tileSize = 256;
            Bitmap bmp = new Bitmap(tileSize * 3, tileSize * 3);
            using Graphics g = Graphics.FromImage(bmp);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int tx = cx + dx;
                    int ty = cy + dy;

                    try
                    {
                        Image tile = await DownloadTileAsync(tx, ty, zoom);
                        g.DrawImage(tile, (dx + 1) * tileSize, (dy + 1) * tileSize);
                    }
                    catch
                    {
                        // Draw placeholder if tile fails
                        g.FillRectangle(Brushes.LightGray, (dx + 1) * tileSize, (dy + 1) * tileSize, tileSize, tileSize);
                    }
                }
            }

            // Required attribution
            g.DrawString("© OpenStreetMap contributors",
                new Font("Arial", 10),
                Brushes.Black,
                new PointF(5, bmp.Height - 20));

            // Draw a target marker at the center
            int centerX = bmp.Width / 2;
            int centerY = bmp.Height / 2;
            using Pen pen = new Pen(Color.Red, 2);

            // Circle
            g.DrawEllipse(pen, centerX - 6, centerY - 6, 12, 12);
            return bmp;
        }

        private static (int x, int y) LatLonToTile(double lat, double lon, int zoom)
        {
            double latRad = lat * Math.PI / 180.0;
            int n = 1 << zoom;

            int x = (int)((lon + 180.0) / 360.0 * n);
            int y = (int)((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);

            return (x, y);
        }

        private static async Task<Image> DownloadTileAsync(int x, int y, int zoom)
        {
            string url = $"https://tile.openstreetmap.de/{zoom}/{x}/{y}.png";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Port Monitor/1.0 (contact: your-email-or-website)");

            var bytes = await client.GetByteArrayAsync(url);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }

        private async void LoadMap(double lat, double lon, int zoom)
        {
            pboxMap.Image = await RenderMapAsync(lat, lon, zoom);
        }

        public async Task<IpInfoResponse> LookupIpInfoAsync(string ip)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://ipinfo.io/{ip}/json";
            string json = await client.GetStringAsync(url);

            return System.Text.Json.JsonSerializer.Deserialize<IpInfoResponse>(json)!;
        }

        private async void lstIPs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstIPs.SelectedItem is not string ip)
                return;

            try
            {
                var info = await LookupIpInfoAsync(ip);
                DisplayGeoData(info);
                CenterMapOnIp(info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching IP info: " + ex.Message);
            }
        }

        private void DisplayGeoData(IpInfoResponse info)
        {
            lstGeoData.Items.Clear();

            lstGeoData.Items.Add($"IP: {info.ip}");
            lstGeoData.Items.Add($"Hostname: {info.hostname}");
            lstGeoData.Items.Add($"City: {info.city}");
            lstGeoData.Items.Add($"Region: {info.region}");
            lstGeoData.Items.Add($"Country: {info.country}");
            lstGeoData.Items.Add($"Location: {info.loc}");
            lstGeoData.Items.Add($"Org: {info.org}");
            lstGeoData.Items.Add($"Postal: {info.postal}");
            lstGeoData.Items.Add($"Timezone: {info.timezone}");
            //lstGeoData.Items.Add($"Anycast: {info.anycast}");
        }

        private void CenterMapOnIp(IpInfoResponse info)
        {
            if (string.IsNullOrWhiteSpace(info.loc))
                return;

            var parts = info.loc.Split(',');
            if (parts.Length != 2)
                return;

            if (double.TryParse(parts[0], System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(parts[1], System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double lon))
            {
                LoadMap(lat, lon, 9); // choose your preferred zoom
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
            if (_isMonitoring) { StopMonitoring(); }
        }

        private async void btnConfig_Click(object sender, EventArgs e)
        {
            using (SettingsForm configWindow = new SettingsForm())
            {
                if (configWindow.ShowDialog() == DialogResult.OK)
                {
                    _settings = ConfigManager.Load();
                    await ShowTemporaryStatus("Settings updated.");
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

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm configWindow = new SettingsForm())
            {
                if (configWindow.ShowDialog() == DialogResult.OK)
                {
                    _settings = ConfigManager.Load();
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
            using (AboutForm about = new AboutForm())
            {
                about.ShowDialog();
            }
        }
    }
}
