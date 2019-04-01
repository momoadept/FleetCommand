using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class Builder
        {
            private IEnumerable<IModule> _modules;
            private IGameContext _gameContext;

            public Builder(IGameContext gameContext)
            {
                _gameContext = gameContext;
            }

            public void BindModules(IEnumerable<IModule> modules)
            {
                _modules = modules;
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
                    appParser.Parse(_modules, _gameContext.Storage);
                }
                catch (Exception e)
                {
                    _gameContext.Storage = "";
                    //Assume that code has changed and reset state
                }
            }

            private ObjectParser<IEnumerable<IModule>> CreateParser()
            {
                // we dynamically create properties for each module to save them with named keys
                var moduleMappings = _modules.Select(
                    it => new Property<IEnumerable<IModule>>(
                        it.UniqueName,
                        modules => modules.First(mod => mod.UniqueName == it.UniqueName),
                        (modules, value) => modules.First(mod => mod.UniqueName == it.UniqueName)?.Restore(value))
                ).ToList();

                var appParser = new ObjectParser<IEnumerable<IModule>>(moduleMappings);
                return appParser;
            }

            public void RunModules()
            {
                foreach (var module in _modules)
                    module.Run();
            }

            public void SaveModules()
            {
                var appParser = CreateParser();
                var state = appParser.Stringify(_modules);
                _gameContext.Storage = state;
            }
        }
    }
}
