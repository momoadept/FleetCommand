using System;
using System.Collections.Generic;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public class LazyBlockLoader : BaseComponent, IBlockLoader, IService
    {
        public int RefreshPeriod { get; set; } = 1000;
        public Type[] Provides { get; } = { typeof(IBlockLoader) };
        public List<IMyTerminalBlock> Blocks { get; } = new List<IMyTerminalBlock>();
        public List<IMyBlockGroup> Groups { get; } = new List<IMyBlockGroup>();

        private SimpleAsyncWorker blockPollingWorker;
        private MyGridProgram context;

        public LazyBlockLoader()
            :base("DefaultBlockLoader")
        {
            
        }

        public override void OnAttached(App app)
        {
            context = App.ServiceProvider.Get<MyGridProgram>();

            blockPollingWorker = new SimpleAsyncWorker("BlockPollingWorker", RefreshBlocks, RefreshPeriod);
            app.Async.AddJob(blockPollingWorker);
            blockPollingWorker.Start();
        }

        

        private void RefreshBlocks()
        {
            context.GridTerminalSystem.GetBlocks(Blocks);
            context.GridTerminalSystem.GetBlockGroups(Groups);
            Log.Debug($"{Blocks.Count} blocks and {Groups.Count} groups detected");
        }
    }
}