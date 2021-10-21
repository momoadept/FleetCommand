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
        public class LcdTracer : IModule, ILcdTracer
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public string UniqueName => "LcdTracer";
            public string Alias => "Lcd Tracer";

            private Dictionary<string, IList<IMyTextPanel>> _lcdsByUnwrappedTag =
                new Dictionary<string, IList<IMyTextPanel>>();

            private Dictionary<string, IList<string>> _tracesByUnwrappedTag = new Dictionary<string, IList<string>>();

            private IGameContext _context;

            public LcdTracer()
            {
                Actions.Add( "LogLastTraces", new ActionContract<Primitive<string>, Void>("LogLastTraces", primitive => LogLastTraces(new Tag(primitive.Value))));
            }

            public void Bind(IBindingContext context)
            {
                _context = context.RequireOne<IGameContext>();
            }

            public void Run()
            {
            }

            public void OnSaving()
            {
            }

            public void SetTrace(Tag tag, Func<string> tracer, Priority updatePriority = Priority.Routine)
            {
            }

            public void RemoveTrace(Tag tag)
            {
            }

            public IPromise<Void> LogLastTraces(Tag tag)
            {

                return Void.Promise();
            }
        }
    }
}