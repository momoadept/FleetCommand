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
            public Dictionary<string, InvDeclarationRecord> Want = new Dictionary<string, InvDeclarationRecord>();
            public Dictionary<string, InvDeclarationRecord> DontWant = new Dictionary<string, InvDeclarationRecord>();
            public IMyInventory Declarer;

            public InvDeclaration(IMyInventory declarer)
            {
                Declarer = declarer;
            }
        }

        public class InvDeclarationRecord
        {
            public MyFixedPoint Amount;
            public int Importance;
            public ItemType ItemType;
            public IMyInventory Declarer;

            public InvDeclarationRecord(ItemType itemType, MyFixedPoint amount, int importance, IMyInventory declarer)
            {
                Amount = amount;
                Importance = importance;
                ItemType = itemType;
                Declarer = declarer;
            }
        }
    }
}
