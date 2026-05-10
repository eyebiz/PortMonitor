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

                    // Update UI safely
                    Invoke(() => {
                        string logEntry = $"[{DateTime.Now:HH:mm:ss}] Connection from {remoteIp}\n";
                        if (!string.IsNullOrWhiteSpace(data))
                            logEntry += $"   Data: {data.Replace("\n", " ").Replace("\r", "")}\n";

                        richTextBox1.AppendText(logEntry);
                        richTextBox1.ScrollToCaret();
                    });

                    // Optional: Append to file here
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
    }
}
