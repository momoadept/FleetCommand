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
        public class InventoryManagerController : StorableModule<InventoryManagerController>, IControllable, IInventoryManager
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public override string UniqueName => "INV";

            public override string Alias => "Inventory Manager";

            private InventoryManagerState _state = new InventoryManagerState();
            private InventoryManager _impl;

            public override void Bind(IBindingContext context)
            {
                _impl = new InventoryManager();
                _impl.Bind(context);
            }

            public override void Run()
            {
                _impl.Restore(_state);
                _impl.Run();
            }

            public InventoryManagerController()
             : base(new List<Property<InventoryManagerController>>(new[]
                 { 
                     new Property<InventoryManagerController>(
                     "state", 
                     x => x._state, 
                     (x, s) => x._state.Restore(s))
                 }))
            {
                Actions.Add<InventoryManagerState, Void>("SetState", SetState);
                Actions.Add<Void, Void>("ForceUpdatePolicies", v => ForceUpdatePolicies());
            }

            //T|INV.ForceUpdatePolicies|{}
            public IPromise<Void> ForceUpdatePolicies()
            {
                return _impl.ForceUpdatePolicies();
            }

            //T|INV.SetState|{SomeString:{test},SomeBool:{true}}
            public IPromise<Void> SetState(InventoryManagerState value)
            {
                return _impl.SetState(value);
            }

            public override void OnSaving()
            {
                _state = _impl.GetState();
            }
        }
    }
}
