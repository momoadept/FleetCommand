using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync.Promises
{
    public class Promise
    {
        private readonly Async async;
        private readonly IAsyncTask task;

        public Promise(IAsyncTask task, Async async)
        {
            this.task = task;
            this.async = async;
        }

        public Promise Then(Event.Handler<IAsyncTask> callback)
        {
            task.Completed += callback;

            return this;
        }

        public Promise Then(IAsyncTask nextTask)
        {
            task.Completed += t => { async.Do(nextTask); };

            return new Promise(nextTask, async);
        }

        public IAsyncTask Pick() => task;
    }

    public class Promise<T>
    {
        private readonly Async async;
        protected Action<Promise<T>> Action;
        protected bool Started = false;

        public Promise(Action<Promise<T>> startPromise, int created)
        {
            Action = startPromise;
            Created = created;
        }

        public void Start()
        {
            Action(this);
        }

        public int Created { get; }
        public bool IsCompleted { get; private set; }
        public event Event.Handler<T> Completed;
        public event Event.Handler<Exception> Rejected;

        public void Resolve(T result)
        {
            IsCompleted = true;
            Completed?.Invoke(result);
        }

        public void Reject(Exception e)
        {
            IsCompleted = true;

            if (Rejected == null)
            {
                throw e;
            }

            Rejected.Invoke(e);
        }

        public Promise<T> Then(Event.Handler<T> callback)
        {
            Completed += callback;

            return this;
        }
    }
}
