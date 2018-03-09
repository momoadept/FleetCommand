using System;
using System.Collections.Generic;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.Delegates;
using FC.Core.Core.FakeAsync;
using FC.Core.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core.BlockLoader
{
    public class CacheBlockLoader : BaseComponent, IBlockLoader, IService
    {
        public int RefreshPeriod { get; set; } = 1000;
        public Type[] Provides { get; } = { typeof(IBlockLoader) };
        public List<IMyTerminalBlock> Blocks { get; } = new List<IMyTerminalBlock>();
        public List<IMyBlockGroup> Groups { get; } = new List<IMyBlockGroup>();

        public event Event.Handler<IBlockLoader> Updated;

        protected SimpleAsyncWorker BlockPollingWorker;

        public CacheBlockLoader()
            :base("DefaultBlockLoader")
        {
            
        }

        protected override void OnAppBootstrapped(App app)
        {
            base.OnAppBootstrapped(app);

            BlockPollingWorker = new SimpleAsyncWorker("BlockPollingWorker", RefreshBlocks, RefreshPeriod);
            Async.AddJob(BlockPollingWorker);
            BlockPollingWorker.Start();
        }


        private void RefreshBlocks()
        {
            Context.GridTerminalSystem.GetBlocks(Blocks);
            Context.GridTerminalSystem.GetBlockGroups(Groups);
            Log.Debug($"{Blocks.Count} blocks and {Groups.Count} groups detected");
            Updated?.Invoke(this);
        }
    }
}