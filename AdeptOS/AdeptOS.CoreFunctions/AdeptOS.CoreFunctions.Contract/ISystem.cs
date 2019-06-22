using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public interface ISystem
        {
            IPromise<Void> ChangeShipNameAndId(string name, string id);

            IPromise<Void> ChangeNodeNameAndId(string name, string id);

            /// <summary>
            /// Returns ShipAlias and ShipId
            /// </summary>
            IPromise<Pair<Primitive<string>, Primitive<string>>> GetShipMeta();

            IPromise<Void> ChangeShipName(string name);

            IPromise<Void> AssignRandomShipId();

            /// <summary>
            /// Sets NodeId to the PB tag
            /// </summary>
            IPromise<Void> UpdatePbTag();

            IPromise<NodeMeta> GetNodeMeta();
        }
    }
}
