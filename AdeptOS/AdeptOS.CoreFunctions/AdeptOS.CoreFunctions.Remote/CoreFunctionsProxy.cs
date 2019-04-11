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
        public class CoreFunctionsProxy: Proxy, ICoreFunctions
        {
            public override string UniqueName { get; }
            public override string Alias { get; }
            protected override Tag ImplementationTag { get; }
            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeShipNameAndId { get; }
            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeNodeNameAndId { get; }
            public IActionContract<Primitive<string>, Void> ChangeShipName { get; }
            public IActionContract<Void, Void> AssignRandomShipId { get; }
            public IActionContract<Void, Void> UpdatePbTag { get; }
        }
    }
}
