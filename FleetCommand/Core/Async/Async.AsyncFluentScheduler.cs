namespace IngameScript.Core.Async
{
    public partial class Async
    {
        public class AsyncFluentScheduler
        {
            private readonly Async async;
            private readonly IAsyncTask task;

            public AsyncFluentScheduler(IAsyncTask task, Async async)
            {
                this.task = task;
                this.async = async;
            }

            public AsyncFluentScheduler Then(AsyncTaskCompleteHandler callback)
            {
                task.Completed += callback;

                return this;
            }

            public AsyncFluentScheduler Then(IAsyncTask nextTask)
            {
                task.Completed += t => { async.Do(nextTask); };

                return new AsyncFluentScheduler(nextTask, async);
            }
        }
    }
}