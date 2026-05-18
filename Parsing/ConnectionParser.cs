using System.Text;
using System.Text.RegularExpressions;
using PortMonitor.Models;

namespace PortMonitor.Parsing
{
    public static class ConnectionParser
    {
        private static readonly string _nl = Environment.NewLine;

        public static void Parse(ConnectionDetails details)
        {
            var raw = details.RawBytes ?? Array.Empty<byte>();

            // --- First bytes (hex + ascii) ---
            var firstBytes = raw.Take(32).ToArray();
            details.FirstBytesHex = BitConverter.ToString(firstBytes);
            details.FirstBytesAscii = new string(firstBytes.Select(b =>
                b >= 0x20 && b <= 0x7E ? (char)b : '.').ToArray());

            // --- Convert to text ---
            string text = raw.Length > 0
                ? Encoding.UTF8.GetString(raw)
                : "";

            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            details.FirstLineRaw = lines.Length > 0 ? lines[0] : "";
            details.FirstLineSanitized = Regex.Replace(details.FirstLineRaw, @"[^\x20-\x7E]", ".");

            // --- Header extraction ---
            details.HeaderLines = new List<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                    break;

                if (line.Contains(':'))
                    details.HeaderLines.Add(line);
            }

            // --- Body extraction ---
            int blankIndex = Array.FindIndex(lines, l => string.IsNullOrWhiteSpace(l));
            if (details.HeaderLines.Count > 0 && blankIndex >= 0 && blankIndex < lines.Length - 1)
                details.Body = string.Join(_nl, lines.Skip(blankIndex + 1));

            // --- Line stats ---
            details.LineCount = lines.Length;
            details.MaxLineLength = lines.Any() ? lines.Max(l => l.Length) : 0;

            // --- ASCII / printable / entropy ---
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
    }
}