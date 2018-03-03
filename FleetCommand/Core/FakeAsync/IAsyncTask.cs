using IngameScript.Core.Interfaces;

namespace IngameScript.Core.FakeAsync
{
    public interface IAsyncTask : IWorker
    {
        int Created { get; }

        int Delay { get; }

        bool IsCompleted { get; }

        event Async.AsyncTaskCompleteHandler Completed;
    }
}