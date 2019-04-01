using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public static class Aos
        {
            public static IAsync Async;
            public static Config Seettings = new Config();
        }

        public class AdeptOSNode
        {
            private NodeConfiguration _node;
            private Scheduler _scheduler;
            private Builder _builder;
            private IGameContext _gameContext;

            public void Start(IGameContext gameContext, NodeConfiguration metadata)
            {
                _gameContext = gameContext;
                _node = metadata;
                Aos.Async = _scheduler = new Scheduler(gameContext);
                BuildAndRun();
            }

            public void Save()
            {
                _builder.SaveModules();
            }

            private void BuildAndRun()
            {
                _builder = new Builder(_gameContext);
                _builder.BindModules(_node.Modules);
                _builder.RestoreModules();
                _builder.RunModules();
            }

            public void Tick(string argument, UpdateType updateSource)
            {
                if ((updateSource & UpdateType.Update1) == UpdateType.Update1
                    || (updateSource & UpdateType.Update10) == UpdateType.Update10
                    || (updateSource & UpdateType.Update100) == UpdateType.Update100)
                    DoBackgroundTasks();
                
                if (!string.IsNullOrEmpty(argument))
                {
                    ExecuteCommand(argument);
                }
            }

            private void DoBackgroundTasks()
            {
                _gameContext.Echo(_scheduler.Tick());
            }

            private void ExecuteCommand(string argument)
            {

            }
        }
    }
}
