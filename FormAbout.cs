using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

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
            lstAboutInfo.Items.Clear();

            // 1. Core Application Metadata
            lstAboutInfo.Items.Add($"Product: {Application.ProductName}");
            lstAboutInfo.Items.Add($"Version: {Application.ProductVersion}");
            if (Environment.ProcessPath is string appPath)
            {
                DateTime buildDate = File.GetLastWriteTime(appPath);
                lstAboutInfo.Items.Add($"Build Date: {buildDate:yyyy-MM-dd HH:mm:ss}");
            }
            lstAboutInfo.Items.Add(new string('-', 40));

            // 2. .NET Runtime & Environment Details
            lstAboutInfo.Items.Add($"Framework: {RuntimeInformation.FrameworkDescription}");
            lstAboutInfo.Items.Add($"CLR Version: {Environment.Version}");
            lstAboutInfo.Items.Add($"OS: {RuntimeInformation.OSDescription}");
            lstAboutInfo.Items.Add($"Architecture: {RuntimeInformation.ProcessArchitecture}");
            lstAboutInfo.Items.Add($"CPU Cores: {Environment.ProcessorCount}");
            lstAboutInfo.Items.Add($"GC Mode: {(GCSettings.IsServerGC ? "Server" : "Workstation")}");
            lstAboutInfo.Items.Add($"GC Latency: {GCSettings.LatencyMode}");
            lstAboutInfo.Items.Add(new string('-', 40));

            // 3. Process Information
            var p = Process.GetCurrentProcess();
            lstAboutInfo.Items.Add($"Process ID: {p.Id}");
            lstAboutInfo.Items.Add($"Memory (Working Set): {p.WorkingSet64 / 1024 / 1024} MB");
            lstAboutInfo.Items.Add($"Threads: {p.Threads.Count}");
            lstAboutInfo.Items.Add($"Handles: {p.HandleCount}");
            lstAboutInfo.Items.Add($"Start Time: {p.StartTime:yyyy-MM-dd HH:mm:ss}");
            lstAboutInfo.Items.Add(new string('-', 40));

            // 4. System Uptime
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            lstAboutInfo.Items.Add($"System Uptime: {uptime:dd\\:hh\\:mm\\:ss}");
            lstAboutInfo.Items.Add(new string('-', 40));

            // 5. Loaded Assemblies
            lstAboutInfo.Items.Add("Loaded Assemblies:");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.GetName().Name);

            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName();
                lstAboutInfo.Items.Add($" - {name.Name} {name.Version}");
            }
        }
    }
}
