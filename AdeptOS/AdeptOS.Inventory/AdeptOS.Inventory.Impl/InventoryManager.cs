using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class InventoryManager : IInventoryManager, IModule
        {
            public IPromise<Void> ForceUpdatePolicies()
            {
                return Void.Promise();
            }

            public IPromise<Void> SetState(InventoryManagerState value)
            {
                _state = value;
                return Void.Promise();
            }

            private InventoryManagerState _state;

            private ILog _log;

            public string UniqueName => "INV_impl";
            public string Alias => "InventoryManagerImplementation";

            public void Bind(IBindingContext context)
            {
                _log = context.RequireOne<ILog>(this);
            }

            public void Restore(InventoryManagerState state)
            {
                _state = state;
                _log.Info("Inventory Manager Restored");
            }

            public InventoryManagerState GetState()
            {
                return _state;
            }

            public void Run()
            {
                _log.Info("Inventory Manager Started");
                _log.Info("My state is", _state.Stringify());
            }

            public void OnSaving()
            {
                _log.Info("Inventory Manager Saving");
            }
        }
    }
}
