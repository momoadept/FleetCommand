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
            Dictionary<Tag, IMyProgrammableBlock> _targetsByTag;
            List<IMyProgrammableBlock> _targetsBuffer = new List<IMyProgrammableBlock>(10);

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>();
                _log = context.RequireOne<ILog>();
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

            public void RegisterHandler(string protocol, IMessageHandler handler) => _handlerByProtocol.Add(protocol, handler);

            public void ProcessMessage(string message)
            {
                var protocol = message.Substring(0, 1);
                if (_handlerByProtocol.ContainsKey(protocol))
                    Aos.Async
                        .Delay()
                        .Then(x => _handlerByProtocol[protocol].Handle(message));
                else if (protocol != "T")
                    //Action shortcut
                    ProcessMessage($"T|{message}|{{}}");
                // Can run with argument "DRILL.Start" instead of "T|DRILL.Start|{}"!
            }

            public void DispatchMessage(Tag targetTag, string message)
            {
                if (!_targetsByTag.ContainsKey(targetTag))
                {
                    _log.Warning($"Can't send message: no PB with tag [{targetTag}]");
                    return;
                }

                DispatchMessage(_targetsByTag[targetTag], message);
            }

            public void DispatchMessage(IMyProgrammableBlock target, string message) =>
                Aos.Async
                    .When(() => target.TryRun(message), Priority.Critical, 10000)
                    .Then(x => _log.Debug(message, "message sent, delay: " + x))
                    .Catch(e => _log.Error(message, "couldn't send", e.ToString()));

            void DiscoverProgrammables()
            {
                _gameContext.Grid.GetBlocksOfType(_targetsBuffer);
                _targetsByTag = new Dictionary<Tag, IMyProgrammableBlock>();
                foreach (var block in _targetsBuffer)
                    foreach (var tag in Tag.FromName(block.CustomName))
                        _targetsByTag.Add(tag, block);
            }
        }
    }
}
