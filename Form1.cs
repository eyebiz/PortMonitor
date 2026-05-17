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
                    client.NoDelay = true; // ensure no accidental writes

                    var details = new ConnectionDetails
                    {
                        ConnectedAt = DateTime.Now,
                        RemoteIp = client.Client.RemoteEndPoint is IPEndPoint ep ? ep.Address.ToString() : "",
                        RemotePort = client.Client.RemoteEndPoint is IPEndPoint ep2 ? ep2.Port : 0,
                        LocalPort = client.Client.LocalEndPoint is IPEndPoint ep3 ? ep3.Port : 0
                    };

                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                    timeoutCts.CancelAfter(3000);

                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    byte[] rawBytes = Array.Empty<byte>();
                    DateTime? firstByteTime = null;

                    try
                    {
                        var stream = client.GetStream();

                        // READ ONLY — never write anything back
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, timeoutCts.Token);

                        if (bytesRead > 0)
                        {
                            rawBytes = buffer.Take(bytesRead).ToArray();
                            firstByteTime = DateTime.Now;
                        }
                    }
                    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                    {
                        details.TimedOut = true;
                    }

                    details.RawBytes = rawBytes;
                    details.BytesRead = bytesRead;
                    details.DisconnectedAt = DateTime.Now;

                    // Detect if client closed connection
                    try
                    {
                        details.ClientClosed =
                            client.Client.Poll(1, SelectMode.SelectRead) &&
                            client.Client.Available == 0;
                    }
                    catch
                    {
                        details.ClientClosed = false;
                    }

                    if (firstByteTime.HasValue)
                        details.TimeToFirstByte = firstByteTime.Value - details.ConnectedAt;

                    // Process raw bytes into structured fields
                    ProcessConnectionDetails(details);

                    // Log to UI + file
                    LogConnection(details);
                }
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                Invoke(() => lblStatus.Text = $"Error: {ex.Message}");
            }
        }

        private void ProcessConnectionDetails(ConnectionDetails details)
        {
            var raw = details.RawBytes;

            // First bytes (hex + ascii)
            var firstBytes = raw.Take(32).ToArray();
            details.FirstBytesHex = BitConverter.ToString(firstBytes);
            details.FirstBytesAscii = new string(firstBytes.Select(b =>
                b >= 0x20 && b <= 0x7E ? (char)b : '.').ToArray());

            // Convert to text
            string text = raw.Length > 0
                ? Encoding.UTF8.GetString(raw)
                : "";

            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            details.FirstLineRaw = lines.Length > 0 ? lines[0] : "";
            details.FirstLineSanitized = Regex.Replace(details.FirstLineRaw, @"[^\x20-\x7E]", ".");

            // Header extraction (only lines with colon)
            details.HeaderLines = new List<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                    break;

                if (line.Contains(':'))
                    details.HeaderLines.Add(line);
            }

            // Body extraction (only if headers exist)
            int blankIndex = Array.FindIndex(lines, l => string.IsNullOrWhiteSpace(l));
            if (details.HeaderLines.Count > 0 && blankIndex >= 0 && blankIndex < lines.Length - 1)
                details.Body = string.Join("\n", lines.Skip(blankIndex + 1));

            // Line stats
            details.LineCount = lines.Length;
            details.MaxLineLength = lines.Any() ? lines.Max(l => l.Length) : 0;

            // ASCII / printable / entropy
            if (raw.Length > 0)
            {
                int asciiCount = raw.Count(b => b <= 0x7F);
                int printableCount = raw.Count(b => b >= 0x20 && b <= 0x7E);

                details.AsciiPercentage = asciiCount * 100.0 / raw.Length;
                details.PrintablePercentage = printableCount * 100.0 / raw.Length;

                var groups = raw.GroupBy(b => b).Select(g => g.Count() / (double)raw.Length);
                details.Entropy = -groups.Sum(p => p * Math.Log(p, 2));
            }
        }

        private void LogConnection(ConnectionDetails details)
        {
            string logEntry =
                $"[{DateTime.Now:HH:mm:ss}] Connection from {details.RemoteIp}:{details.RemotePort} → {details.LocalPort}\n" +
                $"   Connected:     {details.ConnectedAt:HH:mm:ss.fff}\n" +
                $"   Disconnected:  {details.DisconnectedAt:HH:mm:ss.fff}\n" +
                $"   Duration:      {(details.Duration).TotalMilliseconds:F0} ms\n" +
                $"   Timed Out:     {(details.TimedOut ? "Yes" : "No")}\n" +
                $"   Client Closed: {(details.ClientClosed ? "Yes" : "No")}\n\n" +
                $"   Bytes Read:    {details.BytesRead}\n" +
                //$"   First Bytes:   {details.FirstBytesHex}\n" +
                $"   ASCII:         {details.AsciiPercentage:F1}%   Printable: {details.PrintablePercentage:F1}%\n" +
                $"   Entropy:       {details.Entropy:F2}\n\n";

            // Always show first line if printable
            if (!string.IsNullOrWhiteSpace(details.FirstLineSanitized))
                logEntry += $"   First Line:    {details.FirstLineSanitized}\n";

            // Binary payload suppression
            if (details.PrintablePercentage < 80.0)
            {
                logEntry += "   Payload:       [Binary data suppressed]\n";
                logEntry += new string('-', 40) + "\n";
            }
            else
            {
                logEntry +=
                    $"   Lines:         {details.LineCount}\n" +
                    $"   Max Line Len:  {details.MaxLineLength}\n\n";

                if (details.HeaderLines.Count > 0)
                {
                    logEntry += "   Headers:\n";
                    foreach (var h in details.HeaderLines)
                        logEntry += $"      {h}\n";
                    logEntry += "\n";
                }

                if (!string.IsNullOrWhiteSpace(details.Body))
                {
                    logEntry += "   Body:\n";
                    logEntry += details.Body + "\n\n";
                }
                logEntry += new string('-', 40) + "\n";
            }

            // UI update
            Invoke(() =>
            {
                richTextBox1.AppendText(logEntry);
                richTextBox1.ScrollToCaret();

                if (!lstIPs.Items.Contains(details.RemoteIp))
                    lstIPs.Items.Add(details.RemoteIp);
            });

            // File logging
            try { File.AppendAllText(_currentLogPath, logEntry); }
            catch { }
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
