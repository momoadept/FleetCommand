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
            private IEnumerable<IModule> _modules;

            public BindingContext(IEnumerable<IModule> modules)
            {
                _modules = modules;
            }

            public IEnumerable<TModule> Any<TModule>() where TModule : IModule
            {
                return Get<TModule>();
            }

            public IEnumerable<TModule> RequireAny<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if(!modules.Any())
                    throw new BindingException($"Module {caller.UniqueName} needs one or more implementations of {typeof(TModule).Name} but there aren't any in this package");

                return modules;
            }

            public TModule One<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if (modules.Count() > 1)
                    throw new BindingException($"Module {caller.UniqueName} needs one optional implementation of {typeof(TModule).Name} but there are {modules.Count()} in this package");

                return modules.FirstOrDefault();
            }

            public TModule RequireOne<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if (!modules.Any())
                    throw new BindingException($"Module {caller.UniqueName} needs one implementation of {typeof(TModule).Name} but there aren't any in this package");

                if (modules.Count() > 1)
                    throw new BindingException($"Module {caller.UniqueName} needs one implementation of {typeof(TModule).Name} but there are {modules.Count()} in this package");

                return modules.First();
            }

            private IEnumerable<TModule> Get<TModule>()
            {
                return _modules.Where(it => it is TModule).Cast<TModule>();
            }
        }
    }
}
