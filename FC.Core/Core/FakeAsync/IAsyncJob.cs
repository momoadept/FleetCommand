using FC.Core.Core.ComponentModel;

namespace FC.Core.Core.FakeAsync
{
    public interface IAsyncJob : IWorker
    {
        string AsyncJobId { get; }

        bool IsRunning { get; }

        int LastRan { get; set; }

        int DelayBetweenRuns { get; }

        void Start();

        void Stop();
    }
}