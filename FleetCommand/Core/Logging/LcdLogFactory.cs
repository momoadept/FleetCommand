using System;
using IngameScript.Core.BlockReferences;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.ServiceProvider;

namespace IngameScript.Core.Logging
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