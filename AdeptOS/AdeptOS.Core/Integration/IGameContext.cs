using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public interface IGameContext
        {
            void Echo(string s);

            int CurrentSteps { get; }
            int MaxSteps { get; }
            string Storage { get; set; }

            IMyGridTerminalSystem Grid { get; }
        }
    }
}
