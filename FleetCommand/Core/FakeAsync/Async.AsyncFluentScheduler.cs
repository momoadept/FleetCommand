using IngameScript.Core.Delegates;

namespace IngameScript.Core.FakeAsync
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

            public AsyncFluentScheduler Then(Event.Handler<IAsyncTask> callback)
            {
                task.Completed += callback;

                return this;
            }

            public AsyncFluentScheduler Then(IAsyncTask nextTask)
            {
                task.Completed += t => { async.Do(nextTask); };

                return new AsyncFluentScheduler(nextTask, async);
            }

            public IAsyncTask Pick() => task;
        }
    }
}