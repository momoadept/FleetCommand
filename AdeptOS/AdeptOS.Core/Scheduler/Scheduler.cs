using System.Collections.Generic;
using System;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class Scheduler: IAsync
        {
            readonly IGameContext _context;
            Dictionary<Priority, ITimedQueue<Action>> _queue = new Dictionary<Priority, ITimedQueue<Action>>();
             SchedulerStats _stats;
            string _performanceReport = "Waiting for performance snapshot...";
            DateTime _now;

            public Scheduler(IGameContext context)
            {
                _context = context;
                _stats = new SchedulerStats(context);

                _queue.Add(Priority.Critical, new SortedSetTimedQueue<Action>());
                _queue.Add(Priority.Routine, new SortedSetTimedQueue<Action>());
                _queue.Add(Priority.Unimportant, new SortedSetTimedQueue<Action>());
            }

            // Promise generators

            public IPromise<int> Delay(int ms = 0, Priority priority = Priority.Routine)
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

            public IPromise<int> When(Func<bool> condition, Priority priority = Priority.Routine, int timeout = 1000*60)
            {
                var promise = new Promise<int>();
                var startTime = DateTime.Now;
                var interval = Aos.Seettings.Priorities.ConditionCheckInterval(priority);
                
                _queue[priority].Push(
                    startTime,
                    () => CheckCondition(condition, priority, timeout, interval, promise, startTime)
                );

                return promise;
            }

            void CheckCondition(Func<bool> condition, Priority priority, int timeout, int interval,
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

            public IJob CreateJob(Action job, Priority priority = Priority.Routine) => new Job(job, priority);

            // Execution and Balancing

            public string Tick()
            {
                _now = DateTime.Now;

                RunAllCriticalTasks();
                RunRoutineTasks();
                RunUnimportantTasks();

                return TrackPerformance();
            }

            // criticals are guaranteed to run
            void RunAllCriticalTasks() => RunAll(Priority.Critical);

            // Balancing iteration 1. This will spread out pikes of scheduled operations, but may stagnate under high load
            void RunRoutineTasks() => RunSingle(Priority.Routine); // given 6 global ticks per second, this will start stagnating with 6 concurrent jobs and slowing down execution
            void RunUnimportantTasks() => RunSingle(Priority.Unimportant); // stagnation at 180 concurrent jobs, but jobs are spaced out much more (twice a minute)

            void RunAll(Priority priority)
            {
                if (!_queue[priority].AnyLessThan(_now))
                    return;

                var actions = _queue[priority].PopLessThan(_now).ToList();
                foreach (var action in actions)
                {
                    _stats.IncActions();
                    action();

                    if ((float)_context.CurrentSteps / _context.MaxSteps >= 0.05)
                        throw new Exception("5% complexity exceeded");
                }
            }

            void RunSingle(Priority priority)
            {
                if (!_queue[priority].AnyLessThan(_now))
                    return;

                _stats.IncActions();
                _queue[priority].PopNext()();
            }

            string TrackPerformance()
            {
                _stats.Tick();

                if (_stats.Ticks >= Aos.Seettings.SchedulerPerformance.PerformanceSnapshotTicks)
                {
                    var pendingRoutines = _queue[Priority.Routine].Count;
                    var pendingCritical = _queue[Priority.Critical].Count;
                    var pendingOther = _queue[Priority.Unimportant].Count;
                    _performanceReport = $"{_stats.Snapshot()}\n{pendingRoutines} Routine tasks pending (>6 = stagnation)\n{pendingCritical} Critical\n{pendingOther} Other";
                }
                    

                return _performanceReport;
            }
        }
    }
}
