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

        public class AdeptOSNode
        {
            private NodeConfiguration _node;
            private Scheduler _scheduler;
            private Builder _builder;
            private IGameContext _gameContext;

            public void Start(IGameContext gameContext, NodeConfiguration metadata)
            {
                _gameContext = gameContext;
                _node = metadata;
                Aos.Async = _scheduler = new Scheduler(gameContext);
                BuildAndRun();
            }

            private void BuildAndRun()
            {
                _builder = new Builder();
                _builder.BindModules(_node.Modules);
                _builder.RunModules();
            }

            public void Tick(string argument, UpdateType updateSource)
            {
                if ((updateSource & UpdateType.Update1) == UpdateType.Update1
                    || (updateSource & UpdateType.Update10) == UpdateType.Update10
                    || (updateSource & UpdateType.Update100) == UpdateType.Update100)
                    DoBackgroundTasks();
                
                if (!string.IsNullOrEmpty(argument))
                {
                    ExecuteCommand(argument);
                }
            }

            private void DoBackgroundTasks()
            {
                _gameContext.Echo(_scheduler.Tick());
            }

            private void ExecuteCommand(string argument)
            {

            }
        }
    }
}
