namespace IngameScript
{
    partial class Program
    {
        public interface IMessageSender
        {
            void DispatchMessage(Tag targetTag, string message);
        }
    }
}
