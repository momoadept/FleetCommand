using System;

namespace IngameScript
{
    partial class Program
    {
        public interface IAsync
        {
            /// <summary>
            /// </summary>
            /// <param name="ms"></param>
            /// <param name="priority"></param>
            /// <returns>Promise that resolves in a given time with a time error in ms</returns>
            IPromise<int> Delay(int ms = 0, Priority priority = Priority.Routine);

            /// <summary>
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="priority"></param>
            /// <param name="timeout">0 if no timeout</param>
            /// <returns>Promise that resolves when a given condition is true or fails in a timeout. Result is a time elapsed</returns>
            IPromise<int> When(Func<bool> condition, Priority priority = Priority.Routine, int timeout = 0);

            IJob CreateJob(Action job, Priority priority = Priority.Routine);
        }
    }
}
