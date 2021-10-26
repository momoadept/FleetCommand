﻿using Sandbox.Game.EntityComponents;
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
                foreach (var neededBill in _allNeeded)
                    FulfillNeeded(neededBill);

                foreach (var discardBill in _allToDiscard)
                    FulfillNotWanted(discardBill);

                foreach (var acceptBill in _allAccept)
                    FulfillAccept(acceptBill);
            }

            void FulfillAccept(InvAcceptRecord accept)
            {
                _log.Debug("Trying to fill ACCEPT ", accept.ItemType.ToDisplayString(), accept.BlockName);

                var applicableStocks = _allHave
                    .Where(x => new Tag("I", true).NameMatches(x.BlockName))
                    .Where(x => x.Importance < accept.Importance)
                    .Where(x => x.ItemType.Equals(accept.ItemType));

                foreach (var stock in applicableStocks)
                {
                    if (accept.Declarer.IsFull)
                    {
                        _log.Debug("Skipping pulling accepted items since inventory is full");
                        break;
                    }

                    _log.Debug("Found ", stock.Amount.ToString(), "in", stock.BlockName);
                    Pull(stock, accept);
                }
            }

            void FulfillNeeded(InvDeclarationRecord needed)
            {
                //_log.Debug("Trying to fill quote ", needed.ItemType.ToDisplayString(), needed.Amount.ToString(), needed.BlockName);

                // check Unwanted
                //_log.Debug("Looking for unwanted inventory to pull...");
                var applicableDiscarded = _allToDiscard.Where(x => x.ItemType.Equals(needed.ItemType));

                foreach (var discard in applicableDiscarded)
                {
                    if (needed.Amount <= 0)
                        break;

                    if (needed.Declarer.IsFull)
                    {
                        //_log.Debug("Skipping filling quote since inventory is full");
                        break;
                    }

                    //_log.Debug("Found ", discard.Amount.ToString(), "in", discard.BlockName);
                    Pull(discard, needed);
                }

                if (needed.Amount <= 0)
                    return;

                // check Lower priorities
                //_log.Debug("Looking for lower importance inventory to pull...");
                var applicableStocks = _allHave
                    .Where(x => new Tag("I", true).NameMatches(x.BlockName))
                    .Where(x => x.Importance < needed.Importance)
                    .Where(x => x.ItemType.Equals(needed.ItemType));

                foreach (var stock in applicableStocks)
                {
                    if (needed.Amount <= 0)
                        break;

                    if (needed.Declarer.IsFull)
                    {
                        //_log.Debug("Skipping filling quote since inventory is full");
                        break;
                    }

                    //_log.Debug("Found ", stock.Amount.ToString(), "in", stock.BlockName);
                    Pull(stock, needed);
                }
            }

            void FulfillNotWanted(InvDeclarationRecord notWanted)
            {
                //_log.Debug("Trying to discard extra ", notWanted.ItemType.ToDisplayString(), notWanted.Amount.ToString(), notWanted.BlockName);

                // check accepters
                //_log.Debug("Looking for accepting inventory to push...");
                var applicableStocks = _allAccept
                    .Where(x => x.ItemType.Equals(notWanted.ItemType));

                foreach (var declaration in applicableStocks)
                {
                    if (notWanted.Amount <=0)
                        break;

                    //_log.Debug("Pushing to ", declaration.BlockName);
                    Push(notWanted, declaration);
                }
            }

            void Pull(InvDeclarationRecord from, InvDeclarationRecord to)
            {
                if (from.BlockName == to.BlockName)
                    return;
                var ammount = @from.Amount > to.Amount ? to.Amount : @from.Amount;
                @from.Declarer.GetItems(_itemBuffer, x => new ItemType(x).Equals(to.ItemType));

                foreach (var item in _itemBuffer)
                {
                    //_log.Debug("Looking at ", item.Type.SubtypeId, item.Amount.SerializeString());
                    if (ammount <= 0)
                        break;

                    var thisTransfer = ammount > item.Amount ? item.Amount : ammount;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    while (!to.Declarer.CanItemsBeAdded(thisTransfer, item.Type) || thisTransfer.RawValue == 0)
                        thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName, "->", to.BlockName);
                    _log.Debug(to.Declarer.TransferItemFrom(@from.Declarer, item, thisTransfer).ToString());

                    ammount -= thisTransfer;
                    @from.Amount -= thisTransfer;
                    to.Amount -= thisTransfer;
                }
            }

            void Pull(InvDeclarationRecord from, InvAcceptRecord to)
            {
                if (from.BlockName == to.BlockName)
                    return;
                var ammount = from.Amount;
                from.Declarer.GetItems(_itemBuffer, x => new ItemType(x).Equals(to.ItemType));

                foreach (var item in _itemBuffer)
                {
                    //_log.Debug("Looking at ", item.Type.SubtypeId, item.Amount.SerializeString());
                    if (ammount <= 0 || to.Declarer.IsFull)
                        break;

                    var thisTransfer = ammount > item.Amount ? item.Amount : ammount;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    while (!to.Declarer.CanItemsBeAdded(thisTransfer, item.Type) || thisTransfer.RawValue == 0)
                        thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName, "->", to.BlockName);
                    _log.Debug(to.Declarer.TransferItemFrom(from.Declarer, item, thisTransfer).ToString());

                    ammount -= thisTransfer;
                    from.Amount-= thisTransfer;
                }
            }

            void Push(InvDeclarationRecord from, InvAcceptRecord to)
            {
                if (from.BlockName == to.BlockName)
                    return;
                var ammount = from.Amount;
                from.Declarer.GetItems(_itemBuffer, x => new ItemType(x).Equals(from.ItemType));

                foreach (var item in _itemBuffer)
                {
                    if (ammount <= 0)
                        break;

                    var thisTransfer = ammount > item.Amount ? item.Amount : ammount;
                    //_log.Debug("Trying to transfer ", thisTransfer.SerializeString());

                    while (!to.Declarer.CanItemsBeAdded(thisTransfer, item.Type) || thisTransfer.RawValue == 0)
                        thisTransfer.RawValue /= 2;

                    _log.Debug("Transferring ", thisTransfer.SerializeString(), item.Type.ToString());
                    _log.Debug(from.BlockName,"->",to.BlockName);
                    _log.Debug(to.Declarer.TransferItemFrom(from.Declarer, item, thisTransfer).ToString());

                    ammount -= thisTransfer;
                    from.Amount -= thisTransfer;
                }
            }
        }
    }
}