using System;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.ServiceProvider;
using FC.Core.Core.Times;

namespace FC.Core.Core.Logging
{
    public class LcdLogFactory : ILogFactory, IService, IComponent
    {
        protected ILoggingHub Hub;
        protected IBlockReferenceFactory References;
        protected Time Time;
        protected bool IsReady = false;
        public ILog GetLog(IComponent target)
        {
            if (!IsReady)
            {
                throw new Exception($"Log factory called before app initialized");
            }

            return new LcdLog(target.ComponentId, Hub, References, Time);
        }

        public Type[] Provides { get; } = {typeof(ILogFactory)};
        public string ComponentId { get; } = "LogFactory";
        public void OnAttached(App app)
        {
            app.Bootstrapped += appl =>
            {
                Hub = appl.ServiceProvider.Get<ILoggingHub>();
                References = appl.ServiceProvider.Get<IBlockReferenceFactory>();
                Time = app.Time;

                IsReady = true;
            };
        }
    }
}