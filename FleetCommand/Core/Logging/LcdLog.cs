using System;
using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.BlockReferences;
using IngameScript.Core.Delegates;
using IngameScript.Core.Enums;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.Logging
{
    public class LcdLog : ILog
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public int DisplayedEntriesCount { get; set; } = 10;

        public event Event.Handler<LogEntry> EntryAdded;

        protected string Entity { get; }
        protected TagBlockReference<IMyTextPanel> LcdReference { get; }
        protected Time.Time Time;

        public LcdLog(string entity, ILoggingHub hub, IBlockReferenceFactory references, Time.Time time)
        {
            hub.RegisterLog(this);
            Entity = entity;
            Time = time;
            LcdReference = references.GetReference<IMyTextPanel>($"Log {Entity}");
        }

        public void Log(string text, LogType logType = LogType.Info)
        {
            var entry = new LogEntry(logType, text, Entity, Time.Now);
            LogEntries.Add(entry);
            EntryAdded?.Invoke(entry);

            foreach (var lcdReferenceAccessor in LcdReference.Accessors)
            {
                WriteToLcd(lcdReferenceAccessor);
            }
        }

        public void Debug(string text) => Log(text, LogType.Debug);

        public void Info(string text) => Log(text, LogType.Info);

        public void Warning(string text) => Log(text, LogType.Warning);

        public void Error(string text) => Log(text, LogType.Error);
        public void Priority(string text) => Log(text, LogType.Priority);

        protected void WriteToLcd(BlockAccessor<IMyTextPanel> lcd)
        {
            lcd.Block.ShowPublicTextOnScreen();
            var entries = FilterByLogTypes(lcd.Arguments);

            var format =
                Entity + "\n"
                       + string.Join(
                           "\n",
                           Enumerable.Take<LogEntry>(entries
                                   .OrderByDescending(entry => entry.Time), DisplayedEntriesCount)
                               .OrderBy(entry => entry.Time)
                               .Select(entry =>
                                   $"{entry.Time} [{entry.Type}]: {entry.Text}"));

            lcd.Block.WritePublicText(format);
        }

        protected List<LogEntry> FilterByLogTypes(List<string> logTypes)
        {
            var acceptedTypes = LogType.All;

            if (logTypes.Count > 0)
            {
                acceptedTypes = LogType.None;
                foreach (var logType in logTypes)
                {
                    acceptedTypes |= ParseLogType(logType);
                }
            }

            return Enumerable.ToList<LogEntry>(LogEntries.Where(entry => (entry.Type & acceptedTypes) != 0));
        }

        protected LogType ParseLogType(string str)
        {
            switch (str.Trim())
            {
                case "Info":
                    return LogType.Info;
                case "Debug":
                    return LogType.Debug;
                case "Warning":
                    return LogType.Warning;
                case "Error":
                    return LogType.Error;
                case "Priority":
                    return LogType.Priority;
            }

            throw new Exception($"Unknown log type: {str}");
        }
    }
}