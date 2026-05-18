namespace PortMonitor.Models
{
    public class AppSettings
    {
        public bool CloseToTray { get; set; } = false;
        public bool MinimizeToTray { get; set; } = false;
        public bool OpenMinimized { get; set; } = false;
        public string LogFolderPath { get; set; } = "logs";
    }
}