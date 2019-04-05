using System;

namespace IngameScript
{
    partial class Program
    {
        public interface IPromise
        {

        }

        public interface IPromise<out TResult>: IPromise
        {
            IPromise<TResult> Then(Action<TResult> callback);

            IPromise<TResult> Catch(Action<Exception> handler);

            IPromise<TResult> Finally(Action handler);

            /// <summary>
            /// Pass a promise gerenator. It will be launched when current promise resolves or failed if current fails.
            /// </summary>
            /// <returns>
            /// Returns a new promise
            /// </returns>
            IPromise<TNewResult> Next<TNewResult>(Func<IPromise<TNewResult>> nextPromiseGenerator);
        }
    }
}
