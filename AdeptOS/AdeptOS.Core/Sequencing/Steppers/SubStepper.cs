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
        /// Presents a sub-sequence of a different sequence. Completes after a set number of steps.
        /// If resetParent is true, will reset underlying sequence if is reset itself.
        /// Otherwise, when reset, will continue underlying sequence from where it left for another set number of steps, and can complete earlier if underlying sequence is complete. Will still reset underlying sequence if completed
        /// </summary>
        public class SubStepper : IStepper
        {
            private IStepper _base;
            private int _steps;
            private int _stepped;
            private bool _resetParent;

            public SubStepper(IStepper @base, int steps, bool resetParent = true)
            {
                _base = @base;
                _steps = steps;
                _resetParent = resetParent;
            }

            public SequenceStep Next()
            {
                if (IsComplete())
                    return null;

                _stepped++;
                return _base.Next();
            }

            public bool IsComplete() => _stepped >= _steps || _base.IsComplete();

            public void Reset()
            {
                _stepped = 0;
                if (_base.IsComplete() || _resetParent)
                    _base.Reset();
            }

            public IStepper Clone() => new SubStepper(_base.Clone(), _steps, _resetParent);

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">SUB \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).AppendLine(Tracer.Steps(_stepped - 1, IsComplete(), _steps));
                s.Append(_base.Trace(depth + 1));

                return s.ToString();
            }

            public string Name { get; set; }
        }
    }
}