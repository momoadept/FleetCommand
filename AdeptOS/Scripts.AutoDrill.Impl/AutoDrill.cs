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
        public class AutoDrill: IAutoDrill, IModule
        {
            private AutoDrillState _state;
            private IGameContext _gameContext;
            private ILog _log;

            private IJob _workDrill;
            private IJob _checkOverflow;

            private IMyPistonBase _verticalPiston;
            private IMyPistonBase _horizontalPiston;
            private IMyShipDrill _drill;
            private float baseSpeed = 0.25f;
            private float baseStep = 2f;

            private bool _transitionHandled = false;
            private bool valid = true;

            private bool _isForward = true;

            public IPromise<Void> Start()
            {
                if (_workDrill == null)
                    return Void.Promise();

                _state.IsWorking = true;
                _workDrill.Start();

                if (_state.Stage == DrillingStage.Ready)
                {
                    Transition(DrillingStage.DrillForwards);
                }

                _checkOverflow.Start();
                return Void.Promise();
            }

            public IPromise<Void> Stop()
            {
                if (_workDrill == null)
                    return Void.Promise();

                _verticalPiston.Enabled = false;
                _horizontalPiston.Enabled = false;
                _drill.Enabled = false;

                _state.IsWorking = false;
                _transitionHandled = false;
                _workDrill.Stop();
                return Void.Promise();
            }

            public string UniqueName { get; }
            public string Alias { get; }

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
                _log = context.RequireOne<ILog>(this);
            }

            public void Run()
            {
                DetectBlocks();

                _verticalPiston.Enabled = false;
                _horizontalPiston.Enabled = false;
                _drill.Enabled = false;

                _workDrill = Aos.Async.CreateJob(WorkDrill);
                if (_state.IsWorking) 
                    _workDrill.Start();

                (_checkOverflow = Aos.Async.CreateJob(HandleOverflow)).Start();
            }

            public void OnSaving()
            {
            }

            public void Restore(AutoDrillState state)
            {
                _state = state;
                _log.Debug("restored drill state:", _state.Stringify());
            }

            public AutoDrillState GetState() => _state;

            private void DetectBlocks()
            {
                var verticalTag = new Tag("AD_V"); // [AD_V] huipizda
                var horizontalTag = new Tag("AD_H");
                var drillTag = new Tag("AD_D");

                var blocks = new List<IMyTerminalBlock>();
                _gameContext.Grid.SearchBlocksOfName(verticalTag.Wrapped, blocks, it => it is IMyPistonBase);
                _verticalPiston = blocks.FirstOrDefault() as IMyPistonBase;
                blocks.Clear();

                _gameContext.Grid.SearchBlocksOfName(horizontalTag.Wrapped, blocks, it => it is IMyPistonBase);
                _horizontalPiston = blocks.FirstOrDefault() as IMyPistonBase;
                blocks.Clear();

                _gameContext.Grid.SearchBlocksOfName(drillTag.Wrapped, blocks, it => it is IMyShipDrill);
                _drill = blocks.FirstOrDefault() as IMyShipDrill;
                blocks.Clear();

                if (_verticalPiston == null || _horizontalPiston == null || _drill == null)
                {
                    _log.Error("No drill found for auto drill");
                    valid = false;
                    return;
                }

                valid = true;
            }

            private void HandleOverflow()
            {
                var inventory = _drill.GetInventory();

                var volume = (float)inventory.CurrentVolume.ToIntSafe() / (float)inventory.MaxVolume.ToIntSafe();

                if (volume >= 0.5)
                {
                    Stop();
                }

                if (volume <= 0.01)
                {
                    Start();
                }
            }

            private void WorkDrill()
            {
                if (!valid)
                {
                    return;
                }

                if (!_transitionHandled)
                {
                    _transitionHandled = true;
                    GotoStage(_state.Stage);
                }
            }

            private void GotoStage(DrillingStage stage)
            {
                _log.Debug("Current stage:", stage.ToString());
                switch (stage)
                {
                    case DrillingStage.DrillForwards:
                        _verticalPiston.Enabled = false;
                        _drill.Enabled = true;
                        _horizontalPiston.Velocity = baseSpeed;
                        _horizontalPiston.Enabled = true;
                        Aos.Async
                            .When(() => IsPistonExpanded(_horizontalPiston))
                            .Then(ms =>
                            {
                                TerminateIfNotWorking();
                                _isForward = false;
                                _horizontalPiston.Enabled = false;
                                Transition(DrillingStage.LowerPiston);
                            })
                            .Catch(e =>
                            {
                                _log.Warning(e.Message);
                            });
                        break;

                    case DrillingStage.LowerPiston:
                        _horizontalPiston.Enabled = false;
                        if (_verticalPiston.MaxLimit >= _verticalPiston.HighestPosition)
                        {
                            Transition(DrillingStage.Done);
                            break;
                        }
                        _verticalPiston.MaxLimit += baseStep;
                        _verticalPiston.Velocity = baseSpeed;
                        _verticalPiston.Enabled = true;
                        Aos.Async
                            .When(() => IsPistonExpanded(_verticalPiston))
                            .Then(ms =>
                            {
                                TerminateIfNotWorking();
                                _verticalPiston.Enabled = false;
                                if (_isForward)
                                    Transition(DrillingStage.DrillForwards);
                                else
                                    Transition(DrillingStage.DrillBackwards);
                            })
                            .Catch(e =>
                            {
                                _log.Warning(e.Message);
                            });
                        break;

                    case DrillingStage.DrillBackwards:
                        _verticalPiston.Enabled = false;
                        _drill.Enabled = true;
                        _horizontalPiston.Velocity = -baseSpeed;
                        _horizontalPiston.Enabled = true;
                        Aos.Async
                            .When(() => IsPistonContracted(_horizontalPiston))
                            .Then(ms =>
                            {
                                TerminateIfNotWorking();
                                _isForward = true;
                                _horizontalPiston.Enabled = false;
                                Transition(DrillingStage.LowerPiston);
                            })
                            .Catch(e =>
                            {
                                _log.Warning(e.Message);
                            });
                        break;

                    case DrillingStage.Resetting:
                        _checkOverflow.Stop();
                        DoResetting();
                        break;

                    case DrillingStage.Ready:
                        _verticalPiston.Enabled = false;
                        _horizontalPiston.Enabled = false;
                        _drill.Enabled = false;
                        _verticalPiston.MaxLimit = 0;
                        _isForward = true;

                        break;
                    case DrillingStage.Done:
                        _log.Info("Drilling routine complete, please rebuild your drills");
                        Transition(DrillingStage.Resetting);
                        break;
                    default:
                        break;
                }
            }

            private void DoResetting()
            {
                _log.Debug("Resetting drill");

                _drill.Enabled = true;
                _verticalPiston.Velocity = -baseSpeed*5;
                _verticalPiston.Enabled = true;
                Aos.Async
                    .When(() => IsPistonContracted(_verticalPiston))
                    .Next((ms) =>
                    {
                        TerminateIfNotWorking();
                        _verticalPiston.Enabled = false;
                        _horizontalPiston.Velocity = -baseSpeed*5;
                        _horizontalPiston.Enabled = true;

                        return Aos.Async.When(() => IsPistonContracted(_horizontalPiston));
                    })
                    .Then(ms =>
                    {
                        TerminateIfNotWorking();
                        _log.Debug("Drill reset");
                        _drill.Enabled = false;
                        _horizontalPiston.Enabled = false;
                        Transition(DrillingStage.Ready);
                    })
                    .Catch(e =>
                    {
                        _log.Warning(e.Message);
                    });
            }

            private bool IsPistonContracted(IMyPistonBase piston) =>
                Math.Abs(piston.CurrentPosition - piston.LowestPosition) < 0.01;

            private bool IsPistonExpanded(IMyPistonBase piston) =>
                Math.Abs(piston.CurrentPosition - piston.MaxLimit) < 0.01;

            private void TerminateIfNotWorking()
            {
                if (!_state.IsWorking)
                {
                    throw new Exception("drill stopped");
                }
            }

            private void Transition(DrillingStage stage)
            {
                Aos.Async.Delay()
                    .Then(ms =>
                    {
                        _state.Stage = stage;
                        _transitionHandled = false;
                    });
            }
        }
    }
}
