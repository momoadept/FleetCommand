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
            string _targetTag;

            List<string> _lines = new List<string>(500);
            List<IMyTerminalBlock> _target = new List<IMyTerminalBlock>();

            public BlackBoxLogger(LogSeverity level = LogSeverity.Info, string targetTag = "BLACKBOX")
            {
                _targetTag = Tag.Wrap(targetTag);
                _level = level;
            }

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
            }

            public void Run()
            {
                _gameContext.Grid.SearchBlocksOfName(_targetTag, _target);
                Aos.Async.CreateJob(Flush).Start();
            }
            
            public void Log(LogSeverity severity, params string[] items)
            {
                if (severity < _level)
                    return;

                var line = string.Join(" ", items);
                _lines.Add($"{Aos.Now} [{LogHelper.SeverityString(severity)}]: {line}");
            }

            public void OnSaving()
            {
                Flush();
            }

            private void Flush()
            {
                var output = string.Join("\n", _lines);
                foreach (var block in _target)
                    block.CustomData += output;
                
                _lines.Clear();
            }
        }
    }
}
