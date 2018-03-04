using IngameScript.Core.ComponentModel;
using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync
{
    public interface IAsyncTask : IWorker
    {
        int Created { get; }

        int Delay { get; }

        bool IsCompleted { get; }

        event Event.Handler<IAsyncTask> Completed;
    }
}