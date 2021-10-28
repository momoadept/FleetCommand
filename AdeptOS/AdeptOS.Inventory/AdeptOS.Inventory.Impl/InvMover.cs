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
        public class InvMover
        {
            InvCache _cache;
            Queue<BlockInvDef> _tasks = new Queue<BlockInvDef>();
            bool _inventorySorted = false;
            ILog _log;
            List<MyInventoryItem> _itemBuffer = new List<MyInventoryItem>();
            InvManagerConfig _settings;

            public InvMover(ILog log, InvManagerConfig settings)
            {
                _log = log;
                _settings = settings;
            }

            public void MoveNext()
            {
                if (_cache == null)
                    return;

                if (!_tasks.Any())
                {
                    _tasks = new Queue<BlockInvDef>(_cache.ManagedBlocks);
                }

                if (!_inventorySorted)
                {
                    _inventorySorted = true;
                    var next = _tasks.Peek();
                    SortInventory(next.Inventory, next.Block.CustomName);
                    return;
                }

                MoveInventory(_tasks.Dequeue());
            }

            public void RefreshCache(InvCache cache) => _cache = cache;

            void SortInventory(IMyInventory inventory, string blockName)
            {
                _log.Debug("Sorting", blockName);
                var lastFoundItemTypeIndex = new Dictionary<MyItemType, int>(inventory.ItemCount);
                for (int i = 0; i < inventory.ItemCount; i++)
                {
                    var item = inventory.GetItemAt(i);
                    if (item == null)
                        continue;

                    if (!lastFoundItemTypeIndex.ContainsKey(item.Value.Type))
                    {
                        lastFoundItemTypeIndex.Add(item.Value.Type, i);
                        continue;
                    }

                    var transferOk = inventory.TransferItemTo(inventory, i, lastFoundItemTypeIndex[item.Value.Type], true,
                        item.Value.Amount);

                    _log.Debug("Merging", item.Value.Type.ToString(), "... OK:", transferOk.ToString());
                }
            }

            void MoveInventory(BlockInvDef inventory)
            {
                inventory.Inventory.GetItems(_itemBuffer);
                var transfers = 0;
                var current = 0;

                while (transfers < _settings.TransfersPerStep)
                {
                    var item = _itemBuffer[current];
                    var itemType = new ItemType(item);
                    current++;

                    switch (inventory.Type)
                    {
                        case BlockInvType.AcceptAll:
                            UnloadItemToHigherImportance(inventory.GolbalImportance, item, itemType, inventory,
                                ref transfers, item.Amount);
                            break;

                        case BlockInvType.EmptyAll:
                        case BlockInvType.Assembler:
                        case BlockInvType.Refinery:
                            UnloadItemToHigherImportance(-1, item, itemType, inventory,
                                ref transfers, item.Amount);
                            break;

                        case BlockInvType.Quotas:
                            if (!inventory.QuoteByItemType.ContainsKey(itemType.ToString())) // We don't care about this item and can get rid of it
                            {
                                UnloadItemToHigherImportance(-1, item, itemType, inventory,
                                    ref transfers, item.Amount);
                                break;
                            }

                            var quote = inventory.QuoteByItemType[itemType.ToString()];
                            var transfersBefore = transfers;
                            UnloadItemToHigherImportance(quote.Importance, item, itemType, inventory,
                                ref transfers, item.Amount);
                            if (transfersBefore != transfers) // Higher priority container took away our stuff :(
                                break;

                            if (quote.Amount == null) // Hoard as much as we can
                                break;

                            var extra = item.Amount - quote.Amount.Value;
                            if (extra > 0) // Discard extra
                                UnloadItemToHigherImportance(-1, item, itemType, inventory,
                                    ref transfers, extra);
                            break;
                    }
                }
            }

            void UnloadItemToHigherImportance(int minImportance, MyInventoryItem item, ItemType itemType, BlockInvDef inventory, ref int transfers, MyFixedPoint maxTransfer)
            {
                var higherImportanceStorage = _cache
                    .TargetsByItemTypeByPriority[itemType.ToString()]
                    .Where(x => x.Key > minImportance)
                    .SelectMany(x => x.Value.Where(block => !block.Inventory.IsFull));

                foreach (var candidate in higherImportanceStorage)
                {
                    if (!inventory.Inventory.CanTransferItemTo(candidate.Inventory, item.Type))
                        continue;

                    var shouldTransfer =
                        GetPossibleTransfer(candidate, maxTransfer, item.Type);

                    if (shouldTransfer == 0)
                        continue;

                    _log.Debug("Moving", shouldTransfer.SerializeString(), itemType.ToDisplayString(), "from", inventory.Block.CustomName, "to", candidate.Block.CustomName);
                    inventory.Inventory.TransferItemTo(candidate.Inventory, item, shouldTransfer);
                    transfers++;
                    break;
                }
            }

            MyFixedPoint GetPossibleTransfer(BlockInvDef to, MyFixedPoint amount, MyItemType itemType)
            {
                var transfer = amount;
                var customItemType = new ItemType(itemType);

                if (to.QuoteByItemType.ContainsKey(customItemType.ToString()))
                {
                    // Check quote before moving
                    var quote = to.QuoteByItemType[customItemType.ToString()];
                    if (quote.Amount != null)
                    {
                        var alreadyHave = to.Inventory.GetItemAmount(itemType);
                        var need = quote.Amount.Value - alreadyHave;
                        var reallyNeed = need >= 0 ? need : 0;

                        transfer = transfer > reallyNeed ? reallyNeed : transfer;
                    }
                }

                return transfer;
            }
        }
    }
}