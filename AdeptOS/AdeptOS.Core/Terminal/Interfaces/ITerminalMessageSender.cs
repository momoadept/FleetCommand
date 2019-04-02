namespace IngameScript
{
    partial class Program
    {
        public interface ITerminalMessageSender
        {
            void Send(string targetTag, string operationPath, string argument = null);
        }
    }
}
