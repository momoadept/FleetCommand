using System.Collections.Generic;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.Interfaces;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core
{
        
        public class App : IWorker
        {

            public string Id { get; }

            public IMyServiceProvider ServiceProvider { get; } = new MyServiceProvider();

            public Async.Async Async { get; }

            public IBlockLoader Blocks { get; }

            public List<IWorker> Workers { get; } = new List<IWorker>();

            public App(string id, MyGridProgram context)
            {
                Id = id;
                
                ServiceProvider.Use(this);
                ServiceProvider.Use(context);
                ServiceProvider.Use<IBlockLoader>(Blocks);
                ServiceProvider.Use(Async);

                Async = new Async.Async(ServiceProvider);
                Blocks = new LazyBlockLoader(ServiceProvider);
            }

            public void Tick()
            {
                Time.Time.Now++;
                Workers.ForEach(w => w.Tick());
            }
        }
}
