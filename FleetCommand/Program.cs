using IngameScript.Core;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    internal class Program : MyGridProgram
    {
        public Program()
        {
            MyApp = new App("TestShip", this);
        }

        public App MyApp { get; set; }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
        }
    }
}