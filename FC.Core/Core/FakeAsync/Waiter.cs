using System;
using FC.Core.Core.Delegates;

namespace FC.Core.Core.FakeAsync
{
    public class Waiter: IAsyncTask
    {
        public int Created { get; private set; }
        public int Delay { get; private set; } = 0;
        public bool IsCompleted { get; private set; }

        public event Event.Handler<IAsyncTask> Completed;

        protected Func<bool> Condition { get; }
        protected int WaitingCheckDelay { get; }
        protected Time.Time Time { get; }

        public Waiter(Func<bool> condition, Time.Time time, int waitingCheckDelay = 1)
        {
            WaitingCheckDelay = waitingCheckDelay;
            Created = time.Now;
            Time = time;
            Condition = condition;
        }

        public void Tick(Async async)
        {
            if (Condition())
            {
                IsCompleted = true;
                Completed?.Invoke(this);
            }
            else
            {
                Delay = WaitingCheckDelay;
                Created = Time.Now;
            }
        }

    }
}
