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
        public static class Aos
        {
            public static IAsync Async;
            public static Config Seettings = new Config();
        }

        public class Runner
        {
            private RunnerMetadata _metadata;
            private Scheduler _scheduler;
            private IGameContext _gameContext;

            public void Create(RunnerMetadata metadata, IGameContext gameContext)
            {
                _gameContext = gameContext;
                _metadata = metadata;
                Aos.Async = _scheduler = new Scheduler(gameContext);
            }

            public void Tick(string argument)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    DoBackgroundTasks();
                }
                else
                {
                    ExecuteCommand(argument);
                }
            }

            private void DoBackgroundTasks()
            {
                _scheduler.Tick();
            }

            private void ExecuteCommand(string argument)
            {

            }
        }
    }
}
