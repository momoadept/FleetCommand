﻿using System.Collections.Generic;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.Interfaces;
using IngameScript.Core.Logging;
using IngameScript.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core
{
    public class App : IWorker, ILogProvider
    {
        public string Id { get; }
        public string LogEntityId { get; } = "APP";

        // Common singleton services

        public MyGridProgram Context { get; }
        public IMyServiceProvider ServiceProvider { get; } = new MyServiceProvider();
        public Async.Async Async { get; private set; }
        public IBlockLoader Blocks { get; private set; }

        // Bootstrapped component types

        protected List<IWorker> Workers { get; } = new List<IWorker>();
        protected List<IStatusReporter> StatusReporters { get; } = new List<IStatusReporter>();

        protected ILog Log { get; set; }

        public App(string id, MyGridProgram context)
        {
            Id = id;
            Context = context;
            Log = new LcdLog(ServiceProvider, this);

            BootstrapCommonServices();
        }

        public App BootstrapComponents(params IComponent[] components)
        {
            foreach (var component in components)
            {
                BootstrapComponent(component);
            }

            return this;
        }

        public T BootstrapComponent<T>(T component) where T: class, IComponent
        {
            if (component is IWorker)
            {
                Workers.Add(component as IWorker);
                Log.Log($"{component.ComponentId} attached as Worker");
            }

            if (component is IStatusReporter)
            {
                StatusReporters.Add(component as IStatusReporter);
                Log.Log($"{component.ComponentId} attached as Status Reporter");
            }

            if (component is IService)
            {
                foreach (var service in (component as IService).Provides)
                {
                    ServiceProvider.Use(component, service);
                    Log.Log($"{component.ComponentId} attached as provider for ${service}");
                }
            }

            return component;
        }

        public void Tick()
        {
            Time.Time.Now++;
            Workers.ForEach(w => w.Tick());
        }

        protected void BootstrapCommonServices()
        {
            ServiceProvider.Use(this);
            ServiceProvider.Use(Context);

            Async = BootstrapComponent(new Async.Async(ServiceProvider));
            Blocks = BootstrapComponent(new LazyBlockLoader(ServiceProvider));
        }
    }
}
