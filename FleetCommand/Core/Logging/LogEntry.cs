using System;
using IngameScript.Core.Enums;

namespace IngameScript.Core.Logging
{
    public class LogEntry
    {
        public string Text { get; }
        public int Time { get; }
        public DateTime RealTime { get; }
        public LogType Type { get; }
        public string Entity { get; }

        public LogEntry(LogType type, string text, string entity, int time)
        {
            Type = type;
            Text = text;
            Entity = entity;
            Time = time;
            RealTime = DateTime.Now;
        }
    }
}
