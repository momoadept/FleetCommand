using IngameScript.Core.Interfaces;

namespace IngameScript.Core.Async
{
    public interface IAsyncTask : IWorker
    {
        int Created { get; }

        int Delay { get; }

        bool IsCompleted { get; }

        event Async.AsyncTaskCompleteHandler Completed;
    }
}