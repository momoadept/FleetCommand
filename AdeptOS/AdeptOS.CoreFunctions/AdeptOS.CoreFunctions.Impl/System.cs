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
        public class Systems
        {
            IGameContext _gameContext;
            IMessageSender _messageSender;
            ILog _log;

            public Systems(IGameContext gameContext, IMessageSender messageSender, ILog log)
            {
                _gameContext = gameContext;
                _messageSender = messageSender;
                _log = log;
            }

            public void Init()
            {
                _gameContext.Me.UpdateName(Aos.Node.ShipId);
                var nodeTag = new Tag(Aos.Node.NodeId);
                if (!nodeTag.InName(_gameContext.Me.CustomName))
                    _gameContext.Me.CustomName = nodeTag.AddToName(_gameContext.Me.CustomName);

                _log.Debug("Systems functions started OK");
            }

            public IPromise<Void> ChangeShipNameAndId(string name, string id)
            {
                try
                {
                    var oldId = Aos.Node.ShipId;
                    Aos.Node.ShipAlias = name;
                    Aos.Node.ShipId = id;
                    _gameContext.Me.UpdateName(Aos.Node.ShipId);

                    if (Aos.Node.IsMainNode)
                    {
                        var nameAndId = new Pair<Primitive<string>, Primitive<string>>
                        {
                            First = name.AsPrimitive(),
                            Second = id.AsPrimitive()
                        };

                        ShipBroadcast($"T|Systems.ChangeShipNameAndId|{{{nameAndId.Stringify()}}}", oldId);
                    }

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

            public IPromise<Void> ChangeNodeNameAndId(string name, string id)
            {
                Aos.Node.NodeAlias = name;
                Aos.Node.NodeId = id;
                return Void.Promise();
            }

            public IPromise<Pair<Primitive<string>, Primitive<string>>> GetShipMeta()
            {
                return Promise<Pair<Primitive<string>, Primitive<string>>>.FromValue(
                    new Pair<Primitive<string>, Primitive<string>>
                    {
                        First = Aos.Node.ShipAlias.AsPrimitive(),
                        Second = Aos.Node.ShipId.AsPrimitive()
                    });
            }
        }
    }
}
