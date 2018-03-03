using System;

namespace IngameScript.Core.Async
{
    public class SimpleAsyncWorker : IAsyncJob
    {
        public SimpleAsyncWorker(string asyncJobId, Action action, int delayBetweenRuns = 1)
        {
            Action = action;
            DelayBetweenRuns = delayBetweenRuns;
            AsyncJobId = asyncJobId;
        }

        protected Action Action { get; }

        public void Tick()
        {
            Action();
        }

        public string AsyncJobId { get; }
        public bool IsRunning { get; private set; }
        public int LastRan { get; set; } = 0;
        public int DelayBetweenRuns { get; }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}