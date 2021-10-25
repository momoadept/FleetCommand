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
            IMyPistonBase _piston;
            float _speed;
            float _distanceEnd;
            float _pistonStep;
            ILog _log;
            string _prefix;

            public ExtendPiston(IMyPistonBase piston, float speed, string prefix, float distanceEnd = 10f, float step = 1f, ILog log = null)
            {
                _piston = piston;
                _speed = speed;
                _prefix = prefix;
                _distanceEnd = distanceEnd;
                _pistonStep = step;
                _log = log;
            }

            IPromise<Void> Step()
            {
                _log?.Debug("STEPPER---- extend piston step: ", _prefix);
                _piston.MaxLimit = _piston.CurrentPosition + _pistonStep;
                _piston.MaxLimit = Math.Min(_piston.MaxLimit, _distanceEnd);
                _piston.Velocity = _speed;
                _piston.Enabled = true;

                return Aos.Async
                    .When(() => _piston.CurrentPosition.AlmostEquals(_piston.MaxLimit), Priority.Critical)
                    .Then(x => _piston.Velocity = 0)
                    .Next(x => Void.Promise());
            }

            bool IsFullyExtended()
            {
                var isExtended = _piston.CurrentPosition >= _distanceEnd ||
                       _piston.CurrentPosition.AlmostEquals(_piston.HighestPosition);
                _log?.Debug("SQ---- extended: ", isExtended.ToString());
                return isExtended;
            }

            public IStepper Stepper()
            {
                var stepper = new UnitStepper(new SequenceStep()
                {
                    PromiseGenerator = Step,
                    StepTag = _prefix + " Piston extend step",
                });

                var cycle = new CycleStepper(stepper, () => !IsFullyExtended());

                return cycle;
            }

            public SequenceController Sequence() => new SequenceController(Stepper());
        }

        public class ContractPiston
        {
            IMyPistonBase _piston;
            float _speed;
            float _distanceEnd;
            float _pistonStep;
            string _prefix;

            public ContractPiston(IMyPistonBase piston, float speed, string prefix, float distanceEnd = 0f, float step = 1f)
            {
                _piston = piston;
                _speed = speed;
                _prefix = prefix;
                _distanceEnd = distanceEnd;
                _pistonStep = step;
            }

            IPromise<Void> Step()
            {
                _piston.MinLimit = _piston.CurrentPosition - _pistonStep;
                _piston.MinLimit = Math.Max(_piston.MinLimit, _distanceEnd);
                _piston.Velocity = -_speed;
                _piston.Enabled = true;

                return Aos.Async
                    .When(() => _piston.CurrentPosition.AlmostEquals(_piston.MinLimit), Priority.Critical)
                    .Then(x => _piston.Velocity = 0)
                    .Next(x => Void.Promise());
            }

            bool IsFullyRetracted()
            {
                var isExtended = _piston.CurrentPosition <= _distanceEnd ||
                       _piston.CurrentPosition.AlmostEquals(_piston.LowestPosition);
                return isExtended;
            }

            public IStepper Stepper()
            {
                var stepper = new UnitStepper(new SequenceStep()
                {
                    PromiseGenerator = Step,
                    StepTag = _prefix + " Piston contract step",
                });

                var cycle = new CycleStepper(stepper, () => !IsFullyRetracted());

                return cycle;
            }

            public SequenceController Sequence() => new SequenceController(Stepper());
        }

        public class ExtendContractPistonArm
        {
            IMyPistonBase[] _pistons;
            float _speed;
            string _prefix;
            float _distanceEnd;
            float _step;
            bool _asyncMode;
            ILog _log;

            public ExtendContractPistonArm(IMyPistonBase[] pistons, float speed, string prefix, float distanceEnd = 10f, float step = 1f, bool asyncMode = false, ILog log = null)
            {
                _pistons = pistons;
                _speed = speed;
                _prefix = prefix;
                _distanceEnd = distanceEnd;
                _step = step;
                _asyncMode = asyncMode;
                _log = log;
            }

            public IStepper Stepper()
            {
                var N = _pistons.Length;
                var baseSteppers = _pistons.Select(x => _speed > 0 
                    ? new ExtendPiston(x, _speed / (_asyncMode ? N : 1), _prefix, _distanceEnd / N, _step / (_asyncMode ? N : 1), _log).Stepper()
                    : new ContractPiston(x, -_speed / (_asyncMode ? N : 1), _prefix, _distanceEnd / N, _step / (_asyncMode ? N : 1)).Stepper());

                var combinedStepper = new ParallelStepper(_asyncMode, baseSteppers.ToArray());
                return combinedStepper;
            }

            public SequenceController Sequence() => new SequenceController(Stepper());
        }
    }
}
