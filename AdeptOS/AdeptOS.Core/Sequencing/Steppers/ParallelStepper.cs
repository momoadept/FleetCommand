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
        /// Combines N sequences. Runs one step from each sequence in order, skipping completed sequences. Completes when all sequences are completed.
        /// In async mode, produces single step from N steps and executes them in parallel
        /// </summary>
        public class ParallelStepper : IStepper
        {
            private bool _asyncMode;
            private IStepper[] _parallels;
            private int _current = 0;
            private int _stepped = 0;

            public ParallelStepper(bool asyncMode, params IStepper[] parallels)
            {
                _asyncMode = asyncMode;
                _parallels = parallels;
            }

            public SequenceStep Next()
            {
                if (_current >= _parallels.Length)
                    _current = 0;

                if (IsComplete())
                    return null;



                if (!_asyncMode)
                {
                    while (_parallels[_current].IsComplete())
                    {
                        _current++;

                        if (_current >= _parallels.Length)
                            _current = 0;
                    }


                    _stepped++;
                    return _parallels[_current++].Next();
                }

                var items = _parallels.Select(x => x.Next()).Where(x => x != null).ToArray();
                var tag = $"Parallel: {string.Join(", ", items.Select(x => x.StepTag))}";
                Func<IPromise<Void>> runAll = () => Promise<Void>
                    .Synch(items.Select(x => x.PromiseGenerator()).ToArray())
                    .Next(x => Void.Promise());

                _stepped++;
                return new SequenceStep()
                {
                    StepTag = tag,
                    PromiseGenerator = runAll,
                };
            }

            public bool IsComplete() => _parallels.All(x => x.IsComplete());

            public void Reset()
            {
                foreach (var x in _parallels)
                    x.Reset();

                _current = 0;
                _stepped = 0;
            }

            public IStepper Clone() => new ParallelStepper(_asyncMode, _parallels.Select(x => x.Clone()).ToArray());

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">PAR \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).AppendLine(Tracer.Steps(_stepped, IsComplete()));
                s.Append(tab).AppendLine(string.Join("-", _parallels.Select(x => x.IsComplete() ? "[X]" : "[O]")));
                foreach (var parallel in _parallels)
                {
                    s.Append(parallel.Trace(depth + 1));
                }

                return s.ToString();
            }

            public string Name { get; set; }
        }
    }
}