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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class SystemController: IModule, ISystem, IControllable
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public string UniqueName { get; } = "Systems";
            public string Alias { get; } = "Systems";

            Systems _systems;

            public SystemController()
            {
                Actions
                    .Add<Pair<Primitive<string>, Primitive<string>>, Void>(
                        "ChangeShipNameAndId",
                        arg => ChangeShipNameAndId(arg.First.Value, arg.Second.Value))
                    .Add<Pair<Primitive<string>, Primitive<string>>, Void>(
                        "ChangeNodeNameAndId",
                        arg => ChangeNodeNameAndId(arg.First.Value, arg.Second.Value))
                    .Add<Void, Pair<Primitive<string>, Primitive<string>>>(
                        "GetShipMeta",
                        arg => GetShipMeta());
            }

            public void Bind(IBindingContext context) => _systems = new Systems(context.RequireOne<IGameContext>(), context.RequireOne<IMessageSender>(), context.RequireOne<ILog>());

            public void Run() => _systems.Init();

            public void OnSaving()
            {
            }

            public IPromise<Void> ChangeShipNameAndId(string name, string id) => _systems.ChangeShipNameAndId(name, id);
            public IPromise<Void> ChangeNodeNameAndId(string name, string id) => _systems.ChangeNodeNameAndId(name, id);
            public IPromise<Pair<Primitive<string>, Primitive<string>>> GetShipMeta() => _systems.GetShipMeta();
            public IPromise<Void> ChangeShipName(string name) => Void.Promise();

            public IPromise<Void> AssignRandomShipId() => Void.Promise();

            public IPromise<Void> UpdatePbTag() => Void.Promise();

            public IPromise<NodeMeta> GetNodeMeta() => Promise<NodeMeta>.FromValue(new NodeMeta());
        }
    }
}
