using System;

namespace IngameScript.Core.Async
{
    internal class SimpleAsyncTask : IAsyncTask
    {
        private readonly Action action;

        public SimpleAsyncTask(Action action, int delay = 0)
        {
            this.action = action;
            Delay = delay;
        }

        public void Tick()
        {
            action();

            IsCompleted = true;
            Completed?.Invoke(this);
        }

        public int Created { get; } = Time.Now;
        public int Delay { get; }
        public bool IsCompleted { get; private set; }
        public event Async.AsyncTaskCompleteHandler Completed;
    }
}