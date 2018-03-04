using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.BlockReferences.LCD;
using IngameScript.Core.Delegates;
using IngameScript.Core.Enums;

namespace IngameScript.Core.Logging
{
    public class LcdLog : ILog
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();
        public List<LogType> DisplayedLogTypes { get; set; } = App.GlobalConfiguration.CommonLogTypes;
        public int DisplayedEntriesCount { get; set; } = 10;

        public event Event.Handler<LogEntry> EntryAdded;

        protected string Entity { get; }
        protected LcdReference LcdReference { get; }

        public LcdLog(string entity)
        {
            App.ServiceProvider.Get<ILoggingHub>().RegisterLog(this);
            Entity = entity;
            LcdReference = new LcdReference(App.BlockTag($"Log {Entity}"));
        }

        public void Log(string text, LogType logType = LogType.Info)
        {
            var entry = new LogEntry(logType, text, Entity);
            LogEntries.Add(entry);
            EntryAdded?.Invoke(entry);

            if (DisplayedLogTypes.Contains(entry.Type))
            {
                UpdateLcd();
            }
        }

        protected void UpdateLcd()
        {
            // TODO: Optimize this method
            var format =
                Entity + "\n"
                       + string.Join(
                           "\n",
                           LogEntries
                               .Where(entry => DisplayedLogTypes.Contains(entry.Type))
                               .OrderByDescending(entry => entry.Time)
                               .Take(DisplayedEntriesCount)
                               .OrderBy(entry => entry.Time)
                               .Select(entry =>
                                   $"{entry.Time} [{entry.Type}]: {entry.Text}"));
            
            LcdReference.SetText(format);
        }
        
    }
}