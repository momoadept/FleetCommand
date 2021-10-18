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
        public class SequenceStep
        {
            public Func<IPromise<Void>> PromiseGenerator;
            public string StepTag;
        }

        public interface IStepper
        {
            SequenceStep Next();

            bool IsComplete();

            void Reset();
        }

        public class ListStepper : IStepper
        {
            protected IList<SequenceStep> Steps;
            protected int next = 0;

            public ListStepper(IList<SequenceStep> steps)
            {
                Steps = steps;
            }

            public SequenceStep Next()
            {
                next++;

                if (next < Steps.Count)
                    return Steps[next];

                return null;
            }

            public bool IsComplete() => next >= Steps.Count;

            public void Reset() => next = 0;
        }

        public class CombinedStepper : IStepper
        {
            private IStepper _a;
            private IStepper _b;

            public CombinedStepper(IStepper a, IStepper b)
            {
                _a = a;
                _b = b;
            }

            public SequenceStep Next() => _a.IsComplete() ? _b.IsComplete() ? null : _b.Next() : _a.Next();

            public bool IsComplete() => _a.IsComplete() && _b.IsComplete();

            public void Reset()
            {
                _a.Reset();
                _b.Reset();
            }
        }

        public class InterruptingStepper : IStepper
        {
            private IStepper _main;
            private IStepper _interruptor;
            private bool _interruptNow = true;

            public InterruptingStepper(IStepper main, IStepper interruptor)
            {
                _main = main;
                _interruptor = interruptor;
            }

            public SequenceStep Next()
            {
                if (_main.IsComplete() && _interruptNow)
                    return null;
                    
                _interruptNow = !_interruptNow;

                return _interruptNow ? _interruptor.Next() : _main.Next();
            }

            public bool IsComplete() => _main.IsComplete() && _interruptor.IsComplete();

            public void Reset()
            {
                _main.Reset();
                _interruptor.Reset();
                _interruptNow = true;
            }
        }

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
        }

        public class CycleStepper : IStepper
        {
            private IStepper _body;
            private Func<bool> _while;
            private bool _done;
            private bool _first = true;

            public CycleStepper(IStepper body, Func<bool> @while)
            {
                _body = body;
                _while = @while;
            }

            public SequenceStep Next()
            {
                if (_done)
                    return null;

                if (_first && !_while())
                {
                    _first = false;
                    _done = true;
                    return null;
                }

                _first = false;
                var result = _body.Next();

                if (_body.IsComplete())
                {
                    if (_while())
                        _body.Reset();
                    else
                        _done = true;
                }

                return result;
            }

            public bool IsComplete() => _done;

            public void Reset()
            {
                _done = false;
                _first = true;
                _body.Reset();
            }
        }

        public class ParallelStepper : IStepper
        {
            private bool _asyncMode;
            private IStepper[] _parallels;
            private int _current = 0;

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
                        _current++;

                    return _parallels[_current++].Next();
                }

                var items = _parallels.Select(x => x.Next()).Where(x => x != null).ToArray();
                var tag = $"Parallel: {string.Join(", ", items.Select(x => x.StepTag))}";
                Func<IPromise<Void>> runAll = () => Promise<Void>
                    .Synch(items.Select(x => x.PromiseGenerator()).ToArray())
                    .Next(x => Void.Promise());

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
            }
        }
    }
}
