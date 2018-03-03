using System.Collections.Generic;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
        public class LazyBlockLoader: IBlockLoader
        {
            public int RefreshPeriod { get; set; } = 100;

            public List<IMyTerminalBlock> Blocks { get; private set; }
            public List<IMyBlockGroup> Groups { get; private set; }

            private int lifetime = 0;
            private readonly MyGridProgram context;

            public LazyBlockLoader(IMyServiceProvider services)
            {
                context = services.Get<MyGridProgram>();
                services.Get<App>().Workers.Add(this);

                RefreshBlocks();
            }

            public void Tick()
            {
                lifetime++;

                if (lifetime >= RefreshPeriod)
                {
                    lifetime = 0;
                    RefreshBlocks();
                }
            }

            private void RefreshBlocks()
            {
                context.GridTerminalSystem.GetBlocks(Blocks);
                context.GridTerminalSystem.GetBlockGroups(Groups);
            }
        }
}
