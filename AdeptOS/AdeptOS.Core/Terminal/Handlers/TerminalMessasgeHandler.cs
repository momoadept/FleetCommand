using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public class TerminalMessasgeHandler: IModule, IMessageHandler, ITerminalMessageSender
        {
            public string UniqueName => "TProtocol";
            public string Alias => "Terminal Protocol Handler";

            IGameContext _gameContext;
            Dictionary<string, OperationGroup> _controllers;
            ILog _log;

            Dictionary<string, IMyProgrammableBlock> _targetsByTag;
            List<IMyProgrammableBlock> _targetsBuffer = new List<IMyProgrammableBlock>(10);

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
                var controllables = context.RequireAny<IControllable>(this);
                _controllers = controllables.ToDictionary(it => it.Controller.Name, it => it.Controller);
                _log = context.RequireOne<ILog>(this);
            }

            public void Run()
            {
                Aos.Async
                    .CreateJob(DiscoverProgrammables, Priority.Unimportant)
                    .Start();

                DiscoverProgrammables();
            }

            public void OnSaving()
            {
            }

            /// <summary>
            /// Example message: T|module.group.operation|{argument}
            /// </summary>
            public void Handle(string message)
            {
                try
                {
                    ExecuteMessage(message);
                }
                catch (Exception e)
                {
                    _log.Error($"Terminal: could not process message. message: {message}, error: {e}");
                }
            }

            private void ExecuteMessage(string message)
            {
                var messageParts = message.Split('|');
                var path = messageParts[1];
                var arg = messageParts.Length >= 3 ? messageParts[2] : null;

                var nodeNames = path.Split('.');
                if (!_controllers.ContainsKey(nodeNames[0]))
                    throw new Exception($"no module to handle group \"{nodeNames[0]}\"");

                var operations = _controllers[nodeNames[0]].OperationsByPath();
                if (!operations.ContainsKey(path))
                    throw new Exception($"Can't find terminal node: {path}");

                operations[path].Invoke(arg);
            }

            private void DiscoverProgrammables()
            {
                _gameContext.Grid.GetBlocksOfType(_targetsBuffer);
                _targetsByTag = new Dictionary<string, IMyProgrammableBlock>();
                foreach (var block in _targetsBuffer)
                {
                    var tags = Tag.Tags(block.CustomName);
                    foreach (var tag in tags)
                    {
                        _targetsByTag.Add(tag, block);
                    }
                }
            }

            public void Send(string targetTag, string operationPath, string argument = null)
            {
                if (!_targetsByTag.ContainsKey(targetTag))
                {
                    _log.Error($"No target for T operation found: [{targetTag}]");
                    return;
                }

                var message = $"T|{operationPath}|{argument}";

                Aos.Async
                    .When(() => _targetsByTag[targetTag].TryRun(message), Priority.Critical, 10000)
                    .Then(x => _log.Debug(message, "T message sent, delay: " + x))
                    .Catch(e => _log.Error(message, "couldn't send", e.ToString()));
            }
        }
    }
}
