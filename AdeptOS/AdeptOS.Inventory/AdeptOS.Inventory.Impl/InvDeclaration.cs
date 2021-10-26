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
using VRage.ObjectBuilders;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class InvDeclaration
        {
            public Dictionary<string, InvDeclarationRecord> Have = new Dictionary<string, InvDeclarationRecord>();

            /// <summary>
            /// Items needed to fill quote
            /// </summary>
            public Dictionary<string, InvDeclarationRecord> Want = new Dictionary<string, InvDeclarationRecord>();

            /// <summary>
            /// Items over the quote or not in list
            /// </summary>
            public Dictionary<string, InvDeclarationRecord> DontWant = new Dictionary<string, InvDeclarationRecord>();

            /// <summary>
            /// Don't have quote for these but can accept
            /// </summary>
            public Dictionary<string, InvAcceptRecord> Accept = new Dictionary<string, InvAcceptRecord>();

            /// <summary>
            /// Item priority for accepting
            /// </summary>
            public Dictionary<string, int> ItemTypeImportance;

            public IMyInventory Declarer;

            public InvDeclaration(IMyInventory declarer)
            {
                Declarer = declarer;
            }
        }

        public class InvAcceptRecord
        {
            public int Importance;
            public ItemType ItemType;
            public IMyInventory Declarer;

            public InvAcceptRecord(int importance, ItemType itemType, IMyInventory declarer, string blockName)
            {
                Importance = importance;
                ItemType = itemType;
                Declarer = declarer;
                BlockName = blockName;
            }

            public string BlockName;
        }

        public class InvDeclarationRecord
        {
            public MyFixedPoint Amount;
            public int Importance;
            public ItemType ItemType;
            public IMyInventory Declarer;
            public string BlockName;

            public InvDeclarationRecord(ItemType itemType, MyFixedPoint amount, int importance, IMyInventory declarer, string blockName)
            {
                Amount = amount;
                Importance = importance;
                ItemType = itemType;
                Declarer = declarer;
                BlockName = blockName;
            }
        }
    }
}
