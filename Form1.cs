using System.Net;
using System.Net.Sockets;
using System.Text;

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
            _settings = ConfigManager.Load();
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
                File.WriteAllText(_currentLogPath, $"--- Monitor Started on Port {txtPort.Text} at {DateTime.Now} ---{Environment.NewLine}");

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
                    // Wait for a connection
                    using var client = await _listener!.AcceptTcpClientAsync(token);
                    var remoteIp = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";

                    // Capture a bit of data (the "commands" or headers)
                    byte[] buffer = new byte[1024];
                    var stream = client.GetStream();
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // 1. Prepare the strings on the background thread
                    string tool = data.Split('\r', '\n')[0];
                    string logEntry = $"[{DateTime.Now:HH:mm:ss}] Connection from {remoteIp}\n";
                    logEntry += $"   Data: {tool}\n";
                    logEntry += new string('-', 40) + "\n";

                    // 2. Update the UI (Thread-safe)
                    Invoke(() =>
                    {
                        richTextBox1.AppendText(logEntry);
                        richTextBox1.ScrollToCaret();
                        string ipOnly = remoteIp.Split(':')[0];

                        // Add to ListBox if it's not already there
                        if (!lstIPs.Items.Contains(ipOnly))
                        {
                            lstIPs.Items.Add(ipOnly);
                        }
                    });

                    // 3. Write to File (Still on background thread, no Invoke needed)
                    try
                    {
                        // Note: Use a regular AppendAllText or await the Async version here
                        await File.AppendAllTextAsync(_currentLogPath, logEntry);
                    }
                    catch { /* Log file error to lblStatus if needed */ }
                }
            }
            catch (OperationCanceledException) { /* Expected on stop */ }
            catch (Exception ex)
            {
                Invoke(() => lblStatus.Text = $"Listener Error: {ex.Message}");
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

            // Otherwise, do the actual cleanup we wrote before
            StopMonitoring();
            if (!string.IsNullOrEmpty(_currentLogPath))
            {
                File.AppendAllText(_currentLogPath, $"--- Monitor Stopped at {DateTime.Now} ---{Environment.NewLine}");
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            // 1. Create the settings window
            using (SettingsForm configWindow = new SettingsForm())
            {
                // 2. Show it as a popup (Dialog)
                if (configWindow.ShowDialog() == DialogResult.OK)
                {
                    // 3. If they clicked "Save", reload the settings in your main form
                    _settings = ConfigManager.Load();

                    // Optional: Show a status message
                    //lblStatus.Text = "Settings updated.";
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

        private void btnCopyIPs_Click(object sender, EventArgs e)
        {
            if (lstIPs.Items.Count > 0)
            {
                // Join all items into one string and set to clipboard
                string allIPs = string.Join(Environment.NewLine, lstIPs.Items.Cast<string>());
                Clipboard.SetText(allIPs);
                lblStatus.Text = $"{lstIPs.Items.Count} IPs copied to clipboard.";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _settings.CloseToTray = false;
            Application.Exit();
        }
    }
}
