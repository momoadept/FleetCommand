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
        public class AutoDrillController : StorableModule<AutoDrillController>, IControllable, IAutoDrill
        {
            public AutoDrillController() : base(new List<Property<AutoDrillController>>(new []
            {
                new Property<AutoDrillController>("state", s => s._state, (s, v) => s._state.Restore(v))
            }))
            {
                Actions
                    .Add<Void, Void>("Start", v => Start())
                    .Add<Void, Void>("Stop", v => Stop());

                _impl = new AutoDrill();
            }

            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public override string UniqueName => "DRILL";
            public override string Alias => "Auto Drill";

            private AutoDrill _impl;
            private AutoDrillState _state = new AutoDrillState();

            public override void Bind(IBindingContext context)
            {
                _impl.Bind(context);
            }

            public override void Run()
            {
                _impl.Restore(_state);
                _impl.Run();
            }

            public override void OnSaving()
            {
                _state = _impl.GetState();
            }

            //T|DRILL.Start|{}
            public IPromise<Void> Start() => _impl.Start();

            public IPromise<Void> Stop() => _impl.Stop();
        }
    }
}
