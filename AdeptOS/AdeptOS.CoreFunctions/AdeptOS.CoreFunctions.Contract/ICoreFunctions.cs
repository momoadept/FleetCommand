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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Provides baseline system actions
        /// </summary>
        public interface ICoreFunctions
        {
            IActionContract<
                    Pair<Primitive<string>, Primitive<string>>,
                    Void>
                ChangeShipNameAndId { get; }

            IActionContract<
                    Pair<Primitive<string>, Primitive<string>>,
                    Void>
                ChangeNodeNameAndId { get; }

            IActionContract<Primitive<string>, Void> ChangeShipName { get; }
            IActionContract<Void, Void> AssignRandomShipId { get; }

            /// <summary>
            /// Sets NodeId to the PB tag
            /// </summary>
            IActionContract<Void, Void> UpdatePbTag { get; }

            /// <summary>
            /// Returns ShipAlias and ShipId
            /// </summary>
            IActionContract<
                    Void,
                    Pair<Primitive<string>, Primitive<string>>>
                GetShipMeta { get; }

            IActionContract<
                    Void,
                    NodeMeta>
                GetNodeMeta { get; }
        }
    }
}
