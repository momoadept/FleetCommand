using IngameScript.Core.FakeAsync;
using IngameScript.Core.Logging;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.ComponentModel
{
    public class BaseComponent: IComponent
    {
        public string ComponentId { get; }
        protected ILog Log { get; private set; }
        protected MyGridProgram Context { get; private set; }
        protected Async Async { get; private set; }
        protected Time.Time Time { get; private set; }

        public BaseComponent(string componentId)
        {
            ComponentId = componentId;
        }
        public virtual void OnAttached(App app)
        {
            app.Bootstrapped += OnAppBootstrapped;
            Time = app.Time;
        }

        protected virtual void OnAppBootstrapped(App app)
        {
            Context = app.ServiceProvider.Get<MyGridProgram>();
            Async = app.ServiceProvider.Get<Async>();
            Log = app.ServiceProvider.Get<ILogFactory>().GetLog(this);
        }
    }
}
