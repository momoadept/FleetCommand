using IngameScript.Core.Interfaces;

namespace IngameScript.Core.Async
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