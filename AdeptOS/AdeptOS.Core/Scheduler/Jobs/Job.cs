﻿using System;

namespace IngameScript
{
    partial class Program
    {
        public class Job: IJob
        {
            public bool Running { get; private set; }
            public Priority Priority { get; set; }

            Action _action;
            bool _stopping;

            public Job(Action action, Priority priority)
            {
                _action = action;
                Priority = priority;
            }

            public IJob Start()
            {
                _stopping = false;

                if (!Running)
                {
                    Running = true;
                    Work();
                }

                return this;
            }

            public void Stop() => _stopping = true;

            public void Work()
            {
                if (!_stopping)
                {
                    try
                    {
                        _action();
                    }
                    catch (Exception e)
                    {
                        Running = false;
                        _stopping = false;
                        throw new Exception("Job stopped due to unhandled exception", e);
                    }
                    
                    Aos.Async
                        .Delay(Aos.Seettings.Priorities.JobCheckInterval(Priority))
                        .Then(delay => Work());
                }
                else
                {
                    _stopping = false;
                    Running = false;
                }
            }
        }
    }
}
