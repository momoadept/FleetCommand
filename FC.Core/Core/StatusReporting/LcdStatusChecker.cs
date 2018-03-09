using System.Collections.Generic;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync;
using FC.Core.Core.Times;
using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core.StatusReporting
{
    public class LcdStatusChecker: IComponent
    {
        public string ComponentId { get; } = "StatusReporter";

        protected List<IStatusReporter> StatusReporters;
        protected Dictionary<IStatusReporter, StatusReportingStatus> StatusReportingData = new Dictionary<IStatusReporter, StatusReportingStatus>();
        protected SimpleAsyncWorker UpdateStatusesWorker;
        protected MyGridProgram Context;
        protected Async Async;
        protected Time Time;
        protected IBlockReferenceFactory BlockReferenceFactory;

        public LcdStatusChecker(List<IStatusReporter> statusReporters)
        {
            StatusReporters = statusReporters;
        }

        public void OnAttached(App app)
        {
            app.Bootstrapped += OnAppBootstrapped;
        }

        protected void OnAppBootstrapped(App app)
        {
            Context = app.ServiceProvider.Get<MyGridProgram>();
            Async = app.ServiceProvider.Get<Async>();
            BlockReferenceFactory = app.ServiceProvider.Get<IBlockReferenceFactory>();
            Time = app.Time;

            UpdateStatusesWorker = new SimpleAsyncWorker("UpdateLcdStatusWorker", CheckStatuses);
            Async.AddJob(UpdateStatusesWorker);
            UpdateStatusesWorker.Start();
        }

        protected void CheckStatuses()
        {
            foreach (var statusReporter in StatusReporters)
            {
                UpdateStatusReportingData(statusReporter);
                var statusReportingStatus = StatusReportingData[statusReporter];

                if (UpdateStatusNow(statusReporter, statusReportingStatus))
                {
                    var status = statusReporter.GetStatus();
                    var text = $@"
[{statusReporter.StatusEntityId}]
{status}";
                    statusReportingStatus.TargetLcd.ForEach(panel =>
                    {
                        panel.ShowPublicTextOnScreen();
                        panel.WritePublicText(text);
                    });
                    statusReportingStatus.LastReported = Time.Now;
                }
            }
        }

        protected void UpdateStatusReportingData(IStatusReporter reporter)
        {
            if (StatusReportingData.ContainsKey(reporter))
            {
                return;
            }

            StatusReportingData.Add(reporter, CreateStatusReportingStatus(reporter));
        }

        protected bool UpdateStatusNow(IStatusReporter reporter, StatusReportingStatus statusReportingStatus)
        {
            return Time.Now - statusReportingStatus.LastReported >= reporter.RefreshStatusDelay;
        }

        protected StatusReportingStatus CreateStatusReportingStatus(IStatusReporter reporter)
        {
            return new StatusReportingStatus(BlockReferenceFactory.GetReference<IMyTextPanel>($"S {reporter.StatusEntityId}"));
        }

        protected class StatusReportingStatus
        {
            public int LastReported { get; set; } = 0;
            public TagBlockReference<IMyTextPanel> TargetLcd { get; set; }
            public StatusReportingStatus(TagBlockReference<IMyTextPanel> targetLcd)
            {
                TargetLcd = targetLcd;
            }
        }
    }
}
