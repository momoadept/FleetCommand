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
            ILog _log;

            List<InvDeclaration> _declarations;

            List<InvDeclarationRecord> _allNeeded;
            List<InvDeclarationRecord> _allToDiscard;
            List<InvDeclarationRecord> _allHave;
            List<InvAcceptRecord> _allAccept;

            int _ops = 0;
            int limit = 0;

            List<MyInventoryItem> _itemBuffer = new List<MyInventoryItem>();

            public InvMover(List<InvDeclaration> declarations, ILog log)
            {
                _declarations = declarations;
                _log = log;
                _allNeeded = declarations.SelectMany(x => x.Want.Select(y => y.Value))
                    .OrderByDescending(x => x.Importance)
                    .ToList();

                _allToDiscard = declarations.SelectMany(x => x.DontWant.Select(y => y.Value))
                    .OrderByDescending(x => x.Importance)
                    .ToList();

                _allHave = declarations.SelectMany(x => x.Have.Select(y => y.Value))
                    .OrderBy(x => x.Importance)
                    .ToList();

                _allAccept = declarations.SelectMany(x => x.Accept.Select(y => y.Value))
                    .OrderByDescending(x => x.Importance)
                    .ToList();
            }

            public void Iterate()
            {
                
                //_log.Debug("-----Have:");
                //foreach (var has in _allHave)
                //{
                //    _log.Debug(has.BlockName, has.Importance.ToString(), has.ItemType.ToString(), has.Amount.ToString());
                //}

                //_log.Debug("-----Accept bills:");
                //foreach (var accept in _allAccept)
                //{
                //    _log.Debug(accept.BlockName, accept.Importance.ToString(), accept.ItemType.ToString());
                //}

                //_log.Debug("-----Need bills:");
                //foreach (var want in _allNeeded)
                //{
                //    _log.Debug(want.BlockName, want.Importance.ToString(), want.ItemType.ToString(), want.Amount.SerializeString());
                //}

                //_log.Debug("-----Discard bills:");
                //foreach (var unwant in _allToDiscard)
                //{
                //    _log.Debug(unwant.BlockName, unwant.Importance.ToString(), unwant.ItemType.ToString(), unwant.Amount.SerializeString());
                //}

                _ops = 0;

                foreach (var neededBill in _allNeeded)
                {
                    if (_ops > limit)
                        break;

                    FulfillNeeded(neededBill);
                }

                foreach (var discardBill in _allToDiscard)
                {
                    if (_ops > limit)
                        break;

                    FulfillNotWanted(discardBill);
                }

                foreach (var acceptBill in _allAccept)
                {
                    if (_ops > limit)
                        break;

                    FulfillAccept(acceptBill);
                }
            }

            void FulfillAccept(InvAcceptRecord accept)
            {
                _log.Debug("Trying to fill ACCEPT ", accept.ItemType.ToDisplayString(), accept.BlockName);

                var applicableStocks = _allHave
                    .Where(x => x.BlockName.Contains("[I"))
                    .Where(x => x.Importance < accept.Importance)
                    .Where(x => x.ItemType.Equals(accept.ItemType));


                foreach (var stock in applicableStocks)
                {

                    //throw new Exception("Accept cycle");
                    if (_ops > limit)
                        break;
                    if (accept.Declarer.IsFull)
                    {
                        //_log.Debug("Skipping pulling accepted items since inventory is full");
                        break;
                    }

                    //throw new Exception("Accept pulling");
                    //_log.Debug("Found ", stock.Amount.ToString(), "in", stock.BlockName);
                    Pull(stock, accept);
                    _ops++;
                }
            }

            void FulfillNeeded(InvDeclarationRecord needed)
            {
                _log.Debug("Trying to fill quote ", needed.ItemType.ToDisplayString(), needed.Amount.ToString(), needed.BlockName);

                // check Unwanted
                //_log.Debug("Looking for unwanted inventory to pull...");
                var applicableDiscarded = _allToDiscard.Where(x => x.ItemType.Equals(needed.ItemType));

                foreach (var discard in applicableDiscarded)
                {
                    if (_ops > limit)
                        break;
                    if (needed.Amount <= 0)
                        break;

                    if (needed.Declarer.IsFull)
                    {
                        //_log.Debug("Skipping filling quote since inventory is full");
                        break;
                    }

                    //_log.Debug("Found ", discard.Amount.ToString(), "in", discard.BlockName);
                    Pull(discard, needed);
                    _ops++;
                }

                if (needed.Amount <= 0)
                    return;

                // check Lower priorities
                //_log.Debug("Looking for lower importance inventory to pull...");
                var applicableStocks = _allHave
                    .Where(x => x.BlockName.Contains("[I"))
                    .Where(x => x.Importance < needed.Importance)
                    .Where(x => x.ItemType.Equals(needed.ItemType));

                foreach (var stock in applicableStocks)
                {
                    if (_ops > limit)
                        break;
                    if (needed.Amount <= 0)
                        break;

                    if (needed.Declarer.IsFull)
                    {
                        //_log.Debug("Skipping filling quote since inventory is full");
                        break;
                    }

                    //_log.Debug("Found ", stock.Amount.ToString(), "in", stock.BlockName);
                    Pull(stock, needed);
                    _ops++;
                }
            }

            void FulfillNotWanted(InvDeclarationRecord notWanted)
            {
                _log.Debug("Trying to discard extra ", notWanted.ItemType.ToDisplayString(), notWanted.Amount.ToString(), notWanted.BlockName);

                // check accepters
                //_log.Debug("Looking for accepting inventory to push...");
                var applicableStocks = _allAccept
                    .Where(x => x.ItemType.Equals(notWanted.ItemType));

                foreach (var declaration in applicableStocks)
                {
                    if (_ops > limit)
                        break;
                    if (notWanted.Amount <=0)
                        break;

                    //_log.Debug("Pushing to ", declaration.BlockName);
                    Push(notWanted, declaration);
                    _ops++;
                }
            }

            void Pull(InvDeclarationRecord from, InvDeclarationRecord to)
            {
                if (from.BlockName == to.BlockName)
                    return;
                var leftTotransfer = @from.Amount > to.Amount ? to.Amount : @from.Amount;
                @from.Declarer.GetItems(_itemBuffer);

                foreach (var item in _itemBuffer.Where(x => new ItemType(x).Equals(to.ItemType)))
                {
                    //_log.Debug("Looking at ", item.Type.SubtypeId, item.Amount.SerializeString());
                    if (leftTotransfer <= 0)
                        break;

                    var thisTransfer = leftTotransfer > item.Amount ? item.Amount : leftTotransfer;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    //while (thisTransfer.RawValue > 0 && !to.Declarer.CanItemsBeAdded(thisTransfer, item.Type))
                    //    thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName, "->", to.BlockName);
                    //if (thisTransfer > 0)
                    //    _log.Debug(to.Declarer.TransferItemFrom(@from.Declarer, item, thisTransfer).ToString());

                    if (to.Declarer.CanItemsBeAdded(thisTransfer, item.Type))
                    {
                        to.Declarer.TransferItemFrom(@from.Declarer, item, thisTransfer);
                        leftTotransfer -= thisTransfer;
                        @from.Amount -= thisTransfer;
                        to.Amount -= thisTransfer;
                    }
                }
            }

            void Pull(InvDeclarationRecord from, InvAcceptRecord to)
            {
                //var s = $"PULL {from.BlockName} -> {to.BlockName} {from.ItemType}";
                //throw new Exception(s);
                if (from.BlockName == to.BlockName)
                    return;
                var ammount = from.Amount;
                from.Declarer.GetItems(_itemBuffer);

                foreach (var item in _itemBuffer.Where(x => new ItemType(x).Equals(to.ItemType)))
                {
                    //_log.Debug("Looking at ", item.Type.SubtypeId, item.Amount.SerializeString());
                    if (ammount <= 0 || to.Declarer.IsFull)
                        break;

                    var thisTransfer = ammount > item.Amount ? item.Amount : ammount;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    //while (!to.Declarer.CanItemsBeAdded(thisTransfer, item.Type) && thisTransfer.RawValue > 0)
                    //    thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName, "->", to.BlockName);
                    //if (thisTransfer > 0)
                    //    _log.Debug(to.Declarer.TransferItemFrom(from.Declarer, item, thisTransfer).ToString());

                    if (to.Declarer.CanItemsBeAdded(thisTransfer, item.Type))
                    {
                        to.Declarer.TransferItemFrom(@from.Declarer, item, thisTransfer);
                        ammount -= thisTransfer;
                        from.Amount -= thisTransfer;
                    }
                }
            }

            void Push(InvDeclarationRecord from, InvAcceptRecord to)
            {
                if (from.BlockName == to.BlockName)
                    return;
                var leftTotransfer = from.Amount;
                from.Declarer.GetItems(_itemBuffer);

                foreach (var item in _itemBuffer.Where(x => new ItemType(x).Equals(from.ItemType)))
                {
                    _log.Debug("Looking to push ", item.Type.ToString());
                    if (leftTotransfer <= 0)
                        break;

                    var thisTransfer = leftTotransfer > item.Amount ? item.Amount : leftTotransfer;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    //while (!to.Declarer.CanItemsBeAdded(thisTransfer, item.Type) && thisTransfer.RawValue > 0)
                    //    thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName,"->",to.BlockName);
                    //if (thisTransfer > 0)
                    //    _log.Debug(to.Declarer.TransferItemFrom(from.Declarer, item, thisTransfer).ToString());

                    if (to.Declarer.CanItemsBeAdded(thisTransfer, item.Type))
                    {
                        to.Declarer.TransferItemFrom(@from.Declarer, item, thisTransfer);
                        leftTotransfer -= thisTransfer;
                        from.Amount -= thisTransfer;
                    }
                }
            }
        }
    }
}