using IngameScript.Core;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    internal class Program : MyGridProgram
    {
        public App MyApp { get; set; }

        public Program()
        {
            MyApp = new App("TestShip", this);
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            MyApp.Tick();
        }
    }
}