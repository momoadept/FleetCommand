using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using System.Diagnostics;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class RPC : IModule, IRPC, IMessageHandler
        {
            public string UniqueName => "RPC";
            public string Alias => "RPC";

            IMessageSender _sender;
            Dictionary<string, IRemoteImpl> _implementationsByName;
            Dictionary<string, IResolvablePromise<string>> _waitingForResult = new Dictionary<string, IResolvablePromise<string>>();

            public void Bind(IBindingContext context)
            {
                var messageHub = context.RequireOne<IMessageHub>();
                messageHub.RegisterHandler("R", this);
                _implementationsByName = context.RequireAny<IRemoteImpl>()
                    .ToDictionary(it => it.UniqueName);
                _sender = context.RequireOne<IMessageSender>();
            }

            public void Run()
            {
            }

            public void OnSaving()
            {
            }

            public IPromise<string> Call(RpcRoute route, string argument)
            {
                var message = new RpcMessage
                {
                    Type = RpcMessageType.ActionRequest,
                    Id = Id(),
                    ControllerName = route.ControllerName,
                    ActionName = route.ActionName,
                    Data = argument,
                    ReturnTag = new Tag(Aos.Node.NodeId)
                };

                var promise = new Promise<string>();
                _waitingForResult.Add(message.Id, promise);
                _sender.DispatchMessage(route.TargetTag, message.ToString());
                return promise;
            }

            public void Handle(string message)
            {
                var rpcMessage = new RpcMessage(message);
                if (rpcMessage.Type == RpcMessageType.ActionRequest)
                    ExecuteAction(rpcMessage);
                else
                    AcceptResult(rpcMessage);
            }

            void ExecuteAction(RpcMessage message)
            {
                var impl = _implementationsByName[message.ControllerName];
                var operation = impl.Actions[message.ActionName];
                operation
                    .Do(message.Data)
                    .Then(result =>
                    {
                        var data = (result as IStringifiable)?.Stringify() ?? result.ToString();
                        var returnMessage = new RpcMessage()
                        {
                            Id = message.Id,
                            Type = RpcMessageType.ReturnedResult,
                            Data = data
                        };
                        _sender.DispatchMessage(message.ReturnTag, returnMessage.ToString());
                    });
            }

            void AcceptResult(RpcMessage message)
            {
                var promise = _waitingForResult[message.Id];
                _waitingForResult.Remove(message.Id);
                promise.Resolve(message.Data);
            }

            int _counter;
            string Id()
            {
                if (_counter >= 10000)
                    _counter = 0;
                return $"{Aos.Node.NodeId}-{++_counter}";
            }
        }
    }
}
