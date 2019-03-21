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
        public class Builder
        {
            private IEnumerable<IModule> _modules;
            public void BindModules(IEnumerable<IModule> modules)
            {
                _modules = modules;
                var bindingContext = new BindingContext(modules);

                foreach (var module in modules)
                    module.Bind(bindingContext);
            }

            public void RunModules()
            {
                foreach (var module in _modules)
                    module.Run();
            }
        }
    }
}
