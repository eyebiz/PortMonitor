using System.Data;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace PortMonitor
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
            pboxAboutLogo.Image = Properties.Resources.AppIcon.ToBitmap();
            LoadTechnicalInfo();
        }

        private void LoadTechnicalInfo()
        {
            var sb = new StringBuilder();

            void Section(string title)
            {
                sb.AppendLine($"=== {title} ===");
            }

            // 1. Core Application Metadata
            Section("Application");
            sb.AppendLine($"Product: {Application.ProductName}");
            sb.AppendLine($"Version: {Application.ProductVersion}");

            if (Environment.ProcessPath is string appPath)
            {
                DateTime buildDate = File.GetLastWriteTime(appPath);
                sb.AppendLine($"Build Date: {buildDate:yyyy-MM-dd HH:mm:ss}");
            }
            sb.AppendLine();

            // 2. Runtime & Environment
            Section("Runtime");
            sb.AppendLine($"Framework: {RuntimeInformation.FrameworkDescription}");
            sb.AppendLine($"CLR Version: {Environment.Version}");
            sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Architecture: {RuntimeInformation.ProcessArchitecture}");
            sb.AppendLine($"CPU Cores: {Environment.ProcessorCount}");
            sb.AppendLine($"GC Mode: {(GCSettings.IsServerGC ? "Server" : "Workstation")}");
            sb.AppendLine($"GC Latency: {GCSettings.LatencyMode}");
            sb.AppendLine();

            // 3. Process Info
            Section("Process");
            var p = Process.GetCurrentProcess();
            sb.AppendLine($"PID: {p.Id}");
            sb.AppendLine($"Memory (Working Set): {p.WorkingSet64 / 1024 / 1024} MB");
            sb.AppendLine($"Threads: {p.Threads.Count}");
            sb.AppendLine($"Handles: {p.HandleCount}");
            sb.AppendLine($"Start Time: {p.StartTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // 4. System Uptime
            Section("System Uptime");
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            sb.AppendLine($"System Uptime: {uptime:dd\\:hh\\:mm\\:ss}");
            sb.AppendLine();

            // 5. Assemblies
            Section("Loaded Assemblies");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.GetName().Name);

            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName();
                sb.AppendLine($"{name.Name} {name.Version}");
            }
            rtbAboutInfo.Text = sb.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/eyebiz/PortMonitor",
                UseShellExecute = true
            });
        }
    }
}
