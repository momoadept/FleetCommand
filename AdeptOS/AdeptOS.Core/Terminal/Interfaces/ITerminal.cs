namespace IngameScript
{
    partial class Program
    {
        public interface ITerminal
        {
            void RegisterHandler(string protocol, IMessageHandler handler);
            void ProcessMessage(string message);
        }
    }
}
