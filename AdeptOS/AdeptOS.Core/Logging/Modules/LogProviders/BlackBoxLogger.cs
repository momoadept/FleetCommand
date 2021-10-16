using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public class BlackBoxLogger: IModule, ILogProvider
        {
            public string UniqueName => "BBL";
            public string Alias => "Black Box Logger";

            IGameContext _gameContext;

            LogSeverity _level;
            Tag _targetTag;

            List<string> _lines = new List<string>(500);
            List<IMyTerminalBlock> _target = new List<IMyTerminalBlock>();

            public BlackBoxLogger(LogSeverity level = LogSeverity.Info, string targetTag = "BLACKBOX")
            {
                _targetTag = new Tag(targetTag);
                _level = level;
            }

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
            }

            public void Run()
            {
                _gameContext.Grid.SearchBlocksOfName(_targetTag.Wrapped, _target);
                var _surface = _gameContext.Me.GetSurface(0);
                _surface.WriteText("");
                Aos.Async.CreateJob(Flush).Start();
                //Aos.Async.CreateJob(() => this.Log(LogSeverity.Info, "Test Job")).Start();
            }
            
            public void Log(LogSeverity severity, params string[] items)
            {
                if (severity < _level)
                    return;

                var line = string.Join(" ", items);
                _lines.Add($"{Aos.Now.ToShortTimeString()} [{LogHelper.SeverityString(severity)}]: {line}");
            }

            public void OnSaving() => Flush();

            void Flush()
            {
                if (_lines.Count <= 0)
                    return;

                var output = string.Join("\n", _lines);
                foreach (var block in _target)
                    block.CustomData += output;

                _gameContext.Me.GetSurface(0).WriteText("\n", true);
                _gameContext.Me.GetSurface(0).WriteText(output, true);

                _lines.Clear();
            }
        }
    }
}
