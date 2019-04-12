
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public interface IMessageSender
        {
            void DispatchMessage(Tag targetTag, string message);
            void DispatchMessage(IMyProgrammableBlock target, string message);
        }
    }
}
