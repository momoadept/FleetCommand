using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
// ReSharper disable UnusedMember.Global

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        #region mdk preserve
        NConf Pckg = new NConf
        {
            IsMainNode = true,
            NodeAlias = "Drilling station",
            NodeId = "Drill1",
            ShipAlias = "Drilling station",
            ShipId = "Drill1",
            Modules = new List<IModule>()
            {
                new BlackBoxLogger(LogSeverity.Debug),
                new RotorDrillController(),
                new LcdTracer(),
            }
        };

        #endregion

        AdeptOSNode node = new AdeptOSNode();

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
