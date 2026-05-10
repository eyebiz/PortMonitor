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

        public Form1()
        {
            InitializeComponent();
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
            // 1. Stop the listener and background task
            StopMonitoring();

            // 2. Force a synchronous write to ensure it hits the disk before exit
            if (!string.IsNullOrEmpty(_currentLogPath))
            {
                try
                {
                    // Use File.AppendAllText (NOT Async) here
                    File.AppendAllText(_currentLogPath, $"--- Monitor Stopped at {DateTime.Now} ---{Environment.NewLine}");
                }
                catch { /* Final fail-safe */ }
            }
        }
    }
}
