using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class TerminalMessasgeHandler: StorableModule<TerminalMessasgeHandler>, IMessageHandler
        {
            public override string UniqueName { get; }
            public override string Alias { get; }

            IGameContext _gameContext;
            Dictionary<string, OperationGroup> _controllers;
            ILog _log;

            private static List<Property<TerminalMessasgeHandler>> mapping = new List<Property<TerminalMessasgeHandler>>()
            {

            };

            public TerminalMessasgeHandler() : base(mapping)
            {
            }

            public override void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
                var controllables = context.RequireAny<IControllable>(this);
                _controllers = controllables.ToDictionary(it => it.Controller.Name, it => it.Controller);
                _log = context.RequireOne<ILog>(this);
            }

            public override void Run()
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
                var arg = messageParts.Length >= 3 ? messageParts[3] : null;

                var nodeNames = path.Split('.');
                if (!_controllers.ContainsKey(nodeNames[0]))
                    throw new Exception($"no module to handle group \"{nodeNames[0]}\"");

                var operation = FindOperation(nodeNames);
                
            }

            private OperationContract FindOperation(string[] nodeNames)
            {
                var currentNode = _controllers[nodeNames[1]] as IOperationNode;
                var nextInPath = 0;

                while (true)
                {
                    ++nextInPath;

                    if (nextInPath >= nodeNames.Length)
                        break;

                    var nextNodeName = nodeNames[nextInPath];
                    var group = (OperationGroup)currentNode;

                    if (!group.Children.ContainsKey(nextNodeName))
                        throw new Exception($"Can't find terminal node: {nextNodeName}");

                    currentNode = group.Children[nextNodeName];
                }

                if (!(currentNode is OperationContract))
                    throw new Exception($"{currentNode.Name} is not an operation");

                return (OperationContract) currentNode;
            }
        }
    }
}
