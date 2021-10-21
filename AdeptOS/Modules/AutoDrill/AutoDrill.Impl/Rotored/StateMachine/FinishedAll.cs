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
        public class FinishedAll : RotorDrillBaseControlObject
        {
            public override void Enter()
            {
            }

            public override void Exit()
            {
            }

            public override IPromise<Void> Drill() => Reset();

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
                stateMachine.SwitchState(RotorDrillStage.Rewinding);
                return Void.Promise();
            }
        }
    }
}
