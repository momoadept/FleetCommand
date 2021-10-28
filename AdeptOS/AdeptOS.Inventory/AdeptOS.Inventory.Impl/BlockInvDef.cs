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
                    return new BlockInvDef()
                    {
                        Type = BlockInvType.Unmanaged,
                        Inventory = inventory
                    };

                if (inventoryIndex > 0) // This is output from production blocks
                {
                    var type = BlockInvType.Unmanaged;
                    if (block is IMyAssembler)
                        type = BlockInvType.Assembler;
                    if (block is IMyRefinery)
                        type = BlockInvType.Refinery;

                    return new BlockInvDef() { Type = type, Inventory = inventory, };
                }

                var terms = options.Split(':');

                if (terms.Length == 1)
                    return new BlockInvDef() { Type = BlockInvType.AcceptAll, Inventory = inventory };

                if (options.Contains("EMPTY"))
                    return new BlockInvDef() { Type = BlockInvType.EmptyAll, Inventory = inventory };

                if (options.Contains(".ALL"))
                {
                    int importance;
                    var ok = int.TryParse(terms[1].Split('.')[0], out importance);
                    return new BlockInvDef()
                    {
                        Type = BlockInvType.AcceptAll, GolbalImportance = importance, Inventory = inventory,
                    };
                }

                var result = new BlockInvDef()
                {
                    Type = BlockInvType.Quotas, Inventory = inventory, Entries = new List<InfDevItem>()
                };

                foreach (var quoteEntry in terms.Skip(1))
                {
                    var entry = InfDevItem.Parse(quoteEntry);
                    if (entry != null)
                        result.Entries.Add(entry);
                }

                return result;
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

            public InvDeclaration Declare(ILog log, string blockName)
            {
                _log = log;
                //_log.Debug("Declaring inventory", blockName, Type.ToString());
                var result = new InvDeclaration(Inventory)
                {
                    Have = DeclareHave(Inventory, blockName)
                };

                switch (Type)
                {
                    case BlockInvType.AcceptAll:
                        result.Accept = ItemType.Subtypes.Select(x => new ItemType(x)).ToDictionary(x => x.ToString(), x => new InvAcceptRecord(GolbalImportance, x, Inventory, blockName));
                        break;
                    case BlockInvType.Assembler:
                    case BlockInvType.Refinery:
                    case BlockInvType.EmptyAll:
                        result.DontWant = result.Have.Select(x => x)
                            .Select(x => new InvDeclarationRecord(x.Value.ItemType, x.Value.Amount, 0, Inventory, blockName))
                            .ToDictionary(x => x.ItemType.ToString(), x => x);
                        break;
                    case BlockInvType.Quotas:
                        AddQuotes(result, blockName);
                        break;
                    case BlockInvType.Unmanaged:
                        break;
                }

                if (Type == BlockInvType.Unmanaged)
                    return result;

                //_log.Debug("Has:");
                //foreach (var has in result.Have)
                //{
                //    _log.Debug(has.Value.ItemType.ToString(), has.Value.Amount.ToString());
                //}

                //_log.Debug("Accepts:");
                //foreach (var accept in result.Accept)
                //{
                //    _log.Debug(accept.Value.ItemType.ToString(), accept.Value.Importance.ToString());
                //}

                //_log.Debug("Wants:");
                //foreach (var want in result.Want)
                //{
                //    _log.Debug(want.Value.Importance.ToString(), want.Value.ItemType.ToString(), want.Value.Amount.SerializeString());
                //}

                //_log.Debug("Doesn't want:");
                //foreach (var unwant in result.DontWant)
                //{
                //    _log.Debug(unwant.Value.Importance.ToString(), unwant.Value.ItemType.ToString(), unwant.Value.Amount.SerializeString());
                //}

                return result;
            }

            void AddQuotes(InvDeclaration result, string blockName)
            {
                foreach (var entry in Entries)
                    AddQuote(result, entry, blockName);

                var managedItemTypes = Entries
                    .SelectMany(x => x.Type == InvDefType.Item ? new [] {x.ItemType.ToString() }: GetCategorySource(x) )
                    .Select(x => new ItemType(x).ToString())
                    .ToList();

                _log.Debug("-----Managed types for", blockName);
                _log.Debug(managedItemTypes.ToArray());
                for (int i = 0; i < Inventory.ItemCount; i++)
                {
                    var item = Inventory.GetItemAt(i);
                    if (item == null)
                        continue;

                    var type = new ItemType(item.Value);
                    //_log.Debug("looking at ", type.ToString());
                    if (!managedItemTypes.Contains(type.ToString()))
                    {
                        if (result.DontWant.ContainsKey(type.ToString()))
                            result.DontWant[type.ToString()].Amount += item.Value.Amount;
                        else 
                            result.DontWant.Add(type.ToString(), new InvDeclarationRecord(type, item.Value.Amount, 0, Inventory, blockName));
                    }
                }
            }

            void AddQuote(InvDeclaration result, InfDevItem quote, string blockName)
            {
                if (quote.Type == InvDefType.Category)
                {
                    //throw new Exception("category");
                    string[] categorySource = GetCategorySource(quote);

                    foreach (var s in categorySource)
                    {
                        var type = new ItemType(s, quote.ItemType.Type);
                        if (!result.Accept.ContainsKey(type.ToString()))
                            result.Accept.Add(type.ToString(), new InvAcceptRecord(quote.Importance, type, Inventory, blockName));
                        else
                            result.Accept[type.ToString()].Importance =
                                Math.Max(result.Accept[type.ToString()].Importance, quote.Importance);

                        if (result.Have.ContainsKey(type.ToString()))
                        {
                            result.Have[type.ToString()].Importance = quote.Importance;
                        }
                    }

                    return;
                }

                if (quote.Amount != null)
                {
                    var key = quote.ItemType.ToString();
                    var have = result.Have.ContainsKey(key) ? result.Have[key].Amount : 0;
                    if (have > quote.Amount && !result.DontWant.ContainsKey(key))
                        result.DontWant.Add(key, new InvDeclarationRecord(quote.ItemType, have - quote.Amount.Value, quote.Importance, Inventory, blockName));

                    if (have < quote.Amount && !result.Want.ContainsKey(key))
                        result.Want.Add(key, new InvDeclarationRecord(quote.ItemType, quote.Amount.Value - have, quote.Importance, Inventory, blockName));
                }
                else
                {
                    var type = quote.ItemType;
                    if (!result.Accept.ContainsKey(type.ToString()))
                        result.Accept.Add(type.ToString(), new InvAcceptRecord(quote.Importance, type, Inventory, blockName));
                    else
                        result.Accept[type.ToString()].Importance =
                            Math.Max(result.Accept[type.ToString()].Importance, quote.Importance);
                }

                if (result.Have.ContainsKey(quote.ItemType.ToString()))
                {
                    result.Have[quote.ItemType.ToString()].Importance = quote.Importance;
                }
            }

            static string[] GetCategorySource(InfDevItem quote)
            {
                string[] categorySource = { };
                switch (quote.ItemType.Type)
                {
                    case "Ore":
                        categorySource = ItemType.Ores;
                        break;
                    case "Ingot":
                        categorySource = ItemType.Ingots;
                        break;
                    case "Component":
                        categorySource = ItemType.Components;
                        break;
                }

                return categorySource;
            }

            Dictionary<string, InvDeclarationRecord> DeclareHave(IMyInventory inventory, string blockName)
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
                        result.Add(itemType.ToString(), new InvDeclarationRecord(itemType, 0, 0, inventory, blockName));

                    result[itemType.ToString()].Amount += item.Value.Amount;
                }

                return result;
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

        public enum InvDefType
        {
            Category,
            Item
        }

        public class InfDevItem
        {
            public ItemType ItemType;
            public InvDefType Type;
            public MyFixedPoint? Amount;
            public int Importance;

            public static InfDevItem Parse(string s)
            {
                if (s == null || s.Equals(""))
                    return null;

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
                    return null;
                }

                return new InfDevItem()
                {
                    Amount = ammount,
                    ItemType = itemType,
                    Importance = importance,
                    Type = itemType.Subtype == null ? InvDefType.Category : InvDefType.Item
                };
            }
        }
    }
}