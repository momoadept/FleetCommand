using System.Collections.Generic;
using FC.Core.Core.Delegates;
using FC.Core.Core.Enums;

namespace FC.Core.Core.Logging
{
    public class EmptyLog : ILog
    {
        public void Log(string text, LogType logType = LogType.Info)
        {
        }

        public void Debug(string text)
        {
        }

        public void Info(string text)
        {
        }

        public void Warning(string text)
        {
        }

        public void Error(string text)
        {
        }

        public void Priority(string text)
        {
        }

        public List<LogType> DisplayedLogTypes { get; set; }
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();
        public event Event.Handler<LogEntry> EntryAdded;
    }
}