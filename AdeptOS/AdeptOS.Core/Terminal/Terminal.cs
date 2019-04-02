namespace IngameScript
{
    partial class Program
    {
        public class Terminal: IModule, ITerminal
        {
            public string UniqueName => "Terminal";
            public string Alias => "Terminal";

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
            }

            public void ProcessMessage(string message)
            {
            }
        }
    }
}
