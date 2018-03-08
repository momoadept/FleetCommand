using System;
using System.Collections.Generic;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.Delegates;
using IngameScript.Core.FakeAsync;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
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