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
        public enum BlockInvType
        {
            AcceptAll,
            EmptyAll,
            Unmanaged,
            Quotas,
            Assembler,
            Refinery,
        }
        public class BlockInvDef
        {
            public List<InfDevItem> Entries;
            public Dictionary<string, InfDevItem> QuoteByItemType = new Dictionary<string, InfDevItem>();
            public IMyTerminalBlock Block;
            public IMyInventory Inventory;
            public BlockInvType Type;
            public int GolbalImportance;
            ILog _log;

            static BlockInvDef ParseOne(IMyTerminalBlock block, int inventoryIndex)
            {
                var tag = new Tag("I", true);
                var options = tag.GetOptions(block.CustomName);
                var inventory = block.GetInventory(inventoryIndex);
                if (options == null || !tag.NameMatches(block.CustomName))
                    return new BlockInvDef() { Type = BlockInvType.Unmanaged, Inventory = inventory, Block = block, };

                if (inventoryIndex > 0) // This is output from production blocks
                {
                    var type = BlockInvType.Unmanaged;
                    if (block is IMyAssembler)
                        type = BlockInvType.Assembler;
                    if (block is IMyRefinery)
                        type = BlockInvType.Refinery;

                    return new BlockInvDef() { Type = type, Inventory = inventory, Block = block, };
                }

                var terms = options.Split(':');

                if (terms.Length == 1)
                    return new BlockInvDef() { Type = BlockInvType.AcceptAll, Inventory = inventory, Block = block, };

                if (options.Contains("EMPTY"))
                    return new BlockInvDef() { Type = BlockInvType.EmptyAll, Inventory = inventory, Block = block, };

                if (options.Contains(".ALL"))
                {
                    int importance;
                    var ok = int.TryParse(terms[1].Split('.')[0], out importance);
                    return new BlockInvDef()
                    {
                        Type = BlockInvType.AcceptAll,
                        GolbalImportance = importance,
                        Inventory = inventory,
                        Block = block,
                    };
                }

                var result = new BlockInvDef()
                {
                    Type = BlockInvType.Quotas, Inventory = inventory, Entries = new List<InfDevItem>(), Block = block,
                };

                foreach (var quoteEntry in terms.Skip(1))
                {
                    var entry = InfDevItem.Parse(quoteEntry);
                    if (entry != null)
                        result.Entries.AddRange(entry);
                }

                EnsureUniqueItemTypes(result);
                result.QuoteByItemType = result.Entries.ToDictionary(x => x.ItemType.ToString(), x => x);

                return result;
            }

            static void EnsureUniqueItemTypes(BlockInvDef result)
            {
                var duplicateCheckSet = new HashSet<string>();
                var filteredEntries = new Dictionary<string, InfDevItem>();
                foreach (var entry in result.Entries)
                {
                    if (duplicateCheckSet.Contains(entry.ItemType.ToString()))
                    {
                        var existing = filteredEntries[entry.ItemType.ToString()];
                        if (existing.Importance < entry.Importance)
                            filteredEntries[entry.ItemType.ToString()] = entry;
                        continue;
                    }

                    duplicateCheckSet.Add(entry.ItemType.ToString());
                    filteredEntries.Add(entry.ItemType.ToString(), entry);
                }

                result.Entries = filteredEntries.Select(x => x.Value).ToList();
            }

            public static List<BlockInvDef> Parse(IMyTerminalBlock block)
            {
                var result = new List<BlockInvDef>();
                for (int i = 0; i < block.InventoryCount; i++)
                {
                    result.Add(ParseOne(block, i));
                }

                return result.Where(x => x != null).ToList();
            }
        }

        /*
         * [I:priority1=0.qualifier1.quote1=inf:priority12=0.qualifier2.quote2=inf]
         *
         * [I] //Accept all
         * [I:EMPTY] //Remove everything
         * [I:5.ALL] //Accept all, importance 5 (can pull stuff from lower importances)
         * [I:6.Ingot] //Fill with ingots (will take from overfilled quotas and lower importances)
         * [I:7.Iron.1000:7.Nickel.1000] //Store 1000 of iron and nickel ingots
         */
        public class InfDevItem
        {
            public ItemType ItemType;
            public MyFixedPoint? Amount;
            public int Importance;

            public static List<InfDevItem> Parse(string s)
            {
                if (s == null || s.Equals(""))
                    return new List<InfDevItem>();

                var decTerms = s.Split(',');
                int importance = 0;
                int? ammount = null;
                int amoutBuffer;
                ItemType itemType;

                try
                {
                    var firstImportance = int.TryParse(decTerms[0], out importance);
                    itemType = new ItemType(firstImportance ? decTerms[1] : decTerms[0]);
                    if (decTerms.Length >= (firstImportance ? 3 : 2))
                        if (int.TryParse(decTerms[firstImportance ? 2 : 1], out amoutBuffer))
                            ammount = amoutBuffer;

                    if (itemType.Subtype == null && itemType.Type != null)
                        if (!ItemType.AllowedTypes.Contains(itemType.Type))
                            throw new Exception();
                }
                catch (Exception e)
                {
                    return new List<InfDevItem>();
                }

                if (itemType.Subtype == null)
                {
                    var subtypes = ItemType
                        .GetCategorySource(itemType.Type);

                    if (!subtypes.Any())
                        return new List<InfDevItem>();

                    var result = new List<InfDevItem>();
                    foreach (var subtype in subtypes)
                        result.Add(new InfDevItem()
                        {
                            Amount = ammount,
                            ItemType = new ItemType(subtype, itemType.Type),
                            Importance = importance,
                        });

                    return result;
                }

                return new List<InfDevItem>()
                {
                    new InfDevItem()
                    {
                        Amount = ammount,
                        ItemType = itemType,
                        Importance = importance,
                    }
                };
            }
        }
    }
}