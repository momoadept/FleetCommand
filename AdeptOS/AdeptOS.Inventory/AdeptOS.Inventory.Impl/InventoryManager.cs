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
            InvMgrSettings _settings;
            ILog _log;
            IGameContext _gameContext;
            List<IMyTerminalBlock> _observingBlocks = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> _summaryTargets = new List<IMyTerminalBlock>();
            List<InvDeclaration> _declarations = new List<InvDeclaration>();

            public InventoryManager(InvMgrSettings settings = null)
            {
                _settings = settings ?? new InvMgrSettings();
            }

            // Contract

            public IPromise<Void> ForceUpdatePolicies() => Void.Promise();

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
            }

            public void Restore(InventoryManagerState state)
            {
                _state = state;
                _log.Debug("Inventory Manager Restored");
            }

            public InventoryManagerState GetState() => _state;

            public void Run()
            {
                _log.Debug("Inventory Manager Started");
                _log.Debug("My state is", _state.Stringify());

                Aos.Async.CreateJob(ScanInventoryBlocks, Priority.Unimportant).Start();
                Aos.Async.CreateJob(UpdateDeclarations).Start();
                //Aos.Async.CreateJob(ReportInventory).Start();

                //ScanInventoryBlocks();
                //UpdateDeclarations();
            }

            public void OnSaving() => _log.Debug("Inventory Manager Saving");

            // Jobs

            void ScanInventoryBlocks()
            {
                _log.Debug("Scan blocks...");
                _observingBlocks.Clear();
                _summaryTargets.Clear();
                _gameContext.Grid.GetBlocksOfType<IMyTerminalBlock>(_observingBlocks, block => block.HasInventory);
                _gameContext.Grid.SearchBlocksOfName(_settings.InvSummaryLcdTag.Wrapped, _summaryTargets);

                _log.Debug("Discovered inventories:", _observingBlocks.Count.ToString());
                _log.Debug("Discovered lcds:", _summaryTargets.Count.ToString());
            }

            void UpdateDeclarations()
            {
                //_log.Debug("Update declarations...");
                _declarations.Clear();
                foreach (var block in _observingBlocks)
                {
                    var invDefinition = BlockInvDef.Parse(block);
                    var declarations = invDefinition.Select(x => x.Declare(_log, block.CustomName));
                    _declarations.AddRange(declarations);
                }

                new InvMover(_declarations, _log).Iterate();
            }

            void ReportInventory()
            {
                //_log.Debug("Report inventory...");
                var report = new Dictionary<string, InvDeclarationRecord>();

                foreach (var invDeclaration in _declarations)
                {
                    foreach (var item in invDeclaration.Have)
                    {
                        if (!report.ContainsKey(item.Key))
                        {
                            report.Add(item.Key, new InvDeclarationRecord(item.Value.ItemType, 0, 0, null, null));
                        }

                        report[item.Key].Amount += item.Value.Amount;
                    }
                }

                var sortedReport = report.Select(it => it.Value).OrderBy(it => it.ItemType.ToString()).ToList();

                foreach (var target in _summaryTargets.Where(it => it is IMyTextPanel))
                {
                    var surface = (IMyTextPanel)target;
                    surface.ContentType = ContentType.TEXT_AND_IMAGE;
                    surface.WriteText($"Inventory {Aos.Node.NodeId} ({Aos.Node.ShipAlias})\n");

                    foreach (var entry in sortedReport)
                    {
                        surface.WriteText(
                            $"{entry.ItemType.ToDisplayString().FillWhitespace(_settings.SummaryTypeWidth)}{entry.Amount}\n", true);
                    }
                }
            }
        }
    }
}
