using System;
using System.Collections.Generic;
using System.Text;

namespace PortMonitor
{
    public class ConnectionDetails
    {
        // Connection endpoints
        public string RemoteIp { get; set; } = "";
        public int RemotePort { get; set; }
        public int LocalPort { get; set; }

        // Timing
        public DateTime ConnectedAt { get; set; }
        public DateTime DisconnectedAt { get; set; }
        public TimeSpan Duration => DisconnectedAt - ConnectedAt;

        public bool TimedOut { get; set; }
        public bool ClientClosed { get; set; }

        public TimeSpan TimeToFirstByte { get; set; } = TimeSpan.Zero;

        // Raw data
        public int BytesRead { get; set; }
        public byte[] RawBytes { get; set; } = Array.Empty<byte>();

        public string FirstBytesHex { get; set; } = "";
        public string FirstBytesAscii { get; set; } = "";

        // Text extraction
        public string FirstLineRaw { get; set; } = "";
        public string FirstLineSanitized { get; set; } = "";

        public List<string> HeaderLines { get; set; } = new();
        public string Body { get; set; } = "";

        // Structure analysis
        public int LineCount { get; set; }
        public int MaxLineLength { get; set; }

        // Byte analysis
        public double AsciiPercentage { get; set; }
        public double PrintablePercentage { get; set; }
        public double Entropy { get; set; }
    }
}
