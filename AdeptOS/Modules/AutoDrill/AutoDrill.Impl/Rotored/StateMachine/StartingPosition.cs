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
        public class StartingPosition : RotorDrillBaseControlObject
        {
            public override IPromise<Void> Drill()
            {
                stateMachine.SwitchState(RotorDrillStage.MovingToLayer);
                return Void.Promise();
            }

            public override IPromise<Void> Pause() => Void.Promise();

            public override IPromise<Void> Resume() => Void.Promise();

            public override IPromise<Void> Reset()
            {
                Context.State.CurrentLayer = 0;
                stateMachine.SwitchState(RotorDrillStage.Rewinding);
                return Void.Promise();
            }

            public override IPromise<Void> SkipToLayer(int layer)
            {
                Context.State.CurrentLayer = layer;
                return Void.Promise();
            }

            public override void Enter()
            {
                if (!Context.Blocks.Valid) return;

                Context.Blocks.SetDrills(false);
                Context.Blocks.Rotor.RotorLock = true;
            }

            public override void Exit()
            {
            }
        }
    }
}
