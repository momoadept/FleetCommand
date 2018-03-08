using System;
using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync
{
    internal class SimpleAsyncTask : IAsyncTask
    {
        protected Action Action;

        public SimpleAsyncTask(Action action, int created, int delay = 0)
        {
            Action = action;
            Delay = delay;
            Created = created;
        }

        public void Tick(Async async)
        {
            Action();

            IsCompleted = true;
            Completed?.Invoke(this);
        }

        public int Created { get; }
        public int Delay { get; }
        public bool IsCompleted { get; private set; }
        public event Event.Handler<IAsyncTask> Completed;
    }
}