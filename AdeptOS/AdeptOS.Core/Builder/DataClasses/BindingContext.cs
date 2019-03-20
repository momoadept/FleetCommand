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
        public class BindingContext: IBindingContext
        {
            public List<string> Errors => _errors;

            private Dictionary<Type, IModule> _modules;
            private List<string> _errors = new List<string>();

            public BindingContext(Dictionary<Type, IModule> modules)
            {
                _modules = modules;
            }

            public IEnumerable<TModule> Any<TModule>() where TModule : IModule
            {
                throw new NotImplementedException();
            }

            public IEnumerable<TModule> RequireAny<TModule>(IModule caller) where TModule : IModule
            {
                throw new NotImplementedException();
            }

            public TModule One<TModule>() where TModule : IModule
            {
                throw new NotImplementedException();
            }

            public TModule RequireOne<TModule>(IModule caller) where TModule : IModule
            {
                throw new NotImplementedException();
            }
        }
    }
}
