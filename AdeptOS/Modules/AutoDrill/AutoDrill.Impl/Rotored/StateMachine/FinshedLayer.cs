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
        public class FinshedLayer : RotorDrillBaseControlObject
        {
            public override void Enter()
            {
                if (Context.Sequences.LowerToLayer.IsComplete())
                {
                    stateMachine.SwitchState(RotorDrillStage.FinishedAll);
                    return;
                }

                Context.State.CurrentLayer++;
                Context.Sequences.RewindRotor.Reset();
                Context.Sequences.RewindRotor.StepAll()
                    .Next(x => Context.Sequences.LowerToLayer.StepOnce())
                    .Then(v => stateMachine.SwitchState(RotorDrillStage.Drilling));
            }

            public override void Exit()
            {
            }

            public override IPromise<Void> Drill() => Void.Promise();

            public override IPromise<Void> Pause()
            {
                Context.Sequences.LowerToLayer.Pause();
                Context.State.Paused = true;
                return Void.Promise();
            }

            public override IPromise<Void> Resume()
            {
                Context.Sequences.LowerToLayer.Resume();
                Context.State.Paused = false;
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
