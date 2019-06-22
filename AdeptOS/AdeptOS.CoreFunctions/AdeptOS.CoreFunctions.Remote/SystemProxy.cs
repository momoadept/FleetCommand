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
        public class SystemProxy: Proxy, ISystem
        {
            public override string UniqueName { get; } = "System";
            public override string Alias { get; } = "System";
            protected override Tag ImplementationTag { get; }

            public SystemProxy(Tag implementationTag)
            {
                ImplementationTag = implementationTag;
                Remote<Pair<Primitive<string>, Primitive<string>>, Void>("ChangeShipNameAndId");
                Remote<Pair<Primitive<string>, Primitive<string>>, Void>("ChangeShipNameAndId");
                Remote<Void, Pair<Primitive<string>, Primitive<string>>>("GetShipMeta");
            }

            public IPromise<Void> ChangeShipNameAndId(string name, string id)
            {
                return (IPromise<Void>) Actions["ChangeShipNameAndId"]
                    .Do(new Pair<Primitive<string>, Primitive<string>>(name.AsPrimitive(), id.AsPrimitive()));
            }

            public IPromise<Void> ChangeNodeNameAndId(string name, string id)
            {
                return (IPromise<Void>)Actions["ChangeNodeNameAndId"]
                    .Do(new Pair<Primitive<string>, Primitive<string>>(name.AsPrimitive(), id.AsPrimitive()));
            }

            public IPromise<Pair<Primitive<string>, Primitive<string>>> GetShipMeta()
            {
                return (IPromise<Pair<Primitive<string>, Primitive<string>>>) Actions["GetShipMeta"]
                    .Do(new Void());
            }

            public IPromise<Void> ChangeShipName(string name)
            {
                throw new NotImplementedException();
            }

            public IPromise<Void> AssignRandomShipId()
            {
                throw new NotImplementedException();
            }

            public IPromise<Void> UpdatePbTag()
            {
                throw new NotImplementedException();
            }

            public IPromise<NodeMeta> GetNodeMeta()
            {
                throw new NotImplementedException();
            }
        }
    }
}
