using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    partial class Program
    {
        public class Async : IStatusProvider, ILogProvider, IWorker
        {
            public delegate void AsyncTaskCompleteHandler(IAsyncTask task);

            public string StatusEntityId { get; } = "Async";
            public string LogEntityId { get; } = "Async";
            public ILog Log { get; }

            protected List<IAsyncTask> Defered { get; } = new List<IAsyncTask>();

            public Async(IMyServiceProvider services)
            {
                Log = services.Get<ILog>();

                services.Get<App>().Workers.Add(this);
            }

            public void Tick()
            {
                if (!Defered.Any())
                {
                    return;
                }

                List<IAsyncTask> doing;
                do
                {
                    doing = GetTasksToDoNow();
                    doing.ForEach(RunTask);

                } while (doing.Any());
            }

            public string GetStatus()
            {
                return "OK";
            }

            public AsyncFluentScheduler Do(Action task)
            {
                return Do(new SimpleAsyncTask(task));
            }

            public AsyncFluentScheduler Do(IAsyncTask task)
            {
                return new AsyncFluentScheduler(task, this);
            }

            protected List<IAsyncTask> GetTasksToDoNow()
            {
                return Defered.Where(d => Time.Now - d.Created >= d.Delay).ToList();
            }

            protected void RunTask(IAsyncTask task)
            {
                task.Tick();

                if (task.IsCompleted)
                {
                    Defered.Remove(task);
                }
            }

            public class AsyncFluentScheduler
            {
                private readonly IAsyncTask task;
                private readonly Async async;

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

                    task.Completed += t =>
                    {
                        async.Do(nextTask);
                    };

                    return new AsyncFluentScheduler(nextTask, async);
                }
            }
        }
    }
}
