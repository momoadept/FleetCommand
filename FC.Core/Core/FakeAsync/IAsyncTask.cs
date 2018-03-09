using FC.Core.Core.Delegates;

namespace FC.Core.Core.FakeAsync
{
    public interface IAsyncTask
    {
        int Created { get; }

        int Delay { get; }

        bool IsCompleted { get; }

        event Event.Handler<IAsyncTask> Completed;

        void Tick(Async async);
    }
}