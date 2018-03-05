using System;
using System.Collections.Generic;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync;
using IngameScript.Core.Logging;
using IngameScript.Core.ServiceProvider;
using IngameScript.Core.StatusReporting;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core
{
    public class App : IWorker
    {
        public static AppConfig GlobalConfiguration { get; set; }

        public const string ScriptTag = "MFC";
        public static string BlockTag(string value) => $"[{ScriptTag} {value}]";

        public App(string id, MyGridProgram context)
        {
            Id = id;
            Context = context;

            context.Runtime.UpdateFrequency = UpdateFrequency.Update1;

            BootstrapCommonServices();
        }

        public string Id { get; }

        // Common singleton services

        public MyGridProgram Context { get; }
        public static IMyServiceProvider ServiceProvider { get; } = new MyServiceProvider();
        public Async Async { get; private set; }
        public IBlockLoader Blocks { get; private set; }
        public static Time Time { get; } = new Time();

        // Bootstrapped component types

        protected List<IWorker> Workers { get; } = new List<IWorker>();
        protected List<IStatusReporter> StatusReporters { get; } = new List<IStatusReporter>();

        protected ILog Log { get; set; }
        public string LogEntityId { get; } = "APP";

        public void Tick()
        {
            try
            {
                Time.Now++;
                Workers.ForEach(w => w.Tick());
            }
            catch (Exception e)
            {
                Context.Echo(e.Message + e.StackTrace);
            }
            
        }

        public App BootstrapComponents(params IComponent[] components)
        {
            foreach (var component in components) BootstrapComponent(component);

            return this;
        }

        public T BootstrapComponent<T>(T component) where T : class, IComponent
        {
            if (component is IWorker)
            {
                Workers.Add(component as IWorker);
                Log?.Log($"{component.ComponentId} attached as Worker");
            }

            if (component is IStatusReporter)
            {
                StatusReporters.Add(component as IStatusReporter);
                Log?.Log($"{component.ComponentId} attached as Status Reporter");
            }

            if (component is IService)
                foreach (var service in (component as IService).Provides)
                {
                    ServiceProvider.Use(component, service);
                    Log?.Log($"{component.ComponentId} attached as provider for ${service}");
                }

            component.OnAttached(this);
            return component;
        }

        protected void BootstrapCommonServices()
        {
            ServiceProvider.Use(this);
            ServiceProvider.Use(Context);

            Async = BootstrapComponent(new Async());

            BootstrapComponent(new LcdLoggingHub());
            Log = new LcdLog("App");
            
            Blocks = BootstrapComponent(new LazyBlockLoader());
            BootstrapComponent(new LCDStatusChecker(StatusReporters));
        }
    }
}