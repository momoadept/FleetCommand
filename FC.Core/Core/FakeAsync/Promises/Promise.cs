using System;
using FC.Core.Core.Delegates;

namespace FC.Core.Core.FakeAsync.Promises
{
    public class Promise
    {
        protected Action<Promise> Action;
        protected bool Started = false;

        public Promise(Action<Promise> startPromise)
        {
            Action = startPromise;
            Start();
        }

        protected void Start()
        {
            Action(this);
        }

        public bool IsCompleted { get; private set; }
        public event Event.Handler<Promise> Completed;
        public event Event.Handler<Exception> Rejected;

        public void Resolve()
        {
            IsCompleted = true;
            Completed?.Invoke(this);
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

        public Promise Then(Event.Handler<Promise> callback)
        {
            Completed += callback;

            if (IsCompleted)
            {
                callback(this);
            }

            return this;
        }

        public Promise Error(Event.Handler<Exception> callback)
        {
            Rejected += callback;

            return this;
        }

        public static Promise Resolve(Action action)
        {
            var result = new Promise(promise => action());
            result.Resolve();
            return result;
        }

        public static Promise FromError(Exception e)
        {
            var result = new Promise(promise => promise.Reject(e));
            return result;
        }

        public static Promise<T> FromValue<T>(T value)
        {
            var promise = new Promise<T>(p => p.Resolve(value));
            return promise;
        }
    }

    public class Promise<T>
    {
        protected Action<Promise<T>> Action;
        protected bool Started = false;
        protected T Result;

        public Promise(Action<Promise<T>> startPromise)
        {
            Action = startPromise;
            Start();
        }

        protected void Start()
        {
            Action(this);
        }

        public bool IsCompleted { get; private set; }
        public event Event.Handler<T> Completed;
        public event Event.Handler<Exception> Rejected;

        public void Resolve(T result)
        {
            Result = result;
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

            if (IsCompleted)
            {
                callback(Result);
            }

            return this;
        }

        public Promise<T> Error(Event.Handler<Exception> callback)
        {
            Rejected += callback;

            return this;
        }
    }
}
