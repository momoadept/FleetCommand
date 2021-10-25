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
        public class RotorDrill : IModule, IRotorDrill
        {
            public string UniqueName { get; }
            public string Alias { get; }

            RotorDrillState _state;
            RotorDrillBlocks _blocks;
            RotorDrillSequences _sequences;

            IGameContext _context;
            ILog _log;
            ILcdTracer _lcd;

            RotorDrillStateMachine _stateMachine;

            public void Bind(IBindingContext context)
            {
                _context = context.RequireOne<IGameContext>();
                _log = context.RequireOne<ILog>();
                _lcd = context.One<ILcdTracer>();
            }

            public void Run()
            {
                _blocks = new RotorDrillBlocks(_context.Grid);
                if (!(_blocks.Refresh()))
                {
                    _log.Error("Not all blocks found for auto drill");
                    return;
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

                _log.Info(_lcd?.ToString());

                _lcd?.SetTrace(new Tag("AD_SG"), TraceGeneral);
                _lcd?.SetTrace(new Tag("AD_SC"), TraceCurrent);
            }

            string TraceGeneral()
            {
                var report = _stateMachine?.Actions.Report();

                if (_stateMachine == null)
                {
                    report = "Couldn't start drilling";
                }

                return report;
            }

            string TraceCurrent()
            {
                if (_stateMachine == null)
                    return ":(";

                switch (_stateMachine.Current)
                {
                    case RotorDrillStage.StartingPosition:
                        return "Waiting for input. Target layer = " + _state.CurrentLayer;
                    case RotorDrillStage.Drilling:
                        return _sequences.DrillLayer.GetReport();
                    case RotorDrillStage.FinishedLayer:
                        return _sequences.LowerToLayer.GetReport();
                    case RotorDrillStage.Rewinding:
                        return _sequences.RewindRotor.GetReport();
                    case RotorDrillStage.MovingToLayer:
                        return _sequences.LowerToLayer.GetReport();
                    case RotorDrillStage.Error:
                        return "ERROR";
                    case RotorDrillStage.FinishedAll:
                        return "Done drilling. Waiting for input";
                }

                return ":(";
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

            public IPromise<Void> SkipToLayer(int layer)
            {
                _stateMachine?.Actions.SkipToLayer(layer);
                return Void.Promise();
            }

            public void SetState(RotorDrillState state) => _state = state;

            public RotorDrillState GetState() => _state;
        }
    }
}
