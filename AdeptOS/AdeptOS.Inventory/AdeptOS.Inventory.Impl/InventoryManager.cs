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


            private InventoryManagerState _state;
            private InvMgrSettings _settings;
            private ILog _log;
            private IGameContext _gameContext;
            private List<IMyTerminalBlock> _observingBlocks = new List<IMyTerminalBlock>();
            private List<IMyTerminalBlock> _summaryTargets = new List<IMyTerminalBlock>();
            private List<InvDeclaration> _declarations = new List<InvDeclaration>();

            public InventoryManager(InvMgrSettings settings = null)
            {
                _settings = settings ?? new InvMgrSettings();
            }

            // Contract

            public IPromise<Void> ForceUpdatePolicies()
            {
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
            }

            public void Restore(InventoryManagerState state)
            {
                _state = state;
                _log.Debug("Inventory Manager Restored");
            }

            public InventoryManagerState GetState()
            {
                return _state;
            }

            public void Run()
            {
                _log.Debug("Inventory Manager Started");
                _log.Debug("My state is", _state.Stringify());

                Aos.Async.CreateJob(ScanInventoryBlocks, Priority.Unimportant).Start();
                Aos.Async.CreateJob(UpdateDeclarations).Start();
                Aos.Async.CreateJob(ReportInventory).Start();
            }

            public void OnSaving()
            {
                _log.Debug("Inventory Manager Saving");
            }

            // Jobs
            
            private void ScanInventoryBlocks()
            {
                _log.Debug("Scan blocks...");
                _observingBlocks.Clear();
                _summaryTargets.Clear();
                _gameContext.Grid.GetBlocksOfType<IMyTerminalBlock>(_observingBlocks, block => block.HasInventory);
                _gameContext.Grid.SearchBlocksOfName(_settings.InvSummaryLcdTag.Wrapped, _summaryTargets);

                _log.Debug("Discovered inventories:", _observingBlocks.Count.ToString());
                _log.Debug("Discovered lcds:", _summaryTargets.Count.ToString());
            }

            private void UpdateDeclarations()
            {
                //_log.Debug("Update declarations...");
                _declarations.Clear();
                foreach (var block in _observingBlocks)
                {
                    if (block.InventoryCount == 1)
                        _declarations.Add(DeclareInventory(block, block.GetInventory()));
                    else
                    {
                        for (int i = 0; i < block.InventoryCount; i++)
                        {
                            var inventory = block.GetInventory(i);
                            _declarations.Add(DeclareInventory(block, inventory));
                        }
                    }
                }
            }

            private void ReportInventory()
            {
                //_log.Debug("Report inventory...");
                var report = new Dictionary<string, InvDeclarationRecord>();

                foreach (var invDeclaration in _declarations)
                {
                    foreach (var item in invDeclaration.Have)
                    {
                        if (!report.ContainsKey(item.Key))
                        {
                            report.Add(item.Key, new InvDeclarationRecord(item.Value.ItemType, 0, 0, null));
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

            // Internal

            private InvDeclaration DeclareInventory(IMyTerminalBlock block, IMyInventory inventory)
            {
                var result = new InvDeclaration(inventory)
                {
                    Have = DeclareHave(inventory)
                };

                return result;
            }

            private Dictionary<string, InvDeclarationRecord> DeclareHave(IMyInventory inventory)
            {
                var itemCount = inventory.ItemCount;
                var result = new Dictionary<string, InvDeclarationRecord>();
                for (int i = 0; i < itemCount; i++)
                {
                    var item = inventory.GetItemAt(i);
                    if (item == null)
                        continue;

                    var itemType = new ItemType(item.Value);

                    if (!result.ContainsKey(itemType.ToString()))
                        result.Add(itemType.ToString(), new InvDeclarationRecord(itemType, 0, 0, inventory));

                    result[itemType.ToString()].Amount += item.Value.Amount;
                }

                return result;
            }
        }
    }
}
