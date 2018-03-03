using System;
using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.Interfaces;
using IngameScript.Core.ServiceProvider;

namespace IngameScript.Core.Async
{
    public partial class Async : IStatusReporter, ILogProvider, IWorker, IComponent, IService
    {
        public delegate void AsyncTaskCompleteHandler(IAsyncTask task);

        public string ComponentId { get; } = "Async";
        public string StatusEntityId { get; } = "Async";
        public int RefreshStatusDelay { get; } = 100;
        public string LogEntityId { get; } = "Async";
        public Type[] Provides { get; } = {typeof(Async)};
        public ILog Log { get; }

        protected List<IAsyncTask> Defered { get; } = new List<IAsyncTask>();

        protected int CompletedTasksCount = 0;

        public Async(IMyServiceProvider services)
        {
            Log = services.Get<ILog>();
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
            var status = $@"
Async tasks completed last {RefreshStatusDelay} ticks: {CompletedTasksCount}

";
            CompletedTasksCount = 0;
            return status;
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
            return Defered.Where(d => Time.Time.Now - d.Created >= d.Delay).ToList();
        }

        protected void RunTask(IAsyncTask task)
        {
            task.Tick();

            if (task.IsCompleted)
            {
                Defered.Remove(task);
                CompletedTasksCount++;
            }
        }
    }
}
