using System.Net;
using System.Net.Sockets;
using PortMonitor.Models;
using PortMonitor.Parsing;

namespace PortMonitor.Services
{
    public class PortMonitorService
    {
        private TcpListener? _listener;
        private CancellationTokenSource? _cts;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        // Events the UI can subscribe to
        public event Action<ConnectionDetails>? ConnectionReceived;
        public event Action<string>? StatusChanged;
        public event Action? MonitoringStopped;

        public async Task StartAsync(int port)
        {
            if (_isRunning)
                return;

            try
            {
                _cts = new CancellationTokenSource();
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();

                _isRunning = true;
                StatusChanged?.Invoke($"Listening on port {port}");

                _ = Task.Run(() => ListenLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Error: {ex.Message}");
                await StopAsync();
            }
        }

        public async Task StopAsync()
        {
            if (!_isRunning)
                return;

            try
            {
                _isRunning = false;
                _cts?.Cancel();
                _listener?.Stop();
            }
            catch { }

            MonitoringStopped?.Invoke();
            await Task.CompletedTask;
        }

        private async Task ListenLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using var client = await _listener!.AcceptTcpClientAsync(token);
                    client.NoDelay = true;
                    var details = await ReadClientAsync(client, token);

                    // Parse raw bytes into structured fields
                    ConnectionParser.Parse(details);

                    // Notify UI or log service
                    ConnectionReceived?.Invoke(details);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Error: {ex.Message}");
            }
            finally
            {
                await StopAsync();
            }
        }

        private async Task<ConnectionDetails> ReadClientAsync(TcpClient client, CancellationToken token)
        {
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

            return details;
        }
    }
}