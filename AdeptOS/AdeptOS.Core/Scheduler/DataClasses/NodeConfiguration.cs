using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class NodeConfiguration
        {
            public string ShipId;
            public string ShipAlias;

            public string NodeId;
            public string NodeAlias;

            public bool IsMainNode;
            public string MainNodeId;

            public List<IModule> Modules;
        }
    }
}
