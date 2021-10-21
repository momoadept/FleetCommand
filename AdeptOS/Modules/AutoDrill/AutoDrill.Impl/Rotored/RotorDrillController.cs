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
        public class RotorDrillController : StorableModule<RotorDrillController>, IControllable, IRotorDrill
        {
            public RotorDrillController() : base(new List<Property<RotorDrillController>>(new []
            {
                new Property<RotorDrillController>("state", s => s._state, (s, v) => s._state.Restore(v))
            }))
            {
                Actions
                    .Add<Void, Void>("Drill", v => Drill())
                    .Add<Void, Void>("Pause", v => Pause())
                    .Add<Void, Void>("Reset", v => Reset())
                    .Add<Void, Void>("Resume", v => Resume())
                    .Add<Primitive<int>, Void>("SkipToLayer", l => SkipToLayer(l.Value));

                _impl = new RotorDrill();
            }

            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public override string UniqueName => "DRILL";
            public override string Alias => "Auto Drill";

            private RotorDrill _impl;
            private RotorDrillState _state = new RotorDrillState();

            public override void Bind(IBindingContext context)
            {
                _impl.Bind(context);
            }

            public override void Run()
            {
                _impl.SetState(_state);
                _impl.Run();
            }

            public override void OnSaving()
            {
                _state = _impl.GetState();
            }

            //T|DRILL.Start|{}
            public IPromise<Void> Drill() => _impl.Drill();

            public IPromise<Void> Pause() => _impl.Pause();

            public IPromise<Void> Resume() => _impl.Resume();

            public IPromise<Void> Reset() => _impl.Reset();

            public IPromise<Void> SkipToLayer(int layer) => _impl.SkipToLayer(layer);
        }
    }
}
