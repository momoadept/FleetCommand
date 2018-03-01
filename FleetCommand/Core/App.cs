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
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public App MyApp { get; set; }
        public class App : IWorker
        {

            public string Id { get; }

            public IMyServiceProvider ServiceProvider { get; } = new MyServiceProvider();

            public Async Async { get; }

            public IBlockLoader Blocks { get; }

            public List<IWorker> Workers { get; } = new List<IWorker>();

            public App(string id, MyGridProgram context)
            {
                Id = id;
                
                ServiceProvider.Use(this);
                ServiceProvider.Use(context);
                ServiceProvider.Use<IBlockLoader>(Blocks);
                ServiceProvider.Use(Async);

                Async = new Async(ServiceProvider);
                Blocks = new LazyBlockLoader(ServiceProvider);
            }

            public void Tick()
            {
                Time.Now++;
                Workers.ForEach(w => w.Tick());
            }
        }
    }

}
