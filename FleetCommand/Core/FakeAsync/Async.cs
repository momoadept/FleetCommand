using System;
using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.Interfaces;
using IngameScript.Core.ServiceProvider;

namespace IngameScript.Core.FakeAsync
{
    public partial class Async : IStatusReporter, ILogProvider, IWorker, IComponent, IService
    {
        public delegate void AsyncTaskCompleteHandler(IAsyncTask task);

        protected int CompletedTasksCount;

        public Async()
        {
        }

        protected ILog Log { get; private set; }

        protected List<IAsyncTask> Defered { get; } = new List<IAsyncTask>();
        protected List<IAsyncJob> Jobs { get; } = new List<IAsyncJob>();

        public string ComponentId { get; } = "Async";
        public void OnAttached(App app)
        {
            Log = app.ServiceProvider.Get<ILog>();
        }

        public string LogEntityId { get; } = "Async";
        public Type[] Provides { get; } = {typeof(Async)};
        public string StatusEntityId { get; } = "Async";
        public int RefreshStatusDelay { get; } = 100;


        public string GetStatus()
        {
            var status = $@"
Async tasks completed last {RefreshStatusDelay} ticks: {CompletedTasksCount}
Active jobs: 
{GetJobsReport(true)}
Inactive Jobs:
{GetJobsReport(false)}
";
            CompletedTasksCount = 0;
            return status;
        }

        public void Tick()
        {
            RunTasks();
            RunJobs();
        }

        public void AddJob(IAsyncJob job)
        {
            Jobs.Add(job);
        }

        public void RemoveJob(IAsyncJob job)
        {
            Jobs.Remove(job);
        }

        public AsyncFluentScheduler Do(Action task)
        {
            return Do(new SimpleAsyncTask(task));
        }

        public AsyncFluentScheduler Do(IAsyncTask task)
        {
            return new AsyncFluentScheduler(task, this);
        }

        protected void RunJobs()
        {
            foreach (var job in Jobs)
            {
                if (RunJobNow(job))
                {
                    job.Tick();
                    job.LastRan = App.Time.Now;
                }
            }
        }

        protected void RunTasks()
        {
            if (!Defered.Any()) return;

            List<IAsyncTask> doing;
            do
            {
                doing = GetTasksToDoNow();
                doing.ForEach(RunTask);
            } while (doing.Any());
        }

        protected List<IAsyncTask> GetTasksToDoNow()
        {
            return Defered.Where(d => App.Time.Now - d.Created >= d.Delay).ToList();
        }

        protected bool RunJobNow(IAsyncJob job)
        {
            return job.IsRunning && (job.LastRan == 0 || App.Time.Now - job.LastRan >= job.DelayBetweenRuns);
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

        protected string GetJobsReport(bool active)
        {
            return string.Join("\n", Jobs.Where(j => j.IsRunning == active).Select(j => $"{j.AsyncJobId} every {j.DelayBetweenRuns} ticks"));
        }
    }
}