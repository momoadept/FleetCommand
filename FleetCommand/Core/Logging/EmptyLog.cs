using System.Collections.Generic;
using IngameScript.Core.Delegates;
using IngameScript.Core.Enums;

namespace IngameScript.Core.Logging
{
    public class EmptyLog : ILog
    {
        public void Log(string text, LogType logType = LogType.Info)
        {
        }

        public List<LogType> DisplayedLogTypes { get; set; }
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();
        public event Event.Handler<LogEntry> EntryAdded;
    }
}