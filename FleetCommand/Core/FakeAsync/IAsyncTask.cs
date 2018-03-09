using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync
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