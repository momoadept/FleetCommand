using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        
        public class Scheduler: IAsync
        {
            private Dictionary<Priority, ITimedQueue<Action>> _queue = new Dictionary<Priority, ITimedQueue<Action>>();
            private SchedulerStats _stats;
            private string _performanceReport = "Waiting for performance snapshot...";
            private DateTime _now;

            public Scheduler(IGameContext context)
            {
                _stats = new SchedulerStats(context);

                _queue.Add(Priority.Critical, new SortedSetTimedQueue<Action>());
                _queue.Add(Priority.Routine, new SortedSetTimedQueue<Action>());
                _queue.Add(Priority.Unimportant, new SortedSetTimedQueue<Action>());
            }

            public IPromise<int> Delay(int ms, Priority priority = Priority.Routine)
            {
                var promise = new Promise<int>();
                var targetTime = DateTime.Now.AddMilliseconds(ms);

                _queue[priority]
                    .Push(
                        targetTime,
                        () => promise.Resolve((int)DateTime.Now.Subtract(targetTime).TotalMilliseconds)
                    );

                return promise;
            }

            public IPromise<int> When(Func<bool> condition, Priority priority = Priority.Routine, int timeout = 0)
            {
                var promise = new Promise<int>();
                var startTime = DateTime.Now;
                var interval = Aos.Seettings.Priorities.ConditionCheckInterval(priority);
                var tartetTime = startTime.AddMilliseconds(interval);
                
                _queue[priority].Push(
                    tartetTime,
                    () => CheckCondition(condition, priority, timeout, interval, promise, startTime)
                );

                return promise;
            }

            private void CheckCondition(Func<bool> condition, Priority priority, int timeout, int interval,
                Promise<int> promise, DateTime startTime)
            {
                var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;

                if (timeout > 0 && elapsed > timeout)
                    promise.Fail(new ConditionTimeoutException());
                else if (condition())
                    promise.Resolve((int)elapsed);
                else
                    _queue[priority].Push(          
                        DateTime.Now.AddMilliseconds(interval),
                        () => CheckCondition(condition, priority, timeout, interval, promise, startTime)
                    );
            }

            public IJob CreateJob(Action job, Priority priority = Priority.Routine)
            {
                return new Job(job, priority);
            }

            public string Tick()
            {
                _now = DateTime.Now;

                RunAllCriticalTasks();
                RunRoutineTasks();
                RunUnimportantTasks();

                return TrackPerformance();
            }

            private void RunAllCriticalTasks() => RunAll(Priority.Critical);

            private void RunRoutineTasks() => RunAll(Priority.Routine);

            private void RunUnimportantTasks() => RunAll(Priority.Unimportant);

            private void RunAll(Priority priority)
            {
                if (_queue[priority].AnyLessThan(_now))
                    return;
                
                foreach (var action in _queue[priority].PopLessThan(_now))
                {
                    _stats.IncActions();
                    action();
                }
            }

            private string TrackPerformance()
            {
                _stats.Tick();

                if (_stats.Ticks >= Aos.Seettings.SchedulerPerformance.PerformanceSnapshotTicks)
                    _performanceReport = _stats.Snapshot();

                return _performanceReport;
            }
        }
    }
}
