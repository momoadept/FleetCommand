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
        public class MovingToLayer : RotorDrillBaseControlObject
        {
            public override void Enter()
            {
                Context.Blocks.Rotor.RotorLock = true;
                Context.Blocks.Drill.Enabled = true;
                var lower = Context.Sequences.LowerToLayer;
                if (Context.State.CurrentLayer == 0)
                {
                    stateMachine.SwitchState(RotorDrillStage.Drilling);
                    return;
                }

                lower.Step(Context.State.CurrentLayer)
                    .Then(v => stateMachine.SwitchState(RotorDrillStage.Drilling));
            }

            public override void Exit()
            {
            }

            public override IPromise<Void> Drill()
            {
                return Void.Promise();
            }

            public override IPromise<Void> Pause()
            {
                return Void.Promise();
            }

            public override IPromise<Void> Resume()
            {
                return Void.Promise();
            }

            public override IPromise<Void> Reset()
            {
                Context.Sequences.LowerToLayer.Reset();
                stateMachine.SwitchState(RotorDrillStage.Rewinding);
                return Void.Promise();
            }
        }
    }
}
