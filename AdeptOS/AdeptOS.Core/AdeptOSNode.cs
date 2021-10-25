using System;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public static class Aos
        {
            public static IAsync Async;
            public static Config Seettings = new Config();
            public static NConf Node;
            public static DateTime Now;
        }

        public class AdeptOSNode
        {
            NConf _node;
            Scheduler _scheduler;
            Builder _builder;
            IGameContext _gameContext;
            ILog _log;
            IMessageHub _messageHub;
            IMyTextSurface _surface;

            public void Start(IGameContext gameContext, NConf metadata)
            {
                _gameContext = gameContext;
                _node = Aos.Node = metadata;
                Aos.Async = _scheduler = new Scheduler(gameContext);
                try
                {
                    BuildAndRun();
                }
                catch (Exception e)
                {
                    gameContext.Echo("FUCK at Build phase");
                    gameContext.Echo(e.ToString());
                    throw;
                }

                _surface = gameContext.Me.GetSurface(1);
                _surface.ContentType = ContentType.TEXT_AND_IMAGE;
                _surface.WriteText(gameContext.Storage);
            }

            public void Save()
            {
                try
                {
                    _builder.SaveModules();
                    _surface.WriteText(_gameContext.Storage);
                }
                catch (Exception e)
                {
                    _gameContext.Echo("FUCK at Save phase");
                    _gameContext.Echo(e.ToString());
                    throw;
                }
            }

            void BuildAndRun()
            {
                _builder = new Builder(_gameContext);
                _builder.BindModules(_node.Modules);
                _builder.RestoreModules();
                _builder.RunModules();
                _log = _builder.GetLog();
                _messageHub = _builder.GetMessageHub();
            }

            public void Tick(string argument, UpdateType updateSource)
            {
                Aos.Now = DateTime.Now;
                try
                {
                    if (!string.IsNullOrEmpty(argument))
                        ExecuteCommand(argument);

                    if ((updateSource & UpdateType.Update1) == UpdateType.Update1
                        || (updateSource & UpdateType.Update10) == UpdateType.Update10
                        || (updateSource & UpdateType.Update100) == UpdateType.Update100)
                        DoBackgroundTasks();
                }
                catch (Exception e)
                {
                    _gameContext.Echo("Unhandled exception in AOS runtime");
                    _gameContext.Echo(e.ToString());
                    _log.Fatal(e.ToString());
                    //throw;
                }
            }

            void DoBackgroundTasks()
            {
                var snapshot = _scheduler.Tick();
                _gameContext.Echo(snapshot);
            }

            void ExecuteCommand(string argument) => _messageHub.ProcessMessage(argument);
        }
    }
}
