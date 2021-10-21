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
        public class RotorDrill : IModule, IAutoDrill
        {
            public string UniqueName { get; }
            public string Alias { get; }

            RotorDrillState _state;
            RotorDrillBlocks _blocks;
            RotorDrillSequences _sequences;

            IGameContext _context;
            ILog _log;

            IJob _debug;

            RotorDrillStateMachine _stateMachine;

            public void Bind(IBindingContext context)
            {
                _context = context.RequireOne<IGameContext>(this);
                _log = context.RequireOne<ILog>(this);
            }

            public void Run()
            {
                _blocks = new RotorDrillBlocks(_context.Grid);
                if (!(_blocks.Refresh()))
                {
                    _log.Error("Not all blocks found for auto drill");
                }

                _sequences = new RotorDrillSequences();
                _sequences.Build(_blocks);

                _stateMachine = new RotorDrillStateMachine(new Dictionary<RotorDrillStage, IAutoDrillControlObject>()
                {
                    { RotorDrillStage.Drilling, new Drilling() },
                    { RotorDrillStage.FinishedAll, new FinishedAll() },
                    { RotorDrillStage.FinishedLayer, new FinshedLayer() },
                    { RotorDrillStage.MovingToLayer, new MovingToLayer() },
                    { RotorDrillStage.Rewinding, new Rewinding() },
                    { RotorDrillStage.StartingPosition, new StartingPosition() },
                }, _state.Stage, new RotorDrillContext()
                {
                    Sequences = _sequences,
                    Blocks = _blocks,
                    State = _state,
                },
                    _log);

                if (_debug == null)
                    _debug = Aos.Async.CreateJob(Debug);
                _debug.Start();
            }

            private void Debug()
            {
                var report = _stateMachine?.Actions.Report();

                if (_stateMachine == null)
                {
                    report = "Couldn't start drilling";
                }

                if (_blocks.ReportLcd != null)
                    _blocks.ReportLcd.ContentType = ContentType.TEXT_AND_IMAGE;

                _blocks.ReportLcd?.WriteText(report);
            }

            public void OnSaving()
            {
            }

            public IPromise<Void> Drill()
            {
                _stateMachine?.Actions.Drill();
                return Void.Promise();
            }

            public IPromise<Void> Pause()
            {
                _stateMachine?.Actions.Pause();
                return Void.Promise();
            }

            public IPromise<Void> Resume()
            {
                _stateMachine?.Actions.Resume();
                return Void.Promise();
            }

            public IPromise<Void> Reset()
            {
                if (_blocks.Valid)
                {
                    _stateMachine?.Actions.Reset();
                }
                else 
                    Run();

                return Void.Promise();
            }

            public void SetState(RotorDrillState state)
            {
                _state = state;
            }

            public RotorDrillState GetState() => _state;
        }
    }
}
