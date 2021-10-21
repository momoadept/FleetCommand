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

            public void RegisterStateMachine(IStateMachineController<RotorDrillStage, RotorDrillContext> stateMachine)
            {
                this.stateMachine = stateMachine;
            }

            public abstract void Enter();
            public abstract void Exit();

            public virtual string Report()
            {
                var s = new StringBuilder();
                s.AppendLine(Context.Blocks.Rotor.Angle.ToString());
                s.AppendLine(Context.Blocks.Rotor.AngleDeg().ToString());
                s.AppendLine(Context.Blocks.Rotor.LowerLimitDeg.ToString());
                s.AppendLine(Context.Blocks.Rotor.UpperLimitDeg.ToString());
                s.AppendLine(Context.Blocks.Rotor.TargetVelocityRPM.ToString());
                s.AppendLine(stateMachine.Current.ToString());
                s.AppendLine($"Layers: {Context.State.CurrentLayer}");
                s.AppendLine("[DrillLayer]");
                s.AppendLine(Context.Sequences.DrillLayer.GetReport());
                s.AppendLine("[LowerToLayer]");
                s.AppendLine(Context.Sequences.LowerToLayer.GetReport());
                s.AppendLine("[RewindHorizontalDrill]");
                s.AppendLine(Context.Sequences.RewindHorizontalDrill.GetReport());
                s.AppendLine("[RewindRotor]");
                s.AppendLine(Context.Sequences.RewindRotor.GetReport());
                s.AppendLine("[RewindVerticalDrill]");
                s.AppendLine(Context.Sequences.RewindVerticalDrill.GetReport());

                return s.ToString();
            }

            public abstract IPromise<Void> Drill();
            public abstract IPromise<Void> Pause();
            public abstract IPromise<Void> Resume();
            public abstract IPromise<Void> Reset();
        }
    }
}
