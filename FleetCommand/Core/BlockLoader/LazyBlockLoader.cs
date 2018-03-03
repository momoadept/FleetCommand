using System;
using System.Collections.Generic;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public class LazyBlockLoader : IBlockLoader, IComponent, IService
    {
        public string ComponentId { get; } = "DefaultBlockLoader";

        public Type[] Provides { get; } = {typeof(IBlockLoader)};

        public int RefreshPeriod { get; set; } = 100;

        public List<IMyTerminalBlock> Blocks { get; } = new List<IMyTerminalBlock>();
        public List<IMyBlockGroup> Groups { get; } = new List<IMyBlockGroup>();

        private int lifetime = 0;
        private readonly MyGridProgram context;

        public LazyBlockLoader(IMyServiceProvider services)
        {
            context = services.Get<MyGridProgram>();
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
