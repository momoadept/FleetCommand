using System;
using System.Collections.Generic;
using FC.Core.Core.BlockLoader;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.Delegates;
using FC.Core.Core.FakeAsync;
using FC.Core.Core.Logging;
using FC.Core.Core.Messaging;
using FC.Core.Core.ServiceProvider;
using FC.Core.Core.StatusReporting;
using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core
{
    public class App: IComponent
    {
        public static AppConfig GlobalConfiguration { get; set; }
        public static void Echo(string text) => DebugContext.Echo(text);

        protected static MyGridProgram DebugContext;

        public const string ScriptTag = "MFC";

        public event Event.Handler<App> Bootstrapped;

        public string ComponentId { get; }
        public void OnAttached(App app)
        {
        }

        // Common singleton services

        public IMyServiceProvider ServiceProvider { get; } = new MyServiceProvider();
        public Time.Time Time { get; } = new Time.Time();

        // Common component instances

        protected MyGridProgram Context { get; }
        protected Async Async { get; private set; }
        protected IBlockLoader Blocks { get; private set; }

        // Bootstrapped component types

        protected List<IWorker> Workers { get; } = new List<IWorker>();
        protected List<IStatusReporter> StatusReporters { get; } = new List<IStatusReporter>();
        protected List<IMessageProcessor> MessageProcessors { get; } = new List<IMessageProcessor>();

        protected ILog Log { get; set; }
        protected List<string> InitLogEntries { get; } = new List<string>();

        public App(string id, MyGridProgram context)
        {
            ComponentId = id;
            DebugContext = Context = context;

            context.Runtime.UpdateFrequency = UpdateFrequency.Update1;

            BootstrapCommonServices();
        }

        public void Start()
        {
            Bootstrapped?.Invoke(this);

            Log = ServiceProvider.Get<ILogFactory>().GetLog(this);
            FlushSafeLog();
        }

        public void Tick(string argument, UpdateType updateSource)
        {
            try
            {
                if (IsTriggeredByItself(updateSource))
                {
                    AdvanceAppLifecycle();
                }
                else
                {
                    HandleExternalMessage(argument, updateSource);
                }
            }
            catch (Exception e)
            {
                SafeLog(e.Message);
                SafeLog(e.StackTrace);
                Context.Echo(e.Message + e.StackTrace);
            }
        }

        protected void AdvanceAppLifecycle()
        {
            AdvanceTime();
            Workers.ForEach(w => w.Tick());
        }

        protected void HandleExternalMessage(string argument, UpdateType updateSource)
        {
            var message = new ComponentMessage()
            {
                Text = argument
            };
            foreach (var messageProcessor in MessageProcessors)
            {
                messageProcessor.ProcessMessage(message);
                if (message.StopProcessing)
                {
                    break;
                }
            }
        }

        protected void AdvanceTime()
        {
            if ((Context.Runtime.UpdateFrequency & UpdateFrequency.Update100) != 0)
            {
                Time.Now += 100;
            }
            else if ((Context.Runtime.UpdateFrequency & UpdateFrequency.Update10) != 0)
            {
                Time.Now += 10;
            }
            else if ((Context.Runtime.UpdateFrequency & UpdateFrequency.Update1) != 0)
            {
                Time.Now += 1;
            }
        }

        protected bool IsTriggeredByItself(UpdateType updateSource)
        {
            return (updateSource & (UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100)) != 0;
        }

        public App BootstrapComponents(params IComponent[] components)
        {
            foreach (var component in components)
            {
                BootstrapComponent(component);
            }

            return this;
        }

        public T BootstrapComponent<T>(T component) where T : class, IComponent
        {
            if (component is IWorker)
            {
                Workers.Add(component as IWorker);
                SafeLog($"{component.ComponentId} attached as Worker");
            }

            if (component is IStatusReporter)
            {
                StatusReporters.Add(component as IStatusReporter);
                SafeLog($"{component.ComponentId} attached as Status Reporter");
            }

            if (component is IMessageProcessor)
            {
                MessageProcessors.Add(component as IMessageProcessor);
                SafeLog($"{component.ComponentId} attached as Message Processor");
            }

            if (component is IService)
            {
                foreach (var service in (component as IService).Provides)
                {
                    ServiceProvider.Use(component, service);
                    SafeLog($"{component.ComponentId} attached as provider for ${service}");
                }
            }

            component.OnAttached(this);
            return component;
        }

        protected void BootstrapCommonServices()
        {
            ServiceProvider.Use(this);
            ServiceProvider.Use(Context);

            BootstrapComponent(new BlockReferenceFactory());
            BootstrapComponent(new LcdLogFactory());

            Async = BootstrapComponent(new Async());
            Blocks = BootstrapComponent(new CacheBlockLoader());

            BootstrapComponent(new LcdStatusChecker(StatusReporters));
            BootstrapComponent(new LcdLoggingHub());
        }

        protected void SafeLog(string text)
        {
            if (Log != null)
            {
                Log.Info(text);
            }
            else
            {
                InitLogEntries.Add(text);
            }
        }

        protected void FlushSafeLog()
        {
            foreach (var initLogEntry in InitLogEntries)
            {
                Log.Info(initLogEntry);
            }
        }
    }
}