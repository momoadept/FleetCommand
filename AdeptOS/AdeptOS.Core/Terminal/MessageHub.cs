using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public class MessageHub: IModule, IMessageHub, IMessageSender
        {
            public string UniqueName => "MessageHub";
            public string Alias => "Message Hub";

            ILog _log;
            IGameContext _gameContext;

            Dictionary<string, IMessageHandler> _handlerByProtocol = new Dictionary<string, IMessageHandler>(3);
            Dictionary<string, IMyProgrammableBlock> _targetsByTag;
            List<IMyProgrammableBlock> _targetsBuffer = new List<IMyProgrammableBlock>(10);

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
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

            public void RegisterHandler(string protocol, IMessageHandler handler)
            {
                _handlerByProtocol.Add(protocol, handler);
            }

            public void ProcessMessage(string message)
            {
                var protocol = message.Substring(0, 1);
                Aos.Async
                    .Delay()
                    .Then(x => _handlerByProtocol[protocol].Handle(message));
            }

            public void DispatchMessage(string targetTag, string message)
            {
                if (!_targetsByTag.ContainsKey(targetTag))
                {
                    _log.Warning($"Can't send message: no PB with tag [{targetTag}]");
                    return;
                }

                Aos.Async
                    .When(() => _targetsByTag[targetTag].TryRun(message), Priority.Critical, 10000)
                    .Then(x => _log.Debug(message, "message sent, delay: " + x))
                    .Catch(e => _log.Error(message, "couldn't send", e.ToString()));
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
        }
    }
}
