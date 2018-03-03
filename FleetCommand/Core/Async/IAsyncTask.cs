using System;
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

        class SimpleAsyncTask : IAsyncTask
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

            public int Created { get; } = Time.Time.Now;
            public int Delay { get; }
            public bool IsCompleted { get; private set; } = false;
            public event Async.AsyncTaskCompleteHandler Completed;
        }
}
