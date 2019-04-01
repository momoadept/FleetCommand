using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        public class Promise<TResult>: IResolvablePromise<TResult>
        {
            public bool Resolved { get; private set; }
            public bool Failed { get; private set; }
            public bool Completed { get; private set; }

            private TResult _result;
            private Exception _error;

            private List<Action<TResult>> _thens = new List<Action<TResult>>();
            private List<Action<Exception>> _catches = new List<Action<Exception>>();
            private List<Action> _finallies = new List<Action>();

            public IPromise<TResult> Then(Action<TResult> callback)
            {
                if (Resolved)
                    callback(_result);
                else
                    _thens.Add(callback);

                return this;
            }

            public IPromise<TResult> Catch(Action<Exception> handler)
            {
                if (Failed)
                    handler(_error);
                else
                    _catches.Add(handler);

                return this;
            }

            public IPromise<TResult> Finally(Action handler)
            {
                if (Completed)
                    handler();
                else
                    _finallies.Add(handler);

                return this;
            }

            public IPromise<TNewResult> Next<TNewResult>(Func<IPromise<TNewResult>> nextPromiseGenerator)
            {
                var nextPromise = new Promise<TNewResult>();

                _thens.Add(r =>
                {
                    nextPromiseGenerator()
                        .Then(result => nextPromise.Resolve(result))
                        .Catch(e => nextPromise.Fail(e));
                });

                _catches.Add(e => nextPromise.Fail(e));

                return nextPromise;
            }

            public void Resolve(TResult result)
            {
                Resolved = true;
                _result = result;
                foreach (var action in _thens)
                    action(result);

                Complete();
            }

            public void Fail(Exception error)
            {
                Failed = true;
                _error = error;
                foreach (var @catch in _catches)
                    @catch(error);

                Complete();
            }

            private void Complete()
            {
                Completed = true;
                foreach (var @finally in _finallies)
                    @finally();

                _thens.Clear();
                _catches.Clear();
                _finallies.Clear();
            }
        }
    }
}
