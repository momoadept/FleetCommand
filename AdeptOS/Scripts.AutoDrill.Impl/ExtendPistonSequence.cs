using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public class ExtendPiston
        {
            private IMyPistonBase _piston;
            private float _speed;
            private float _distanceEnd;
            private float _pistonStep;
            private string _prefix;

            public ExtendPiston(IMyPistonBase piston, float speed, string prefix, float distanceEnd = 10f, float step = 1f)
            {
                _piston = piston;
                _speed = speed;
                _prefix = prefix;
                _distanceEnd = distanceEnd;
                _pistonStep = step;
            }

            private IPromise<Void> Step()
            {
                _piston.MaxLimit = _piston.CurrentPosition + _pistonStep;
                _piston.MaxLimit = Math.Min(_piston.MaxLimit, _distanceEnd);
                _piston.Velocity = _speed;
                _piston.Enabled = true;

                return Aos.Async
                    .When(() => _piston.CurrentPosition.AlmostEquals(_piston.MaxLimit))
                    .Then(x => _piston.Velocity = 0)
                    .Next(x => Void.Promise());
            }

            private bool IsFullyExtended() => _piston.MaxLimit >= _distanceEnd ||
                                              _piston.MaxLimit.AlmostEquals(_piston.HighestPosition);

            public StepSequence Sequence()
            {
                var stepper = new UnitStepper(new SequenceStep()
                {
                    PromiseGenerator = Step,
                    StepTag = _prefix + " Piston extend step",
                });

                var cycle = new CycleStepper(stepper, () => !IsFullyExtended());

                return new StepSequence(cycle);
            }
        }
    }
}
