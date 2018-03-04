using System;
using System.Collections.Generic;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public class LazyBlockLoader : IBlockLoader, IComponent, IService
    {
        private MyGridProgram context;

        public int RefreshPeriod { get; set; } = 1000;

        public List<IMyTerminalBlock> Blocks { get; } = new List<IMyTerminalBlock>();
        public List<IMyBlockGroup> Groups { get; } = new List<IMyBlockGroup>();

        public string ComponentId { get; } = "DefaultBlockLoader";

        private SimpleAsyncWorker blockPollingWorker;

        public void OnAttached(App app)
        {
            context = App.ServiceProvider.Get<MyGridProgram>();

            blockPollingWorker = new SimpleAsyncWorker("BlockPollingWorker", RefreshBlocks, RefreshPeriod);
            app.Async.AddJob(blockPollingWorker);
            blockPollingWorker.Start();
        }

        public Type[] Provides { get; } = {typeof(IBlockLoader)};

        private void RefreshBlocks()
        {
            context.GridTerminalSystem.GetBlocks(Blocks);
            context.GridTerminalSystem.GetBlockGroups(Groups);
        }
    }
}