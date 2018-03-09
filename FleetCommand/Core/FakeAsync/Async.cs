using System;
using System.Collections.Generic;
using System.Linq;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync.Promises;
using IngameScript.Core.Logging;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.FakeAsync
{
    public partial class Async : IComponent, IStatusReporter, IWorker, IService
    {
        protected int CompletedTasksCount;

        protected List<IAsyncTask> Defered { get; } = new List<IAsyncTask>();
        protected List<IAsyncJob> Jobs { get; } = new List<IAsyncJob>();
        protected Time Time;
        protected App App;
        public ILog Log { get; private set; }

        public Type[] Provides { get; } = {typeof(Async)};
        public string StatusEntityId { get; } = "Async";
        public int RefreshStatusDelay { get; } = 100;


        public string GetStatus()
        {
            var status = $@"
Async tasks completed last {RefreshStatusDelay} ticks: {CompletedTasksCount}

Active Jobs: 
{GetJobsReport(true)}

Inactive Jobs:
{GetJobsReport(false)}

Active waiters:
{Defered.Count(t => t is Waiter)}
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
            Log?.Info($"Async job added: {job.AsyncJobId} each {job.DelayBetweenRuns} ticks");
        }

        public void RemoveJob(IAsyncJob job)
        {
            Jobs.Remove(job);
        }

        public Promise Do(Action task)
        {
            return Do(new SimpleAsyncTask(task, Time.Now));
        }

        public Promise Do(IAsyncTask task)
        {
            Defered.Add(task);
            Log?.Debug($"Scheduled async task after {task.Delay} ticks");
            return new Promise(task, this);
        }

        public Promise When(Func<bool> condition, int waitingCheckDelay = 1)
        {
            var task = new Waiter(condition, Time, waitingCheckDelay);
            Defered.Add(task);
            Log?.Debug($"Started waiter");
            return new Promise(task, this);
        }

        public Promise WhenReady<TService>() where TService : class
        {
            return When(() => App.ServiceProvider.Get<TService>() != null);
        }

        protected void RunJobs()
        {
            foreach (var job in Jobs)
            {
                if (RunJobNow(job))
                {
                    job.Tick();
                    job.LastRan = App.Time.Now;
                    CompletedTasksCount++;
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
            task.Tick(this);

            if (task.IsCompleted)
            {
                Defered.Remove(task);
                CompletedTasksCount++;
                if (task is Waiter)
                {
                    Log?.Debug("Finished waiter");
                }
            }
        }

        protected string GetJobsReport(bool active)
        {
            return string.Join("\n", Jobs.Where(j => j.IsRunning == active).Select(j => $"{j.AsyncJobId}: every {j.DelayBetweenRuns} ticks"));
        }

        public string ComponentId { get; } = "Async";
        public void OnAttached(App app)
        {
            app.Bootstrapped += OnAppBootstrapped;
        }

        private void OnAppBootstrapped(App app)
        {
            App = app;
            Time = app.Time;
        }
    }
}