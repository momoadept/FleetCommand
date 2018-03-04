using System;
using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.ServiceProvider;

namespace IngameScript.Core.Logging
{
    public interface ILoggingHub
    {
        void RegisterLog(ILog log);
    }

    class LcdLoggingHub : ILoggingHub, IComponent, IService, IStatusReporter
    {
        public string ComponentId { get; } = "LoggingHub";
        public Type[] Provides { get; } = {typeof(ILoggingHub)};
        public string StatusEntityId { get; } = "Master Log";
        public int RefreshStatusDelay { get; } = 10;
        public int DisplayedEntriesCount { get; set; } = 20;

        protected List<ILog> Logs { get; } = new List<ILog>();

        public void RegisterLog(ILog log)
        {
            Logs.Add(log);
        }

        
        public void OnAttached(App app)
        {
            
        }

        
        public string GetStatus()
        {
            if (!App.GlobalConfiguration.EnableMasterLog)
            {
                return "";
            }

            var combinedLog = CreateCombinedLog();

            return string.Join(
                "\n",
                combinedLog.Select(
                    entry =>
                    $"({entry.Entity}) {entry.RealTime.ToShortTimeString()} [{entry.Type}]: {entry.Text}"));
        }

        private List<LogEntry> CreateCombinedLog()
        {
            var allEntries = Logs.SelectMany(log => log.LogEntries);

            return allEntries
                .OrderByDescending(entry => entry.Time)
                .Take(DisplayedEntriesCount)
                .OrderBy(entry => entry.Time)
                .ToList();
        }
    }
}
