using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public class TerminalMessasgeHandler: IModule, IMessageHandler
        {
            public string UniqueName => "TProtocol";
            public string Alias => "Terminal Protocol Handler";

            ILog _log;
            Dictionary<string, IControllable> _controllersByName;
            IMessageSender _sender;

            public void Bind(IBindingContext context)
            {
                var controllables = context.Any<IControllable>() ;
                _controllersByName = controllables.ToDictionary(it => it.UniqueName);
                _log = context.RequireOne<ILog>();
                _sender = context.RequireOne<IMessageSender>();
            }

            public void Run()
            {
            }

            public void OnSaving()
            {
            }

            /// <summary>
            /// Example message: T|module.operation|{argument}
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

            void ExecuteMessage(string messageString)
            {
                var message = new TerminalMessage(messageString);
                if (!_controllersByName.ContainsKey(message.ControllerName))
                {
                    if (Aos.Node.IsMainNode)
                        throw new Exception($"no controller \"{message.ControllerName}\"");

                    _log.Warning($"Can't handle message here, redirecting to main node: {messageString}");
                    _sender.DispatchMessage(new Tag(Aos.Node.MainNodeId), messageString);
                    return;
                }
                    
                var controller = _controllersByName[message.ControllerName];

                if(!controller.Actions.ContainsKey(message.ActionName))
                    throw new Exception($"{message.ControllerName} has no action {message.ActionName}");

                controller.Actions[message.ActionName]
                    .Do(message.Argument)
                    .Then(x => _log.Debug($"{message.Path} returned {x}"));

                _log.Debug($"Action executed OK: {messageString}");
            }
        }
    }
}
