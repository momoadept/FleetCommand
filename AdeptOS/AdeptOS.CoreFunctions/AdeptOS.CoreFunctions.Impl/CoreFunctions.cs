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
            public string UniqueName => "System";
            public string Alias => "System";

            IGameContext _gameContext;
            IMessageSender _messageSender;
            ILog _log;

            public CoreFunctions()
            {
                ChangeShipNameAndId = Contract.Action<Pair<Primitive<string>, Primitive<string>>, Void>(Actions, "ChangeShipNameAndId", _ChangeShipNameAndId);
                ChangeNodeNameAndId = Contract.Action<Pair<Primitive<string>, Primitive<string>>, Void>(Actions, "ChangeNodeNameAndId", _ChangeNodeNameAndId);
                ChangeShipName = Contract.Action<Primitive<string>, Void>(Actions, "ChangeShipName", _ChangeShipName);
                AssignRandomShipId = Contract.Action<Void, Void>(Actions, "AssignRandomShipId", _AssignRandomShipId);
                UpdatePbTag = Contract.Action<Void, Void>(Actions, "UpdatePbTag", _UpdatePbTag);
                GetShipMeta = Contract.Action<Void, Pair<Primitive<string>, Primitive<string>>>(Actions, "GetShipMeta", _GetShipMeta);
                GetNodeMeta = Contract.Action<Void, NodeMeta>(Actions, "GetNodeMeta", _GetNodeMeta);
            }

            public void Bind(IBindingContext context)
            {
                _gameContext = context.RequireOne<IGameContext>(this);
                _messageSender = context.RequireOne<IMessageSender>(this);
                _log = context.RequireOne<ILog>(this);
            }

            public void Run()
            {
                _gameContext.Me.UpdateName(Aos.Node.ShipId);
                var nodeTag = new Tag(Aos.Node.NodeId);
                if (!nodeTag.InName(_gameContext.Me.CustomName))
                    _gameContext.Me.CustomName = nodeTag.AddToName(_gameContext.Me.CustomName);
            }

            public void OnSaving()
            {
            }

            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeShipNameAndId { get; }
            IPromise<Void> _ChangeShipNameAndId(Pair<Primitive<string>, Primitive<string>> nameAndId)
            {
                try
                {
                    var oldId = Aos.Node.ShipId;
                    Aos.Node.ShipAlias = nameAndId.First.Value;
                    Aos.Node.ShipId = nameAndId.Second.Value;
                    _gameContext.Me.UpdateName(Aos.Node.ShipId);

                    if (Aos.Node.IsMainNode)
                        ShipBroadcast($"T|System.ChangeShipNameAndId|{{{nameAndId.Stringify()}}}", oldId);

                    _log.Info($"Renamed {oldId} to {Aos.Node.ShipAlias}-{Aos.Node.ShipId}");

                    return Void.Promise();
                }
                catch (Exception e)
                {
                    _log.Error($"Rename failed: {e}");
                    return Promise<Void>.FromError(e);
                }
            }

            void ShipBroadcast(string message, string shipId)
            {
                var blocks = new List<IMyProgrammableBlock>();
                _gameContext.Grid.GetBlocksOfType(blocks);

                foreach (var pb in blocks.Where(it => it.CustomName.Contains($"({shipId})")))
                    _messageSender.DispatchMessage(pb, message);
            }

            public IActionContract<Pair<Primitive<string>, Primitive<string>>, Void> ChangeNodeNameAndId { get; }
            IPromise<Void> _ChangeNodeNameAndId(Pair<Primitive<string>, Primitive<string>> nameAndId)
            {
                Aos.Node.NodeAlias = nameAndId.First.Value;
                Aos.Node.NodeId = nameAndId.Second.Value;
                return Void.Promise();
            }

            public IActionContract<Primitive<string>, Void> ChangeShipName { get; }
            IPromise<Void> _ChangeShipName(Primitive<string> name)
            {
                var oldName = Aos.Node.ShipAlias;
                Aos.Node.ShipAlias = name.Value;
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

            public IActionContract<Void, Pair<Primitive<string>, Primitive<string>>> GetShipMeta { get; }
            IPromise<Pair<Primitive<string>, Primitive<string>>> _GetShipMeta(Void a = null)
            {
                return Promise<Pair<Primitive<string>, Primitive<string>>>.FromValue(
                    new Pair<Primitive<string>, Primitive<string>>
                    {
                        First = Aos.Node.ShipAlias.AsPrimitive(),
                        Second = Aos.Node.ShipId.AsPrimitive()
                    });
            }

            public IActionContract<Void, NodeMeta> GetNodeMeta { get; }
            IPromise<NodeMeta> _GetNodeMeta(Void a = null)
            {
                return Promise<NodeMeta>.FromValue(new NodeMeta()
                {
                    Id = Aos.Node.NodeId,
                    Alias = Aos.Node.NodeAlias,
                    IsMain = Aos.Node.IsMainNode
                });
            }
        }
    }
}
