using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
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
        public class Drilling : RotorDrillBaseControlObject
        {
            private IJob _checkCargoSpaceJob;

            public override void Enter()
            {
                Context.Blocks.SetDrills(true);
                var drillLayer = Context.Sequences.DrillLayer;
                if (drillLayer.IsComplete() || drillLayer.IsStarted())
                    drillLayer.Reset();

                if (Context.State.Paused)
                    drillLayer.Pause();

                drillLayer
                    .StepAll()
                    .Then(x => stateMachine.SwitchState(RotorDrillStage.FinishedLayer));

                _checkCargoSpaceJob = _checkCargoSpaceJob ?? Aos.Async.CreateJob(CheckCargoSpace);
                _checkCargoSpaceJob.Start();
            }

            private void CheckCargoSpace()
            {
                var drill = Context.Blocks.Drill;
                var inv = drill.First().GetInventory();
                if ((float)inv.CurrentVolume.ToIntSafe() / (float)inv.MaxVolume.ToIntSafe() > 0.5)
                {
                    Context.Sequences.DrillLayer.Pause();
                    _checkCargoSpaceJob.Stop();
                    Aos.Async.When(() => (float)inv.CurrentVolume.ToIntSafe() / (float)inv.MaxVolume.ToIntSafe() < 0.1 || !Context.Sequences.DrillLayer.IsPaused())
                        .Then(v =>
                        {
                            Context.Sequences.DrillLayer.Resume();
                            _checkCargoSpaceJob.Start();
                        });
                }
            }

            public override void Exit()
            {
                _checkCargoSpaceJob.Stop();
                Context.Sequences.DrillLayer.Reset();
            }

            public override IPromise<Void> Drill()
            {
                return Void.Promise();
            }

            public override IPromise<Void> Pause()
            {
                Context.Sequences.DrillLayer.Pause();
                Context.State.Paused = true;
                Context.Blocks.SetDrills(false);
                return Void.Promise();
            }

            public override IPromise<Void> Resume()
            {
                Context.Sequences.DrillLayer.Resume();
                Context.State.Paused = false;
                Context.Blocks.SetDrills(true);
                return Void.Promise();
            }

            public override IPromise<Void> Reset()
            {
                Context.Sequences.DrillLayer.Reset();
                stateMachine.SwitchState(RotorDrillStage.Rewinding);
                return Void.Promise();
            }
        }
    }
}
