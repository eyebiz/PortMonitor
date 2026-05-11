using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PortMonitor
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            LoadTechnicalInfo();
        }

        private void LoadTechnicalInfo()
        {
            lstAboutInfo.Items.Clear();

            // 1. Core Application Metadata
            lstAboutInfo.Items.Add($"Product: {Application.ProductName}");
            lstAboutInfo.Items.Add($"Version: {Application.ProductVersion}");
            // Use Process.GetCurrentProcess().MainModule.FileName instead of Assembly.Location
            string appPath = Environment.ProcessPath ?? string.Empty;
            DateTime buildDate = File.GetLastWriteTime(appPath);
            lstAboutInfo.Items.Add($"Build Date: {buildDate:yyyy-MM-dd HH:mm:ss}");
            lstAboutInfo.Items.Add(new string('-', 30));

            // 2. .NET Runtime & Environment Details
            lstAboutInfo.Items.Add($"Framework: {RuntimeInformation.FrameworkDescription}");
            lstAboutInfo.Items.Add($"OS: {RuntimeInformation.OSDescription}");
            lstAboutInfo.Items.Add($"Architecture: {RuntimeInformation.ProcessArchitecture}");
            lstAboutInfo.Items.Add(new string('-', 30));

            // 3. Enumerating Loaded Components (Assemblies)
            lstAboutInfo.Items.Add("Loaded Modules:");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                .OrderBy(a => a.GetName().Name);

            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName();
                // Filter out some internal noise if desired, or show everything
                lstAboutInfo.Items.Add($" - {name.Name}: {name.Version}");
            }
        }
    }
}
