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
            private IMyShipDrill _drill;
            private IMyTextPanel _reportLcd;
            private bool valid;
            private StepSequence _extend;

            public IPromise<Void> Start()
            {
                if (_extend == null)
                    return Void.Promise();

                if (!_extend.IsStarted())
                    _extend.StepAll();

                if (_extend.IsPaused())
                    _extend.Resume();

                return Void.Promise();
            }

            public IPromise<Void> Stop()
            {
                _extend.Pause();

                return Void.Promise();
            }

            public IPromise<Void> Reset()
            {
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
                Aos.Async.CreateJob(Report).Start();
                _extend = new ExtendPiston(_horizontalPiston, 1, "Horizontal").Sequence();
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

                var report = _extend.GetReport().Split('\n').Reverse();

                _reportLcd.WriteText("\n");
                _reportLcd.WriteText(string.Join("\n", report));
                _reportLcd.WriteText("\n");
            }

            private void DetectBlocks()
            {
                var verticalTag = new Tag("AD_V"); // [AD_V] huipizda
                var horizontalTag = new Tag("AD_H");
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
