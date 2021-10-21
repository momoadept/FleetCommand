﻿using Sandbox.Game.EntityComponents;
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
        public class Rewinding : RotorDrillBaseControlObject
        {
            public override void Enter()
            {
                Context.Sequences.RewindVerticalDrill.Reset();
                Context.Sequences.RewindRotor.Reset();
                Context.Sequences.RewindHorizontalDrill.Reset();
                Context.Sequences.LowerToLayer.Reset();
                Context.Sequences.DrillLayer.Reset();
                Context.State.CurrentLayer = 0;

                Context.Sequences.RewindVerticalDrill.StepAll()
                    .Next(v =>
                        Context.Sequences.RewindHorizontalDrill.StepAll())
                    .Next(v =>
                        Context.Sequences.RewindRotor.StepAll())
                    .Then(v =>
                    {
                        Context.Blocks.Drill.Enabled = false;
                        Context.Blocks.Rotor.RotorLock = true;
                        stateMachine.SwitchState(RotorDrillStage.StartingPosition);
                    });
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
                Context.Sequences.RewindVerticalDrill.Pause();
                Context.Sequences.RewindHorizontalDrill.Pause();
                Context.Sequences.RewindRotor.Pause();
                return Void.Promise();
            }

            public override IPromise<Void> Resume()
            {
                Context.Sequences.RewindVerticalDrill.Resume();
                Context.Sequences.RewindHorizontalDrill.Resume();
                Context.Sequences.RewindRotor.Resume();
                return Void.Promise();
            }

            public override IPromise<Void> Reset()
            {
                return Void.Promise();
            }
        }
    }
}
