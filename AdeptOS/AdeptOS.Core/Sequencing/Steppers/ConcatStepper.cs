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
        /// Combines N sequences. Executes first sequence steps in order, then second sequence steps in order, and so on, then completes.
        /// </summary>
        public class ConcatStepper : IStepper
        {
            private IStepper[] _parts;
            private int _current = 0;
            private int _stepped = 0;
            private bool _done;

            public ConcatStepper(params IStepper[] parts)
            {
                _parts = parts;
            }

            public SequenceStep Next()
            {
                if (_current >= _parts.Length)
                    _done = true;

                if (IsComplete())
                    return null;


                while (_parts[_current].IsComplete())
                {
                    _current++;

                    if (_current >= _parts.Length)
                    {
                        _done = true;
                        return null;
                    }
                }

                _stepped++;
                return _parts[_current].Next();
            }

            public bool IsComplete() => _done;

            public void Reset()
            {
                foreach (var x in _parts)
                    x.Reset();

                _current = 0;
                _stepped = 0;
                _done = false;
            }

            public IStepper Clone() => new ConcatStepper(_parts.Select(x => x.Clone()).ToArray());

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">CON \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).AppendLine(Tracer.Steps(_stepped, IsComplete()));
                s.Append(tab).AppendLine(string.Join("-", _parts.Select(x => x.IsComplete() ? "[X]" : "[O]")));
                foreach (var parallel in _parts)
                {
                    s.Append(parallel.Trace(depth + 1));
                }

                return s.ToString();
            }

            public string Name { get; set; }
        }
    }
}