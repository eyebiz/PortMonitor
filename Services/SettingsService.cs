using PortMonitor.Models;
using System.Text.Json;

namespace PortMonitor.Services
{
    public class SettingsService
    {
        private readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public AppSettings Load()
        {
            if (!File.Exists(ConfigPath))
            {
                // 1. Create defaults: both false, folder is "logs"
                var defaults = new AppSettings
                {
                    CloseToTray = false,
                    MinimizeToTray = false,
                    OpenMinimized = false,
                    LogFolderPath = "logs"
                };

                // 2. Save it immediately so the file exists alongside the EXE
                Save(defaults);
                return defaults;
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch { return new AppSettings(); }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save config: {ex.Message}");
            }
        }
    }
}