using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class MessageHub: IModule, IMessageHub
        {
            public string UniqueName => "MessageHub";
            public string Alias => "Message Hub";

            Dictionary<string, IMessageHandler> _handlerByProtocol = new Dictionary<string, IMessageHandler>(3);

            public void Bind(IBindingContext context)
            {
            }

            public void Run()
            {
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
        }
    }
}
