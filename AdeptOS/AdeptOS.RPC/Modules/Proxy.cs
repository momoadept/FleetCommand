using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using System.Reflection;
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
        public abstract class Proxy: IModule, IControllable
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public abstract string UniqueName { get; }
            public abstract string Alias { get; }
            protected abstract Tag ImplementationTag { get; }

            IRPC _rpc;

            public virtual void Bind(IBindingContext context)
            {
                _rpc = context.RequireOne<IRPC>(this);
            }

            public void Run() { }

            public void OnSaving() { }

            protected void Remote<TArgument, TResult>(string name, bool noArgument = false) 
                where TResult : class, IStringifiable, new() 
                where TArgument : class, IStringifiable, new()
            {
                var contract =  new RemoteActionContract<TArgument, TResult>(_rpc, ImplementationTag, UniqueName, name, noArgument);
                Actions.Add(name, contract);
            }
        }
    }
}
