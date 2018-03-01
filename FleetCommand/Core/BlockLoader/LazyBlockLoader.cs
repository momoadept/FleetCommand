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
}
