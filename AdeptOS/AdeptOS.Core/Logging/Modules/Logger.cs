using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class Logger: IModule, ILog
        {
            IEnumerable<ILogProvider> _providers;
            LogSeverity _globalLogSeverity;

            public Logger(LogSeverity globalLogSeverity)
            {
                _globalLogSeverity = globalLogSeverity;
            }

            public string UniqueName => "LogHub";
            public string Alias => "LogHub";

            public void Bind(IBindingContext context) => _providers = context.RequireAny<ILogProvider>();

            public void Run()
            {
                Info("==========||||||||||=========");
                Info($"Log started on {Aos.Node.ShipAlias}-{Aos.Node.ShipId}.{Aos.Node.NodeId} {Aos.Node.NodeAlias}");
            }

            public void OnSaving()
            {
                Info($"Systems saving");
                Info("==========||||||||||=========");
            }

            public void Debug(params string[] items) => Log(LogSeverity.Debug, items);

            public void Info(params string[] items) => Log(LogSeverity.Info, items);

            public void Warning(params string[] items) => Log(LogSeverity.Warning, items);

            public void Error(params string[] items) => Log(LogSeverity.Error, items);

            public void Fatal(params string[] items) => Log(LogSeverity.Fatal, items);

            void Log(LogSeverity severity, params string[] items)
            {
                if (severity < _globalLogSeverity)
                    return;

                foreach (var provider in _providers)
                {
                    provider.Log(severity, items);
                }
            }
        }
    }
}
