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
        public interface IPromise<TResult>
        {
            IPromise<TResult> Then(Action<TResult> callback);

            IPromise<TResult> Catch(Action<Exception> handler);

            IPromise<TResult> Finally(Action handler);

            /// <summary>
            /// Pass a promise gerenator. It will be launched when current promise resolves or failed if current fails.
            /// </summary>
            /// <returns>
            /// Returns a new promise
            /// </returns>
            IPromise<TNewResult> Next<TNewResult>(Func<IPromise<TNewResult>> nextPromiseGenerator);
        }
    }
}
