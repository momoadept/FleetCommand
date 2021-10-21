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
        /// Presents a sub-sequence of a sequence as a single step.
        /// If number of steps provided, if in Once mode, completes after first run, otherwise continues untill underlying sequence is complete.
        /// </summary>
        public class SkipStepper : IStepper
        {
            private IStepper _base;
            private readonly string _tag;
            private int _steps;
            private bool _once;
            private bool _done;
            private int _stepped = 0;

            private SequenceController _internalSequence;

            public SkipStepper(IStepper @base, string tag = "multiple steps", int steps = 0, bool once = true)
            {
                _base = @base;
                _tag = tag;
                _steps = steps;
                _once = once;
                _internalSequence = new SequenceController(@base, null, false);
            }

            public SequenceStep Next()
            {
                Func<IPromise<Void>> target;
                if (_steps == 0)
                    target = () => _internalSequence.StepAll();
                else
                    target = () => _internalSequence.Step(_steps);

                _stepped++;

                return new SequenceStep()
                {
                    PromiseGenerator = () =>
                    {
                        var promise = new Promise<Void>();
                        var steps = target();
                        promise.Catch(e =>
                        {
                            if (e is SequenceStoppedException)
                                _internalSequence.Interrupt();
                        });
                        steps.Finally(() =>
                        {
                            _done = _once || (_internalSequence.IsComplete() && _internalSequence.GetException() != null);
                            promise.Resolve(new Void());
                        });

                        return promise;
                    },
                    StepTag = _tag,
                };
            }

            public bool IsComplete() => _done;

            public void Reset()
            {
                _done = false;
                _internalSequence?.Reset();
                _stepped = 0;
            }

            public IStepper New() => new SkipStepper(_base.New(), _tag, _steps, _once);

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">SKP \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).AppendLine(Tracer.Steps(_stepped - 1, IsComplete()));
                s.Append(_base.Trace(depth + 1));

                return s.ToString();
            }

            public string Name { get; set; }
        }
    }
}