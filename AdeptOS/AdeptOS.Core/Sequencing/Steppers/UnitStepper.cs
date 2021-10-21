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
        /// <summary>
        /// Presents a sequence of a single step, that completes after this step
        /// </summary>
        public class UnitStepper : IStepper
        {
            private bool _done = false;
            private SequenceStep _step;

            public UnitStepper(SequenceStep step)
            {
                _step = step;
            }

            public SequenceStep Next()
            {
                if (_done)
                    return null;

                _done = true;
                return _step;
            }

            public bool IsComplete() => _done;

            public void Reset() => _done = false;
            public IStepper New() => new UnitStepper(_step);

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">UNT \"{Name}\"";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name).Append(" ").Append(_done ? "X" : "O");

                return s.ToString();
            }

            public string Name { get; set; }
        }

    }
}