namespace IngameScript
{
    partial class Program
    {
        public static class LogHelper
        {
            public static string SeverityString(LogSeverity severity)
            {
                switch (severity)
                {
                    case LogSeverity.Debug:
                        return "DBG";
                    case LogSeverity.Info:
                        return "INF";
                    case LogSeverity.Warning:
                        return "WRN";
                    case LogSeverity.Error:
                        return "ERR";
                    case LogSeverity.Fatal:
                        return "FTL";
                    default:
                        return "WTF";
                }
            }
        }
    }
}
