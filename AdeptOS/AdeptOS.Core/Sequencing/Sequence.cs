﻿using Sandbox.Game.EntityComponents;
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
            :base("Sequence manually closed")
            {
                
            }
        }

        public class SequenceController: ISequenceController
        {
            protected IStepper Stepper;

            Promise<Void> _currentlyRunning;

            bool _isPaused;

            bool _isResetting;

            bool _isComplete;

            bool _isWorking;

            bool _isStarted;

            Exception _terminatedWith;

            bool _continueOnError;

            Queue<Func<bool, IPromise<Void>>> _pending = new Queue<Func<bool, IPromise<Void>>>();

            public SequenceController(IStepper stepper, bool continueOnError = false)
            {
                Stepper = stepper;
                _continueOnError = continueOnError;
            }

            void Work() =>
                Aos.Async.Delay(0, Priority.Critical).Then(x =>
                {
                    if (!CanStepNow())
                        return;

                    if (_pending.Any())
                    {
                        _isStarted = true;
                        _pending.Dequeue()(false);
                    }
                });

            IPromise<Void> AddPendingStep()
            {
                var result = new Promise<Void>();

                _pending.Enqueue(reset =>
                {
                    if (reset)
                    {
                        result.Fail(new SequenceStoppedException());
                        return Promise<Void>.FromError(new SequenceStoppedException());
                    }
                        

                    var step = Stepper.Next();
                    if (step == null)
                    {
                        _isComplete = true;
                        result.Resolve(new Void());
                        return Void.Promise();
                    }

                    _isWorking = true;

                    var promise = step.PromiseGenerator()
                        .Catch(e =>
                        {
                            _terminatedWith = e;
                            _isComplete = !_continueOnError || _isComplete;
                            result.Fail(e);
                        })
                        .Finally(() =>
                        {
                            _isWorking = false;
                            _currentlyRunning = null;
                        });

                    _currentlyRunning = promise as Promise<Void>;

                    if (Stepper.IsComplete())
                        promise
                            .Then(x => _isComplete = true);

                    promise
                        .Then(x => result.Resolve(x));

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

                    next.Then(x => Work());
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
                if (_isComplete)
                {
                    return Void.Promise();
                }

                //return StepOnce()
                //    .Next(x => Aos.Async.Delay().Next(y => StepAll()));

                var result = new Promise<Void>();

                Action doWork = null;
                doWork = () => StepOnce()
                    .Then(x => Aos.Async.Delay(0, Priority.Critical).Then(y =>
                    {
                        if (_isComplete)
                            result.Resolve(new Void());
                        else
                            doWork();
                    }));

                doWork();
                return result;
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
                _isPaused = false;
                if (_currentlyRunning != null && !_currentlyRunning.Completed)
                {
                    _currentlyRunning?.Finally(() =>
                    {
                        _currentlyRunning = null;
                        _isResetting = _isComplete = _isWorking = _isStarted = false;
                        _terminatedWith = null;
                    });
                    _currentlyRunning?.Fail(new SequenceStoppedException());

                }
                else
                    _isResetting = _isComplete = _isWorking = _isStarted = false;

                _currentlyRunning = null;

                ClosePending();
                _terminatedWith = null;
            }

            public void Interrupt() => ClosePending();

            public string GetReport()
            {
                var s = new StringBuilder();
                s.AppendLine("Sequence " + Name);
                s.AppendLine();

                s.Append("STARTED:").Append(IsStarted() ? "[X] " : "[]");
                s.Append("PAUSED: ").Append(IsPaused() ? "[X] " : "[]");
                s.Append("WORKING:").Append(_isWorking ? "[X] " : "[]");
                s.Append("HAS WORK:").Append(HasWork() ? $"[{_pending.Count}] " : "[]");
                s.Append("COMPLETE: ").Append(IsComplete() ? "[X] " : "[]");
                s.Append("RESETTING: ").Append(_isResetting ? "[X] " : "[]");
                s.Append("ERROR: ").Append(_terminatedWith != null ? $"[{_terminatedWith.Message}] " : "[]");

                s.AppendLine();
                s.AppendLine();

                s.Append(Stepper.Trace());
                return s.ToString();
            }

            public Exception GetException() => _terminatedWith;

            void ClosePending()
            {
                while (_pending.Any())
                {
                    var next = _pending.Dequeue();
                    next(true);
                }
            }

            public string Name { get; set; }
        }

    }
}
