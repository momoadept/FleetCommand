using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
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
        public static class Contract
        {
            public static ActionContract<TArgument, TResult> Action<TArgument, TResult>(
                Dictionary<string, IActionContract> actions, 
                string name, 
                Func<TArgument, IPromise<TResult>> impl, 
                bool noArgument = false)
                where TResult : class, IStringifiable, new()
                where TArgument : class, IStringifiable, new()
            {
                var contract = new ActionContract<TArgument, TResult>(name, impl, noArgument);
                actions.Add("name", contract);
                return contract;
            }
        }
    }
}
