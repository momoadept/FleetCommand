﻿using System.Collections.Generic;
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

            TResult _result;
            Exception _error;

            List<Action<TResult>> _thens = new List<Action<TResult>>();
            List<Action<Exception>> _catches = new List<Action<Exception>>();
            List<Action> _finallies = new List<Action>();

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

            public IPromise<TNewResult> Next<TNewResult>(Func<TResult, IPromise<TNewResult>> nextPromiseGenerator)
            {
                var nextPromise = new Promise<TNewResult>();

                _thens.Add(r =>
                    nextPromiseGenerator(r)
                        .Then(result => nextPromise.Resolve(result))
                        .Catch(e => nextPromise.Fail(e))
                );

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

            void Complete()
            {
                Completed = true;
                foreach (var @finally in _finallies)
                    @finally();

                _thens.Clear();
                _catches.Clear();
                _finallies.Clear();
            }

            public static IPromise<TValue> FromValue<TValue>(TValue result)
            {
                var promise = new Promise<TValue>();
                promise.Resolve(result);
                return promise;
            }

            public static IPromise<TResult> FromError(Exception e)
            {
                var promise = new Promise<TResult>();
                promise.Fail(e);
                return promise;
            }
        }
    }
}
