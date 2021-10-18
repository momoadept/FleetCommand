using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public class SequenceStoppedException : Exception
        {
            public SequenceStoppedException()
            :base("Sequence terminated")
            {
                
            }
        }

        public class StepSequence: ISequence
        {
            protected IStepper Stepper;

            private Promise<Void> _currentlyRunning;
            private string _currentlyRunningTag;

            private bool _isPaused;

            private bool _isResetting;

            private bool _isComplete;

            private bool _isWorking;

            private bool _isStarted;

            private Exception _terminatedWith;

            private Queue<Func<bool, IPromise<Void>>> _pending = new Queue<Func<bool, IPromise<Void>>>();

            public StepSequence(IStepper stepper)
            {
                Stepper = stepper;
            }

            public StepSequence Extend(Func<IStepper, IStepper> extend) => new StepSequence(extend(Stepper));

            private void Work()
            {
                Aos.Async.Delay().Then(x =>
                {
                    if (!CanStepNow())
                        return;

                    _isStarted = true;
                    if (_pending.Any())
                        _pending.Dequeue()(false);
                });
            }

            private IPromise<Void> AddPendingStep()
            {
                var result = new Promise<Void>();

                _pending.Enqueue(reset =>
                {
                    if (reset)
                        return Promise<Void>.FromError(new SequenceStoppedException());

                    _isWorking = true;

                    var step = Stepper.Next();
                    var promise = step.PromiseGenerator()
                        .Then(x => result.Resolve(x))
                        .Catch(e =>
                        {
                            _terminatedWith = e;
                            _isComplete = true;
                            result.Fail(e);
                        })
                        .Finally(() =>
                        {
                            _isWorking = false;
                            _currentlyRunning = null;
                            _currentlyRunningTag = null;
                        });

                    _currentlyRunning = promise as Promise<Void>;
                    _currentlyRunningTag = step.StepTag;

                    if (Stepper.IsComplete())
                        promise
                            .Then(x => _isComplete = true);

                    Work();

                    return promise;
                });

                Work();

                return result;
            }

            public IPromise<Void> StepOnce()
            {
                return AddPendingStep();
            }

            public IPromise<Void> StepAll()
            {
                if (_isComplete)
                    return Void.Promise();

                return StepOnce()
                    .Next(x => StepAll());
            }

            public bool IsComplete() => _isComplete;

            public bool HasWork() => !_isComplete && !_pending.Any();

            public bool IsStepInProgress() => _isWorking;

            public bool CanStepNow() => !_isWorking && !_isResetting && !_isPaused;

            public bool IsStarted() => _isStarted;

            public bool IsPaused() => _isPaused;

            public void Pause() => _isPaused = true;

            public void Resume()
            {
                _isPaused = false;
                Work();
            }

            public void Reset()
            {
                _isResetting = true;
                _isStarted = false;
                Stepper.Reset();
                _currentlyRunningTag = null;
                _isPaused = false;
                if (_currentlyRunning != null)
                    _currentlyRunning.Finally(() =>
                    {
                        _currentlyRunning = null;
                        _isResetting = false;
                    });
                else
                    _isResetting = false;

                ClosePending();
            }

            public void Interrupt()
            {
                ClosePending();
            }

            public string GetReport()
            {
                var s = new StringBuilder();
                if (_isPaused)
                    s.AppendLine("PAUSED");

                if (_isResetting)
                    s.AppendLine("RESETTING");

                if (_isWorking)
                    s.AppendLine("WORKING");

                if (_isComplete)
                    s.AppendLine("COMPLETE");

                if (HasWork())
                    s.AppendLine("HAS WORK");

                if (!IsStarted())
                    s.AppendLine("NOT STARTED");

                if (_terminatedWith != null)
                    s.AppendLine("Last exception:")
                        .AppendLine(_terminatedWith.Message);

                if (_currentlyRunning != null)
                    s.AppendLine($"Current step: {_currentlyRunningTag}");

                s.AppendLine($"Pending steps: {_pending.Count}");

                return s.ToString();
            }

            public Exception GetException()
            {
                return _terminatedWith;
            }

            private void ClosePending()
            {
                while (_pending.Any())
                {
                    var next = _pending.Dequeue();
                    next(true);
                }
            }
        }

    }
}
