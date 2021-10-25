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
        public abstract class RotorDrillBaseControlObject: IAutoDrillControlObject
        {
            protected IStateMachineController<RotorDrillStage, RotorDrillContext> stateMachine;
            protected RotorDrillContext Context => stateMachine.Context;

            public void RegisterStateMachine(IStateMachineController<RotorDrillStage, RotorDrillContext> stateMachine) => this.stateMachine = stateMachine;

            public abstract void Enter();
            public abstract void Exit();

            public virtual string Report()
            {
                var s = new StringBuilder();
                s.AppendLine("Auto drill");
                s.AppendLine(stateMachine.Current.ToString());
                s.AppendLine("Saved State: ").AppendLine(Context.State.Stringify());
                s.AppendLine();
                s.AppendLine(Context.Blocks.Rotor.Angle.ToString());
                s.AppendLine(Context.Blocks.Rotor.AngleDeg().ToString());
                s.AppendLine(Context.Blocks.Rotor.LowerLimitDeg.ToString());
                s.AppendLine(Context.Blocks.Rotor.UpperLimitDeg.ToString());
                s.AppendLine(Context.Blocks.Rotor.TargetVelocityRPM.ToString());

                return s.ToString();
            }

            public virtual IPromise<Void> Drill() => Void.Promise();

            public virtual IPromise<Void> Pause() => Void.Promise();

            public virtual IPromise<Void> Resume() => Void.Promise();

            public virtual IPromise<Void> Reset() => Void.Promise();

            public virtual IPromise<Void> SkipToLayer(int layer) => Void.Promise();
        }
    }
}
