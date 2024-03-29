﻿using Sandbox.Game.EntityComponents;
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
        #region mdk preserve
        public class Void : IStringifiable
            #endregion
        {
            public string Stringify() => "{}";

            public void Restore(string value)
            {
            }

            public static IPromise<Void> Promise() => Promise<Void>.FromValue(new Void());

            public override string ToString() => "VOID";
        }
    }
}
