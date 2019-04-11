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
        public class CoreFunctions: IModule, IControllable, ICoreFunctions
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public string UniqueName { get; }
            public string Alias { get; }

            public CoreFunctions()
            {
                ChangeShipNameAndId = Contract.Action<Pair<Primitive<string>, Primitive<string>>, Void>(Actions, "ChangeShipNameAndId", _ChangeShipNameAndId);
                ChangeNodeNameAndId = Contract.Action<Pair<Primitive<string>, Primitive<string>>, Void>(Actions, "ChangeNodeNameAndId", _ChangeNodeNameAndId);
                ChangeShipName = Contract.Action<Primitive<string>, Void>(Actions, "ChangeShipName", _ChangeShipName);
                AssignRandomShipId = Contract.Action<Void, Void>(Actions, "AssignRandomShipId", _AssignRandomShipId);
                UpdatePbTag = Contract.Action<Void, Void>(Actions, "UpdatePbTag", _UpdatePbTag);
            }

            public void Bind(IBindingContext context)
            {
            }

            public void Run()
            {
            }

            public void OnSaving()
            {
            }

            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeShipNameAndId { get; }
            IPromise<Void> _ChangeShipNameAndId(Pair<Primitive<string>, Primitive<string>> nameAndId)
            {
                return Void.Promise();
            }

            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeNodeNameAndId { get; }
            IPromise<Void> _ChangeNodeNameAndId(Pair<Primitive<string>, Primitive<string>> nameAndId)
            {
                return Void.Promise();
            }

            public IActionContract<Primitive<string>, Void> ChangeShipName { get; }
            IPromise<Void> _ChangeShipName(Primitive<string> name)
            {
                return Void.Promise();
            }

            public IActionContract<Void, Void> AssignRandomShipId { get; }
            IPromise<Void> _AssignRandomShipId(Void a)
            {
                return Void.Promise();
            }

            public IActionContract<Void, Void> UpdatePbTag { get; }
            IPromise<Void> _UpdatePbTag(Void a)
            {
                return Void.Promise();
            }

        }
    }
}
