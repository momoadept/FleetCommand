using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
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
        #region mdk preserve
        NConf Pckg = new NConf
        {
            IsMainNode = true,
            NodeAlias = "AdeptOS Inventory Manager",
            NodeId = "AosInv",
            ShipAlias = "Ship1",
            ShipId = "DS1",
            Modules = new List<IModule>()
            {
                new BlackBoxLogger(LogSeverity.Debug),
                new InventoryManagerController(),
            }
        };

        #endregion

        private AdeptOSNode node = new AdeptOSNode();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            node.Start(new GameContext(this, x => Storage = x), Pckg);
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