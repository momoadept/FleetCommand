using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
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
            AutoDrillState _state;
            IGameContext _gameContext;
            ILog _log;


            List<IMyPistonBase> _horizontalPistonArm = new List<IMyPistonBase>();
            List<IMyPistonBase> _verticalPistonArm = new List<IMyPistonBase>();
            IMyShipDrill _drill;
            IMyTextPanel _reportLcd;

            SequenceController _dig;
            SequenceController _reset;

            IJob _checkCargoSpace;
            IJob _waitForCargoSpace;

            bool valid;

            public IPromise<Void> Drill()
            {
                switch (_state.Stage)
                {
                    case DrillingStage.StartingPosition:
                        _reset.Reset();
                        _dig.StepAll()
                            .Then(x =>
                            {
                                _state.Stage = DrillingStage.Done;
                                _checkCargoSpace.Stop();
                                _waitForCargoSpace.Stop();
                            });
                        _checkCargoSpace.Start();
                        _state.Stage = DrillingStage.Working;
                        break;
                    case DrillingStage.Working:
                        break;
                    case DrillingStage.Done:
                        _dig.Reset();
                        _state.Stage = DrillingStage.Rewinding;
                        _reset.StepAll()
                            .Then(x => _state.Stage = DrillingStage.StartingPosition);
                        break;
                    case DrillingStage.Rewinding:
                        break;
                    case DrillingStage.PausedWorking:
                        _state.Stage = DrillingStage.Working;
                        _drill.Enabled = true;
                        _dig.Resume();
                        break;
                    case DrillingStage.PausedRewinding:
                        _state.Stage = DrillingStage.Rewinding;
                        _drill.Enabled = true;
                        _reset.Resume();
                        break;
                    case DrillingStage.WaitingForCargoSpace:
                        break;
                    case DrillingStage.Error:
                        CheckGrid();
                        if (valid)
                            _state.Stage = DrillingStage.Done;
                        break;
                }

                return Void.Promise();
            }

            public IPromise<Void> Pause()
            {
                switch (_state.Stage)
                {
                    case DrillingStage.StartingPosition:
                        break;
                    case DrillingStage.Working:
                        _dig.Pause();
                        _drill.Enabled = false;
                        _state.Stage = DrillingStage.PausedWorking;
                        break;
                    case DrillingStage.Done:
                        break;
                    case DrillingStage.Rewinding:
                        _reset.Pause();
                        _drill.Enabled = false;
                        _state.Stage = DrillingStage.PausedRewinding;
                        break;
                    case DrillingStage.PausedWorking:
                        break;
                    case DrillingStage.PausedRewinding:
                        break;
                    case DrillingStage.WaitingForCargoSpace:
                        break;
                    case DrillingStage.Error:
                        break;
                }

                return Void.Promise();
            }

            public IPromise<Void> Resume() => Drill();

            public IPromise<Void> Reset()
            {
                var state = _state.Stage;
                _state.Stage = DrillingStage.Rewinding;

                switch (state)
                {
                    case DrillingStage.StartingPosition:
                    case DrillingStage.Working:
                    case DrillingStage.Done:
                    case DrillingStage.PausedWorking:
                    case DrillingStage.PausedRewinding:
                    case DrillingStage.WaitingForCargoSpace:
                    case DrillingStage.Rewinding:
                    case DrillingStage.Error:
                        //_reset.Reset();
                        _reset.StepAll()
                            .Then(x => _state.Stage = DrillingStage.StartingPosition)
                            .Then(x => _reset.Reset());
                        _dig.Reset();
                        _checkCargoSpace.Stop();
                        _waitForCargoSpace.Stop(); 
                        break;
                }

                return Void.Promise();
            }

            public string UniqueName { get; }
            public string Alias { get; }

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>();
                _log = context.RequireOne<ILog>();
            }

            public void Run()
            {
                CheckGrid();
                Aos.Async.CreateJob(Report).Start();

                _checkCargoSpace = Aos.Async.CreateJob(CheckCargoSpace);
                _waitForCargoSpace = Aos.Async.CreateJob(ContinueWithCargoSpace);

                switch (_state.Stage)
                {
                    case DrillingStage.StartingPosition:
                        break;
                    case DrillingStage.Working:
                        _reset.Reset();
                        _dig.StepAll();
                        _checkCargoSpace.Start();
                        break;
                    case DrillingStage.Done:
                        break;
                    case DrillingStage.Rewinding:
                        _dig.Reset();
                        _reset.StepAll();
                        break;
                    case DrillingStage.PausedWorking:
                        _reset.Reset();
                        _dig.StepAll();
                        _dig.Pause();
                        break;
                    case DrillingStage.PausedRewinding:
                        _dig.Reset();
                        _reset.StepAll();
                        _reset.Pause();
                        break;
                    case DrillingStage.WaitingForCargoSpace:
                        _dig.StepAll();
                        _dig.Pause();
                        _waitForCargoSpace.Start();
                        break;
                    case DrillingStage.Error:
                        _log.Error("Drill loaded with error");
                        break;
                }

                _log.Debug("drill initialized");
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

            void CheckGrid()
            {
                DetectBlocks();
                BuildSequences();
            }

            void ContinueWithCargoSpace()
            {
                var inv = _drill.GetInventory();
                if (_state.Stage != DrillingStage.WaitingForCargoSpace)
                    return;

                if ((float)inv.CurrentVolume.ToIntSafe() / (float)inv.MaxVolume.ToIntSafe() < 0.1)
                {
                    _checkCargoSpace.Start();
                    _waitForCargoSpace.Stop();

                    _dig.Resume();
                    _state.Stage = DrillingStage.Working;
                }
            }

            void CheckCargoSpace()
            {
                var inv = _drill.GetInventory();
                if ((float)inv.CurrentVolume.ToIntSafe() / (float)inv.MaxVolume.ToIntSafe() > 0.5)
                {
                    if (_state.Stage == DrillingStage.Working)
                    {
                        _dig.Pause();
                        _checkCargoSpace.Stop();
                        _waitForCargoSpace.Start();
                        _state.Stage = DrillingStage.WaitingForCargoSpace;
                    }
                }
            }

            void Report()
            {
                if (_reportLcd == null)
                    return;

                var report = _dig.GetReport().Split('\n').Reverse();
                var report2 = _reset.GetReport().Split('\n').Reverse();

                _reportLcd.WriteText($"\nStage: {_state.Stage}\n");

                _reportLcd.WriteText("\n[Digging]\n", true);
                _reportLcd.WriteText(string.Join("\n", report), true);
                _reportLcd.WriteText("\n", true);
                _reportLcd.WriteText("\n[Resetting]\n", true);
                _reportLcd.WriteText(string.Join("\n", report2), true);
                _reportLcd.WriteText("\n", true);
            }

            void DetectBlocks()
            {
                var verticalTag = new Tag("AD_V"); // [AD_V] huipizda
                var horizontalTag = new Tag("AD_H");
                var drillTag = new Tag("AD_D");
                var lcdTag = new Tag("AD_S");

                _horizontalPistonArm = FindGroupByTag<IMyPistonBase>(horizontalTag);
                _log.Debug($"{_horizontalPistonArm.Count} HORIZONTALS");
                _verticalPistonArm = FindGroupByTag<IMyPistonBase>(verticalTag);
                _log.Debug($"{_verticalPistonArm.Count} VERTICALS");
                _drill = FindBlockByTag<IMyShipDrill>(drillTag).FirstOrDefault();
                _reportLcd = FindBlockByTag<IMyTextPanel>(lcdTag).FirstOrDefault();

                valid = _verticalPistonArm.Any() && _horizontalPistonArm.Any() && _drill != null;

                if (!valid)
                {
                    _state.Stage = DrillingStage.Error;
                    _dig?.Reset();
                    _reset?.Reset();
                }
            }

            void BuildSequences()
            {
                if (_dig != null)
                    _dig.Reset();

                if (_reset != null)
                    _reset.Reset();

                var max = _horizontalPistonArm.First().MaxLimit;
                var baseSpeed = 0.25f;
                var baseStep = 1f;
                var extendVertical = new ExtendContractPistonArm(
                    _verticalPistonArm.ToArray(),
                    baseSpeed,
                    "lower drill",
                    max * _verticalPistonArm.Count,
                    baseStep,
                    false).Stepper();

                var retractVertical = new ExtendContractPistonArm(
                    _verticalPistonArm.ToArray(),
                    -baseSpeed*5,
                    "lift drill",
                    0f,
                    max * _verticalPistonArm.Count).Stepper();

                var extendHorizontal = new SkipStepper(new ExtendContractPistonArm(
                    _horizontalPistonArm.ToArray(),
                    baseSpeed,
                    "extend drill",
                    max * _horizontalPistonArm.Count,
                    baseStep,
                    false,
                    _log).Stepper());

                var retractHorizontal = new SkipStepper(new ExtendContractPistonArm(
                    _horizontalPistonArm.ToArray(),
                    -baseSpeed,
                    "retract drill",
                    0f,
                    baseStep).Stepper());

                var retractHorizontalRewind = new ExtendContractPistonArm(
                    _horizontalPistonArm.ToArray(),
                    -baseSpeed*5,
                    "retract drill",
                    0f,
                    max * _horizontalPistonArm.Count).Stepper();

                var enableDrill = new UnitStepper(new SequenceStep()
                {
                    StepTag = "enable Dril",
                    PromiseGenerator = () =>
                    {
                        _drill.Enabled = true;
                        return Void.Promise();
                    }
                });

                var disableDrill = new UnitStepper(new SequenceStep()
                {
                    StepTag = "disable Dril",
                    PromiseGenerator = () =>
                    {
                        _log.Debug("SQ---- disable drill");
                        _drill.Enabled = false;
                        return Void.Promise();
                    }
                });

                var extendRetract = new PairStepper(extendHorizontal, retractHorizontal);
                var extendRetractInfinitely = new CycleStepper(extendRetract, () => true);
                var dig = new PairStepper(
                    enableDrill, 
                    new InterruptingStepper(extendVertical, extendRetractInfinitely));

                _dig = new SequenceController(dig);

                //TODO: extend PairStepper to N arguments
                _reset = new SequenceController(
                    new PairStepper(new PairStepper(retractVertical, retractHorizontalRewind), 
                        disableDrill));
            }

            List<IMyBlockGroup> gbuffer = new List<IMyBlockGroup>();
            List<IMyTerminalBlock> bbuffer = new List<IMyTerminalBlock>();

            List<TBlock> FindGroupByTag<TBlock>(Tag tag)
            where TBlock: class
            {
                gbuffer.Clear();
                
                var result = new List<TBlock>();

                _gameContext.Grid.GetBlockGroups(gbuffer, group => group.Name.Contains(tag.Wrapped));
                foreach (var blockGroup in gbuffer)
                {
                    bbuffer.Clear();
                    blockGroup.GetBlocksOfType<TBlock>(bbuffer);
                    result.AddRange(bbuffer.Cast<TBlock>());
                }

                return result;
            }

            List<TBlock> FindBlockByTag<TBlock>(Tag tag)
                where TBlock : class
            {
                    bbuffer.Clear();
                    _gameContext.Grid.SearchBlocksOfName(tag.Wrapped, bbuffer, x => x is TBlock);
                    return bbuffer.Cast<TBlock>().ToList();
            }
        }
    }
}
