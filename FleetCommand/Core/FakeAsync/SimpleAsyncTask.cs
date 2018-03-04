using System;
using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync
{
    internal class SimpleAsyncTask : IAsyncTask
    {
        private readonly Action action;

        public SimpleAsyncTask(Action action, int delay = 0)
        {
            this.action = action;
            Delay = delay;
            Created = App.Time.Now;
        }

        public void Tick()
        {
            action();

            IsCompleted = true;
            Completed?.Invoke(this);
        }

        public int Created { get; }
        public int Delay { get; }
        public bool IsCompleted { get; private set; }
        public event Event.Handler<IAsyncTask> Completed;
    }
}