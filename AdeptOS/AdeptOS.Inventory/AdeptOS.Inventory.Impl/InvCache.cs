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
using VRage.Game.ObjectBuilders.Components.BankingAndCurrency;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class InvCache
        {
            public Dictionary<string, Dictionary<int, List<BlockInvDef>>> TargetsByItemTypeByPriority = new Dictionary<string, Dictionary<int, List<BlockInvDef>>>();
            public List<BlockInvDef> ManagedBlocks = new List<BlockInvDef>();

            ILog _log;

            public InvCache(ILog log)
            {
                _log = log;
            }

            public void RefreshList(List<IMyTerminalBlock> blocks, InvManagerConfig settings)
            {
                var invDefinitions = new List<BlockInvDef>();
                foreach (var block in blocks)
                {
                    invDefinitions.AddRange(BlockInvDef.Parse(block));
                }

                ManagedBlocks = invDefinitions;
                TargetsByItemTypeByPriority.Clear();

                foreach (var invDef in ManagedBlocks)
                {
                    var type = invDef.Type;
                    if (!settings.IgnoreUntagged && type == BlockInvType.Unmanaged)
                        type = BlockInvType.AcceptAll;

                    switch (type)
                    {
                        case BlockInvType.AcceptAll:
                            DefineAcceptAllInventory(invDef);
                            break;
                        case BlockInvType.Quotas:
                            DefineQuoteInventory(invDef);
                            break;
                        case BlockInvType.Assembler:
                        case BlockInvType.Refinery:
                        case BlockInvType.EmptyAll:
                        case BlockInvType.Unmanaged:
                            break;
                    }
                }

                //foreach (var byPrio in TargetsByItemTypeByPriority)
                //{
                //    _log.Debug("----------------", byPrio.Key);

                //    foreach (var pair in byPrio.Value)
                //    {
                //        _log.Debug("=====", pair.Key.ToString());

                //        foreach (var def in pair.Value)
                //        {
                //            _log.Debug(def.Block.CustomName);
                //        }
                //    }
                //}
            }

            void DefineAcceptAllInventory(BlockInvDef inv)
            {
                foreach (var itemType in ItemType.AllTypes)
                {
                    var key = itemType.ToString();
                    if (!TargetsByItemTypeByPriority.ContainsKey(key))
                        TargetsByItemTypeByPriority.Add(key, new Dictionary<int, List<BlockInvDef>>());

                    var byPriority = TargetsByItemTypeByPriority[key];

                    if (!byPriority.ContainsKey(inv.GolbalImportance))
                        byPriority.Add(inv.GolbalImportance, new List<BlockInvDef>());

                    byPriority[inv.GolbalImportance].Add(inv);
                }
            }

            void DefineQuoteInventory(BlockInvDef inv)
            {
                foreach (var quote in inv.Entries)
                {
                    var key = quote.ItemType.ToString();
                    if (!TargetsByItemTypeByPriority.ContainsKey(key))
                        TargetsByItemTypeByPriority.Add(key, new Dictionary<int, List<BlockInvDef>>());

                    var byPriority = TargetsByItemTypeByPriority[key];

                    if (!byPriority.ContainsKey(quote.Importance))
                        byPriority.Add(quote.Importance, new List<BlockInvDef>());

                    byPriority[quote.Importance].Add(inv);
                }
            }
        }
    }
}