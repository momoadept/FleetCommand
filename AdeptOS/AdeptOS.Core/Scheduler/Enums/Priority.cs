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
        public enum Priority
        {
            /// <summary>
            /// Guaranteed to execute when scheduled
            /// </summary>
            Critical = 1,

            /// <summary>
            /// Can be delayed, but always runs before unimportant
            /// </summary>
            Routine = 100,

            /// <summary>
            /// Run only if have time
            /// </summary>
            Unimportant = 5000
        }
    }
}
