namespace IngameScript
{
    partial class Program
    {
        public interface IMessageSender
        {
            void DispatchMessage(string targetTag, string message);
        }
    }
}
