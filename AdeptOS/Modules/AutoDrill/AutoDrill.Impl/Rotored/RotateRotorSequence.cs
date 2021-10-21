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
        public class RotateRotorSequence
        {
            private IMyMotorStator _rotor;
            private float _stepDeg;
            private Func<float> _speed;

            private float _startDeg;
            private float _stopDeg;

            public RotateRotorSequence(IMyMotorStator rotor, float stepDeg, Func<float> speed, int direction, float startDeg, float stopDeg)
            {
                _rotor = rotor;
                _stepDeg = stepDeg;
                _speed = speed;
                _startDeg = startDeg;
                _stopDeg = stopDeg;
            }

            private IPromise<Void> Step()
            {
                float targetLimit;

                if (_stopDeg - _startDeg > 0)
                {
                    _rotor.TargetVelocityRPM = _speed();
                    _rotor.UpperLimitDeg = _rotor.AngleDeg() + _stepDeg;
                    targetLimit = _rotor.UpperLimitDeg = Math.Min(_rotor.UpperLimitDeg, _stopDeg);
                    _rotor.LowerLimitDeg = _startDeg;
                }
                else
                {
                    _rotor.TargetVelocityRPM = -_speed();
                    _rotor.LowerLimitDeg = _rotor.AngleDeg() - _stepDeg;
                    targetLimit = _rotor.LowerLimitDeg = Math.Max(_rotor.LowerLimitDeg, _stopDeg);
                    _rotor.UpperLimitDeg = _startDeg;
                }

                _rotor.Enabled = true;
                _rotor.RotorLock = false;

                return Aos.Async
                    .When(() => _rotor.AngleDeg().AlmostEquals(targetLimit))
                    .Then(x =>
                    {
                        _rotor.RotorLock = true;
                    })
                    .Next(x => Void.Promise());
            }

            private IPromise<Void> Rewind()
            {
                var direction = _startDeg - _rotor.AngleDeg();

                if (direction < 0)
                {
                    _rotor.TargetVelocityRPM = -_speed()*5;
                    _rotor.LowerLimitDeg = _startDeg;
                }
                else
                {
                    _rotor.TargetVelocityRPM = _speed()*5;
                    _rotor.UpperLimitDeg = _startDeg;
                }

                _rotor.Enabled = true;
                _rotor.RotorLock = false;

                return Aos.Async
                    .When(() => _rotor.AngleDeg().AlmostEquals(_startDeg))
                    .Then(x =>
                    {
                        _rotor.RotorLock = true;
                    })
                    .Next(x => Void.Promise());
            }

            public IStepper Stepper()
            {
                var unit = new UnitStepper(new SequenceStep()
                {
                    StepTag = "Rotate rotor",
                    PromiseGenerator = Step,
                });

                return new CycleStepper(unit, () => !_rotor.AngleDeg().AlmostEquals(_stopDeg));
            }

            public IStepper RewindStepper()
            {
                return new UnitStepper(new SequenceStep()
                {
                    StepTag = "Rotate rotor",
                    PromiseGenerator = Rewind,
                });
            }
        }
    }
}
