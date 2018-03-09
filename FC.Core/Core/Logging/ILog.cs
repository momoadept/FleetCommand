using System.Collections.Generic;
using FC.Core.Core.Delegates;
using FC.Core.Core.Enums;

namespace FC.Core.Core.Logging
{
    public interface ILog
    {
        void Log(string text, LogType logType = LogType.Info);
        void Debug(string text);
        void Info(string text);
        void Warning(string text);
        void Error(string text);
        void Priority(string text);
        List<LogEntry> LogEntries { get; }
        event Event.Handler<LogEntry> EntryAdded;
    }
}