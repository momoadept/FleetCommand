using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private AdeptOSNode node = new AdeptOSNode(); 

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            node.Start(new GameContext(this), new NodeConfiguration
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
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            node.Tick(argument, updateSource);
        }
    }
}