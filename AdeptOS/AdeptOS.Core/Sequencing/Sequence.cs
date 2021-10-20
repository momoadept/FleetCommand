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
            :base("Sequence terminated before promise could resolve")
            {
                
            }
        }

        public class StepSequence: ISequence
        {
            protected IStepper Stepper;
            private ILog _log;

            private Promise<Void> _currentlyRunning;
            private string _currentlyRunningTag;

            private bool _isPaused;

            private bool _isResetting;

            private bool _isComplete;

            private bool _isWorking;

            private bool _isStarted;

            private Exception _terminatedWith;

            private bool _continueOnError;

            private Queue<Func<bool, IPromise<Void>>> _pending = new Queue<Func<bool, IPromise<Void>>>();

            private int counter = 0;

            public StepSequence(IStepper stepper, ILog _log, bool continueOnError = false)
            {
                Stepper = stepper;
                this._log = _log;
                _continueOnError = continueOnError;
            }

            public StepSequence Extend(Func<IStepper, IStepper> extend) => new StepSequence(extend(Stepper), _log, _continueOnError);

            private void Work()
            {
                Aos.Async.Delay(0, Priority.Critical).Then(x =>
                {
                    _log.Debug("SQ---- Work");
                    if (!CanStepNow())
                        return;

                    if (_pending.Any())
                    {
                        _log.Debug("SQ---- Work -> dequeue");
                        _isStarted = true;
                        _pending.Dequeue()(false);
                    }
                });
            }

            private IPromise<Void> AddPendingStep()
            {
                _log?.Debug("SQ---- add pending step");
                var result = new Promise<Void>();

                _pending.Enqueue(reset =>
                {
                    _log?.Debug("SQ---- enqueue");
                    if (reset)
                    {
                        result.Fail(new SequenceStoppedException());
                        return Promise<Void>.FromError(new SequenceStoppedException());
                    }
                        

                    var step = Stepper.Next();
                    if (step == null)
                    {
                        _log?.Debug("SQ---- stepper empty");
                        _isComplete = true;
                        result.Resolve(new Void());
                        return Void.Promise();
                    }
                    _log?.Debug("SQ---- step received");
                    _log?.Debug($"{++counter} {step.StepTag}");

                    _isWorking = true;

                    var promise = step.PromiseGenerator()
                        .Catch(e =>
                        {
                            _log.Debug($"{counter} step failed");
                            _terminatedWith = e;
                            _isComplete = !_continueOnError || _isComplete;
                            result.Fail(e);
                        })
                        .Finally(() =>
                        {
                            _log.Debug($"{counter} step completed");
                            _isWorking = false;
                            _currentlyRunning = null;
                            _currentlyRunningTag = null;
                        });

                    _currentlyRunning = promise as Promise<Void>;
                    _currentlyRunningTag = step.StepTag;

                    if (Stepper.IsComplete())
                        promise
                            .Then(x =>
                            {
                                _log.Debug($"{counter} step resolved and last");
                                _isComplete = true;
                            });

                    promise
                        .Then(x =>
                        {
                            _log.Debug($"{counter} step resolved");
                            result.Resolve(x);
                        });

                    return promise;
                });

                //result.Resolve(new Void());
                return result;
            }

            public IPromise<Void> StepOnce()
            {
                Work();
                return AddPendingStep();
            }

            public IPromise<Void> Step(int times)
            {
                var promises = new List<IPromise<Void>>();
                var result = new Promise<Void>();
                for (int i = 0; i < times; i++)
                {
                    var next = StepOnce();

                    promises.Add(next);
                }

                Promise<Void>.Synch(promises.ToArray())
                    .Then(sync =>
                    {
                        if (sync.Errors.Any())
                            result.Fail(sync.Errors[0]);
                        else
                            result.Resolve(new Void());
                    });

                return result;
            }

            public IPromise<Void> StepAll()
            {
                _log.Debug("SQ---- step all");
                if (_isComplete)
                {
                    _log.Debug("SQ---- step all -> complete");
                    return Void.Promise();
                }

                _log.Debug("SQ---- step all -> step once");
                return StepOnce()
                    .Next(x => Aos.Async.Delay().Next(y => StepAll()));
            }

            public bool IsComplete() => _isComplete;

            public bool HasWork() => !_isComplete && _pending.Any();

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
                Stepper?.Reset();
                _log.Debug("Stepper?.Reset()");
                _currentlyRunningTag = null;
                _isPaused = false;
                if (_currentlyRunning != null && !_currentlyRunning.Completed)
                {
                    _log.Debug("_currentlyRunning != null");
                    _currentlyRunning?.Finally(() =>
                    {
                        _log.Debug("_currentlyRunning finally");
                        _currentlyRunning = null;
                        _isResetting = _isComplete = _isWorking = _isStarted = false;
                        _terminatedWith = null;
                    });
                    _currentlyRunning?.Fail(new SequenceStoppedException());

                    _log.Debug("_currentlyRunning things");
                }
                else
                    _isResetting = _isComplete = _isWorking = _isStarted = false;

                _currentlyRunning = null;

                ClosePending();
                _log.Debug("ClosePending");
                _terminatedWith = null;
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
                    s.AppendLine($"Last exception: {_terminatedWith.Message}");

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
