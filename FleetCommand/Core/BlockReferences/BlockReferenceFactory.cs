using System;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public class BlockReferenceFactory : BaseComponent, IBlockReferenceFactory, IService
    {
        protected IBlockLoader Loader { get; private set; }
        protected string AppTag { get; private set; }

        protected bool IsReady = false;

        public TagBlockReference<T> GetReference<T>(string tag) where T : IMyTerminalBlock
        {
            if (!IsReady)
            {
                throw new Exception("Trying to get block reference before app is ready");
            }

            return new TagBlockReference<T>(Loader, Log, tag, AppTag);
        }

        public Type[] Provides { get; } = {typeof(IBlockReferenceFactory)};

        public BlockReferenceFactory() : base("BlockReferences")
        {
        }

        protected override void OnAppBootstrapped(App app)
        {
            base.OnAppBootstrapped(app);

            Loader = app.ServiceProvider.Get<IBlockLoader>();
            AppTag = app.ComponentId;

            IsReady = true;
        }
    }
}