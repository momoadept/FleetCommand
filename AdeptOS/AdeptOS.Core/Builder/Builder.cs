using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class Builder
        {
            List<IModule> _modules = new List<IModule>();
            IGameContext _gameContext;
            MessageHub _messageHub;
            ILog _log;

            public Builder(IGameContext gameContext)
            {
                _gameContext = gameContext;
                SetupCoreModules();
            }

            private void SetupCoreModules()
            {
                _modules.Add(_gameContext as IModule);
                _modules.Add((_log = new Logger(LogSeverity.Debug)) as IModule);
                _messageHub = new MessageHub();
                _modules.Add(_messageHub);
                var terminalHandler = new TerminalMessasgeHandler();
                _modules.Add(terminalHandler);
                _messageHub.RegisterHandler("T", terminalHandler);
            }

            public void BindModules(IEnumerable<IModule> modules)
            {
                _modules.AddRange(modules);
                var bindingContext = new BindingContext(_modules);

                foreach (var module in _modules)
                    module.Bind(bindingContext);
            }

            public void RestoreModules()
            {
                if (string.IsNullOrEmpty(_gameContext.Storage))
                    return;

                try
                {
                    var appParser = CreateParser();
                    appParser.Parse(GetStorableModules(), _gameContext.Storage);
                }
                catch (Exception)
                {
                    _gameContext.Storage = "";
                    //Assume that code has changed and reset state
                }
            }

            ObjectParser<IEnumerable<IStorableModule>> CreateParser()
            {
                // we dynamically create properties for each module to save them with named keys
                var moduleMappings = _modules.Select(
                    it => new Property<IEnumerable<IStorableModule>>(
                        it.UniqueName,
                        modules => modules.FirstOrDefault(mod => mod.UniqueName == it.UniqueName),
                        (modules, value) => modules.FirstOrDefault(mod => mod.UniqueName == it.UniqueName)?.Restore(value))
                ).ToList();

                var appParser = new ObjectParser<IEnumerable<IStorableModule>>(moduleMappings);
                return appParser;
            }

            public void RunModules()
            {
                foreach (var module in _modules)
                    module.Run();
            }

            public ILog GetLog() => _log;
            public IMessageHub GetMessageHub() => _messageHub;

            public void SaveModules()
            {
                var appParser = CreateParser();
                var state = appParser.Stringify(GetStorableModules());
                _gameContext.Storage = state;
            }

            IEnumerable<IStorableModule> GetStorableModules() => _modules.Where(it => it is IStorableModule).Cast<IStorableModule>();
        }
    }
}
