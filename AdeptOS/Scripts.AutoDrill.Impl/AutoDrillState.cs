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

namespace IngameScript
{
    partial class Program
    {
        public enum DrillingStage
        {
            DrillForwards = 1,
            LowerPiston = 2,
            DrillBackwards = 3,
            Resetting = 4,
            Ready = 5,
            Done = 6,
        }

        public class AutoDrillState : BaseDataObject<AutoDrillState>
        {
            public bool IsWorking;
            public DrillingStage Stage = DrillingStage.Resetting;

            public AutoDrillState()
                : base(new List<Property<AutoDrillState>>(
                new []{
                    new Property<AutoDrillState>("IsWorking", s => s.IsWorking, (s, v) => s.IsWorking = bool.Parse(v)),
                    new Property<AutoDrillState>("Stage", s => s.Stage, (s, v) => s.Stage = (DrillingStage)Enum.Parse(typeof(DrillingStage), v)),
                }))

            {
            }
        }
    }
}
