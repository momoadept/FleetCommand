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


            private IMyPistonBase _verticalPiston;
            private IMyPistonBase _horizontalPiston;
            private List<IMyPistonBase> _horizontalPistonArm = new List<IMyPistonBase>();
            private IMyShipDrill _drill;
            private IMyTextPanel _reportLcd;
            private bool valid;
            private StepSequence _extend;
            private StepSequence _extendArm;
            private StepSequence _contract;
            private StepSequence _contractArm;

            public IPromise<Void> Start()
            {
                _log.Debug("drill start");
                try
                {
                    

                    Aos.Async.Delay().Then(x =>
                    {
                        _contractArm.Reset();

                        if (!_extendArm.IsStarted())
                            _extendArm.StepAll();

                        if (_extendArm.IsPaused())
                            _extendArm.Resume();
                    });

                    return Void.Promise();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                    _gameContext.Echo(e.Message);
                    return Void.Promise();
                }
            }

            public IPromise<Void> Stop()
            {
             
                Aos.Async.Delay().Then(x =>
                {
                    _extendArm.Reset();

                    if (!_contractArm.IsStarted())
                        _contractArm.StepAll();

                    if (_contractArm.IsPaused())
                        _contractArm.Resume();
                });

                return Void.Promise();
            }

            public IPromise<Void> Reset()
            {
                _contractArm.Reset();
                _extendArm.Reset();

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
                _extend = new ExtendPiston(_horizontalPiston, 1, "Horizontal", 8f, 1f, _log).Sequence();
                _contract = new ContractPiston(_horizontalPiston, 3, "Horizontal", 0.5f, 1f, _log).Sequence();
                _extendArm = new ExtendContractPistonArm(_horizontalPistonArm.ToArray(), 1, "Arm extend", 30, 2f).Sequence();
                _contractArm = new ExtendContractPistonArm(_horizontalPistonArm.ToArray(), -1, "Arm contract", 1, 2f).Sequence();

                Aos.Async.CreateJob(Report).Start();
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

            private void Report()
            {
                if (_reportLcd == null)
                    return;

                var report = _extendArm.GetReport().Split('\n').Reverse();
                var report2 = _contractArm.GetReport().Split('\n').Reverse();

                _reportLcd.WriteText("\n[Extend]\n");
                _reportLcd.WriteText(string.Join("\n", report), true);
                _reportLcd.WriteText("\n", true);
                _reportLcd.WriteText("\n[Contract]\n", true);
                _reportLcd.WriteText(string.Join("\n", report2), true);
                _reportLcd.WriteText("\n", true);
            }

            private void DetectBlocks()
            {
                var verticalTag = new Tag("AD_V"); // [AD_V] huipizda
                var horizontalTag = new Tag("AD_H");
                var horizontalArmTag = new Tag("AD_HH");
                var drillTag = new Tag("AD_D");
                var lcdTag = new Tag("AD_S");

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

                _gameContext.Grid.SearchBlocksOfName(lcdTag.Wrapped, blocks, it => it is IMyTextPanel);
                _reportLcd = blocks.FirstOrDefault() as IMyTextPanel;
                blocks.Clear();

                var groups = new List<IMyBlockGroup>();
                _horizontalPistonArm.Clear();
                _gameContext.Grid.GetBlockGroups(groups, x => x.Name.Contains(horizontalArmTag.Wrapped));
                foreach (var blockGroup in groups)
                {
                    blocks.Clear();
                    blockGroup.GetBlocksOfType<IMyPistonBase>(blocks);
                    _horizontalPistonArm.AddRange(blocks.Cast<IMyPistonBase>());
                }
                

                if (_verticalPiston == null || _horizontalPiston == null || _drill == null)
                {
                    _log.Error("No drill found for auto drill");
                    valid = false;
                    return;
                }

                valid = true;
            }
        }
    }
}
