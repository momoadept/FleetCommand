using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private AdeptOSNode node = new AdeptOSNode(); 

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            node.Start(new GameContext(this, x => Storage = x), new NodeConfiguration
            {
                IsMainNode = true,
                NodeAlias = "AdeptOS Demo",
                NodeId = "AOSDEMO",
                ShipAlias = "Demo Ship",
                ShipId = "DS1",
                Modules = new List<IModule>()
                {

                }
            });
        }

        public void Save()
        {
            node.Save();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            node.Tick(argument, updateSource);
        }
    }
}