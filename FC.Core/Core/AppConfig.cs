namespace IngameScript.Core
{
    public class AppConfig
    {
        // Logging
        public bool EnableMasterLog { get; set; } = false;
        public LogType MasterLogTypes { get; set; } = LogType.All;
        public LogType CommonLogTypes { get; set; } = LogType.All;
    }
}
