using PortMonitor.Models;
using System.Text;

namespace PortMonitor.Services
{
    public class LogService
    {
        private readonly string _logDirectory;
        private string _currentLogPath = "";
        private readonly string _nl = Environment.NewLine;

        public string CurrentLogPath => _currentLogPath;

        public LogService(string? baseDirectory = null)
        {
            _logDirectory = Path.Combine(baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory, "logs");

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        public string StartNewLog(int port)
        {
            string fileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
            _currentLogPath = Path.Combine(_logDirectory, fileName);
            string entry = $"--- Monitoring started on port {port} at {DateTime.Now} ---{_nl}";
            AppendRaw(entry);
            return entry;
        }

        public string StopLog()
        {
            if (string.IsNullOrEmpty(_currentLogPath))
                return "";

            string entry = $"--- Monitoring stopped at {DateTime.Now} ---{_nl}";
            AppendRaw(entry);
            return entry;
        }

        public void AppendConnection(ConnectionDetails d)
        {
            string entry = FormatConnection(d);
            AppendRaw(entry);
        }

        public string FormatConnection(ConnectionDetails d)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] Connection from {d.RemoteIp}:{d.RemotePort} → {d.LocalPort}");
            sb.AppendLine($"   Connected:     {d.ConnectedAt:HH:mm:ss.fff}");
            sb.AppendLine($"   Disconnected:  {d.DisconnectedAt:HH:mm:ss.fff}");
            sb.AppendLine($"   Duration:      {d.Duration.TotalMilliseconds:F0} ms");
            sb.AppendLine($"   Timed Out:     {(d.TimedOut ? "Yes" : "No")}");
            sb.AppendLine($"   Client Closed: {(d.ClientClosed ? "Yes" : "No")}");
            sb.AppendLine();
            sb.AppendLine($"   Bytes Read:    {d.BytesRead}");
            //sb.AppendLine($"   First Bytes:   {d.FirstBytesHex}");
            sb.AppendLine($"   ASCII:         {d.AsciiPercentage:F1}%   Printable: {d.PrintablePercentage:F1}%");
            sb.AppendLine($"   Entropy:       {d.Entropy:F2}");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(d.FirstLineSanitized))
                sb.AppendLine($"   First Line:    {d.FirstLineSanitized}");

            if (d.PrintablePercentage < 80.0)
            {
                sb.AppendLine($"   Payload:       [Binary data suppressed]");
            }
            else
            {
                sb.AppendLine($"   Lines:         {d.LineCount}");
                sb.AppendLine($"   Max Line Len:  {d.MaxLineLength}");
                sb.AppendLine();

                if (d.HeaderLines.Count > 0)
                {
                    sb.AppendLine("   Headers:");
                    foreach (var h in d.HeaderLines)
                        sb.AppendLine($"      {h}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(d.Body))
                {
                    sb.AppendLine("   Body:");
                    sb.AppendLine(d.Body);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(new string('-', 60));
            sb.AppendLine();

            return sb.ToString();
        }

        private void AppendRaw(string text)
        {
            try
            {
                File.AppendAllText(_currentLogPath, text);
            }
            catch
            {
                // swallow logging errors — never crash the app
            }
        }
    }
}