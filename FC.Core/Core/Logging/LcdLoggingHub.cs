﻿using System;
using System.Collections.Generic;
using System.Linq;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core.Logging
{
    public class LcdLoggingHub : ILoggingHub, IComponent, IService, IStatusReporter
    {
        public string ComponentId { get; } = "LoggingHub";
        public Type[] Provides { get; } = {typeof(ILoggingHub)};
        public string StatusEntityId { get; } = "Priority Messages";
        public int RefreshStatusDelay { get; } = 10;
        public int DisplayedEntriesCount { get; set; } = 20;

        protected List<ILog> Logs { get; } = new List<ILog>();
        protected TagBlockReference<IMyTerminalBlock> FlightRecorder;

        public void RegisterLog(ILog log)
        {
            Logs.Add(log);
            log.EntryAdded += PushToFlightRecorder;
        }

        private void PushToFlightRecorder(LogEntry entry)
        {
            if (App.GlobalConfiguration.EnableFlightRecorder)
            {
                FlightRecorder.ForEach(block => block.CustomData += FormatEntry(entry) + "\n");
            }
        }

        public void OnAttached(App app)
        {
            app.Bootstrapped += a =>
            {
                FlightRecorder = a.ServiceProvider.Get<IBlockReferenceFactory>()
                    .GetReference<IMyTerminalBlock>("FlightRec");
            };
        }

        public string GetStatus()
        {
            if (!App.GlobalConfiguration.EnablePriorityLog)
            {
                return "";
            }

            var combinedLog = CreateCombinedLog();

            return string.Join(
                "\n",
                combinedLog.Select(FormatEntry));
        }

        private string FormatEntry(LogEntry entry)
        {
            return $"{entry.Entity} {entry.RealTime.ToShortTimeString()} [{entry.Type}]: {entry.Text}";
        }

        private List<LogEntry> CreateCombinedLog()
        {
            var allEntries = Logs
                .SelectMany(log => log.LogEntries)
                .Where(entry => (entry.Type & App.GlobalConfiguration.PriorityLogTypes)!=0);

            return allEntries
                .OrderByDescending(entry => entry.Time)
                .Take(DisplayedEntriesCount)
                .OrderBy(entry => entry.Time).ToList();
        }
    }
}