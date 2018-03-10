using FC.Core.Core.Enums;

namespace FC.Core.Core
{
    public class AppConfig
    {
        // Logging
        public bool EnablePriorityLog { get; set; } = true;
        public bool EnableFlightRecorder { get; set; } = true;
        public LogType PriorityLogTypes { get; set; } = LogType.All;
        public LogType CommonLogTypes { get; set; } = LogType.All;
    }
}
