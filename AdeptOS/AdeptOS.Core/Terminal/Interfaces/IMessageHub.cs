namespace IngameScript
{
    partial class Program
    {
        public interface IMessageHub
        {
            void RegisterHandler(string protocol, IMessageHandler handler);
            void ProcessMessage(string message);
        }
    }
}
