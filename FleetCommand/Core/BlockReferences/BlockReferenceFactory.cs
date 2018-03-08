﻿using System;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public class BlockReferenceFactory : IComponent, IBlockReferenceFactory, IService
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

            return new TagBlockReference<T>(Loader, tag, AppTag);
        }

        public Type[] Provides { get; } = {typeof(IBlockReferenceFactory)};

        protected void OnAppBootstrapped(App app)
        {
            Loader = app.ServiceProvider.Get<IBlockLoader>();
            AppTag = app.ComponentId;

            IsReady = true;
        }

        public string ComponentId { get; } = "BlockReferences";
        public void OnAttached(App app)
        {
            app.Bootstrapped += OnAppBootstrapped;
        }
    }
}