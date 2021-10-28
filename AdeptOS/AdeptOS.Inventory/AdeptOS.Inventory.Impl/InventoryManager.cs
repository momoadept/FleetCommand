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
        public class InventoryManager : IInventoryManager, IModule
        {
            public string UniqueName => "INV_impl";
            public string Alias => "InventoryManagerImplementation";


            InventoryManagerState _state;
            InvManagerConfig _settings;
            ILog _log;
            IGameContext _gameContext;
            List<IMyTerminalBlock> _observingBlocks = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> _summaryTargets = new List<IMyTerminalBlock>();
            InvCache _invCache = new InvCache();
            InvMover _mover;

            public InventoryManager(InvManagerConfig settings = null)
            {
                _settings = settings;
            }

            // Contract

            public IPromise<Void> ForceUpdatePolicies()
            {
                ScanInventoryBlocks();
                return Void.Promise();
            }

            public IPromise<Void> SetState(InventoryManagerState value)
            {
                _state = value;
                return Void.Promise();
            }

            // Lifecycle

            public void Bind(IBindingContext context)
            {
                _log = context.RequireOne<ILog>();
                _gameContext = context.RequireOne<IGameContext>();

                if (_settings == null)
                    _settings = new InvManagerConfig(_gameContext.Me.CustomData);

                _mover = new InvMover(_log, _settings);
            }

            public void Restore(InventoryManagerState state)
            {
                _state = state;
                _log.Debug("Inventory Manager Restored");
            }

            public InventoryManagerState GetState() => _state;

            public void Run()
            {
                _log.Info("Inventory Manager Started");
                _log.Debug("My state is", _state.Stringify());

                Aos.Async.CreateJob(ScanInventoryBlocks, Priority.Unimportant).Start();
                Aos.Async.CreateJob(MoveInventory).Start();
            }

            public void OnSaving() {}

            // Jobs

            void ScanInventoryBlocks()
            {
                _log.Debug("Scan blocks...");
                _observingBlocks.Clear();
                _summaryTargets.Clear();
                _gameContext.Grid.GetBlocksOfType<IMyTerminalBlock>(_observingBlocks, ShouldObserveBlock);
                _gameContext.Grid.SearchBlocksOfName(_settings.SummaryLcdTag.Wrapped, _summaryTargets);

                _log.Debug("Discovered inventories:", _observingBlocks.Count.ToString());
                _log.Debug("Discovered lcds:", _summaryTargets.Count.ToString());

                _invCache.RefreshList(_observingBlocks, _settings);
                _mover?.RefreshCache(_invCache);
            }

            void MoveInventory()
            {
                for (int i = 0; i < _settings.StepsPerTick; i++)
                {
                    _mover.MoveNext();
                }
            }

            bool ShouldObserveBlock(IMyTerminalBlock block) => block.HasInventory && (_settings.IgnoreUntagged || _settings.Tag.NameMatches(block.CustomName));
        }
    }
}
