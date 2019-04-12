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
        public class NodeMeta: BaseDataObject<NodeMeta>
        {
            public string Id;
            public string Alias;
            public bool IsMain;

            static List<Property<NodeMeta>> _mapping = new List<Property<NodeMeta>>
            {
                new Property<NodeMeta>("Id", it => it.Id, (it, id) => it.Id = id),
                new Property<NodeMeta>("Alias", it => it.Alias, (it, alias) => it.Alias = alias),
                new Property<NodeMeta>("IsMain", it => new Primitive<bool>(it.IsMain), (it, isMain) => it.IsMain = new Primitive<bool>().Retrieve(isMain)),
            };

            public NodeMeta() : base(_mapping)
            {
            }
        }
    }
}
