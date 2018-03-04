using System;
using System.Collections.Generic;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.Delegates;
using IngameScript.Core.Enums;

namespace IngameScript.Core.Logging
{
    public interface ILog
    {
        void Log(string text, LogType logType = LogType.Info);
        List<LogType> DisplayedLogTypes { get; set; }
        List<LogEntry> LogEntries { get; }
        event Event.Handler<LogEntry> EntryAdded;
    }
}