using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Async;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.BlockReferences.LCD;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.Interfaces;

namespace IngameScript.Core.StatusReporting
{
    // ReSharper disable once InconsistentNaming
    public class LCDStatusChecker: IComponent
    {
        public string ComponentId { get; } = "StatusReporter";

        protected List<IStatusReporter> StatusReporters;
        protected Dictionary<IStatusReporter, StatusReportingStatus> StatusReportingData;
        protected SimpleAsyncWorker UpdateStatusesWorker;
        protected IBlockLoader BlockLoader;

        public LCDStatusChecker(List<IStatusReporter> statusReporters)
        {
            StatusReporters = statusReporters;
        }

        public void OnAttached(App app)
        {
            BlockLoader = app.ServiceProvider.Get<IBlockLoader>();

            UpdateStatusesWorker = new SimpleAsyncWorker("UpdateLcdStatusWorker", CheckStatuses);
            app.Async.AddJob(UpdateStatusesWorker);
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
                    statusReportingStatus.TargetLcd.SetText(text);
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
            return Time.Now - statusReportingStatus.LastReported <= reporter.RefreshStatusDelay;
        }

        protected StatusReportingStatus CreateStatusReportingStatus(IStatusReporter reporter)
        {
            return new StatusReportingStatus(new LcdReference(BlockLoader, reporter.StatusEntityId));
        }

        protected class StatusReportingStatus
        {
            public int LastReported { get; set; } = 0;
            public LcdReference TargetLcd { get; set; }
            public StatusReportingStatus(LcdReference targetLcd)
            {
                TargetLcd = targetLcd;
            }
        }
    }
}
