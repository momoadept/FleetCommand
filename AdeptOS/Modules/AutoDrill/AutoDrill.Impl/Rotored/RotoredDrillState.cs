using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        public enum RotorDrillStage
        {
            StartingPosition = 1,
            Drilling = 2,
            FinishedLayer = 3,
            Rewinding = 4,
            MovingToLayer = 6,
            Error = 8,
            FinishedAll = 9,
        }

        public interface IAutoDrillControlObject : IAutoDrill, IStateMachineControlObject<RotorDrillStage, RotorDrillContext>
        {
            string Report();
        }

        public class RotorDrillStateMachine : StateMachine<IAutoDrillControlObject, RotorDrillStage, IAutoDrill, RotorDrillContext>
        {
            private ILog _log;

            public RotorDrillStateMachine(Dictionary<RotorDrillStage, IAutoDrillControlObject> controlObjects, RotorDrillStage initialState, RotorDrillContext context, ILog log) : base(controlObjects, initialState, context)
            {
                _log = log;
            }

            public override void SwitchState(RotorDrillStage next)
            {
                _log?.Debug($"Switch state: {CurrentState} -> {next} ");
                base.SwitchState(next);
                if (Context.State != null)
                    Context.State.Stage = next;
            }
        }

        public class RotorDrillState : BaseDataObject<RotorDrillState>
        {
            public RotorDrillStage Stage = RotorDrillStage.Rewinding;
            public int CurrentLayer = 0;
            public bool Paused = false;

            public RotorDrillState() 
                : base(new List<Property<RotorDrillState>>()
                {
                    new Property<RotorDrillState>("Stage", x => x.Stage, (x, v) => v.ToEnum(ref x.Stage)),
                    new Property<RotorDrillState>("CurrentLayer", x => x.CurrentLayer, (x, v) => x.CurrentLayer = int.Parse(v)),
                    new Property<RotorDrillState>("Paused", x => x.Paused, (x, v) => x.Paused = bool.Parse(v)),
                })
            {
            }
        }
    }
}
