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
        public class Job: IJob
        {
            public bool Running { get; private set; }

            public Priority Priority { get; set; }

            private Action _action;

            private bool _stopping;

            public Job(Action action, Priority priority)
            {
                _action = action;
                Priority = priority;
            }

            public void Start()
            {
                _stopping = false;

                if (!Running)
                {
                    Running = true;
                    Work();
                }
            }

            public void Stop()
            {
                _stopping = true;
            }

            public void Work()
            {
                if (!_stopping)
                {
                    _action();
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
