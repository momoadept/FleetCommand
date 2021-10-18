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
            StartingPosition = 1,
            Working = 2,
            Done = 3,
            Rewinding = 4,
            PausedWorking = 5,
            PausedRewinding = 6,
            WaitingForCargoSpace = 7,
            Error = 8,
        }

        public class AutoDrillState : BaseDataObject<AutoDrillState>
        {
            public DrillingStage Stage = DrillingStage.Rewinding;

            public AutoDrillState()
                : base(new List<Property<AutoDrillState>>(
                new []{
                    new Property<AutoDrillState>("Stage", s => s.Stage, (s, v) => s.Stage = (DrillingStage)Enum.Parse(typeof(DrillingStage), v)),
                }))

            {
            }
        }
    }
}
