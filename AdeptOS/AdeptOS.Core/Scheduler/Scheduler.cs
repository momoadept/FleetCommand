using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        
        public class Scheduler: IAsync
        {
            private Dictionary<Priority, SortedList<DateTime, Action>> _queue = new Dictionary<Priority, SortedList<DateTime, Action>>();

            public Scheduler()
            {
                _queue.Add(Priority.Critical, new SortedList<DateTime, Action>());
                _queue.Add(Priority.Routine, new SortedList<DateTime, Action>());
                _queue.Add(Priority.Unimportant, new SortedList<DateTime, Action>());
            }

            public IPromise<int> Delay(int ms, Priority priority = Priority.Routine)
            {
                var promise = new Promise<int>();
                var targetTime = DateTime.Now.AddMilliseconds(ms);

                _queue[priority]
                    .Add(
                        targetTime,
                        () => promise.Resolve(DateTime.Now.Subtract(targetTime).Milliseconds)
                    );

                return promise;
            }

            public IPromise<int> When(Func<bool> condition, Priority priority = Priority.Routine, int timeout = 0)
            {
                var promise = new Promise<int>();
                var startTime = DateTime.Now;
                var interval = Aos.Seettings.PriorityConfig.ConditionCheckInterval(priority);
                var tartetTime = startTime.AddMilliseconds(interval);

                _queue[priority].Add(
                    tartetTime,
                    () => CheckCondition(condition, priority, timeout, interval, promise, startTime)
                );

                return promise;
            }

            private void CheckCondition(Func<bool> condition, Priority priority, int timeout, int interval,
                Promise<int> promise, DateTime startTime)
            {
                var elapsed = DateTime.Now.Subtract(startTime).Milliseconds;

                if (timeout > 0 && elapsed > timeout)
                    promise.Fail(new ConditionTimeoutException());
                else if (condition())
                    promise.Resolve(elapsed);

                else
                    _queue[priority].Add(
                        DateTime.Now.AddMilliseconds(interval),
                        () => CheckCondition(condition, priority, timeout, interval, promise, startTime)
                    );
            }

            public IJob AddJob(Action job, Priority priority = Priority.Routine)
            {
                return new Job(job, priority);
            }

            public string Tick()
            {

            }
        }
    }
}
