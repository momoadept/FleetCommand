using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class OperationGroup: IOperationNode
        {
            public string Name { get; }
            public string Alias { get; }
            public Dictionary<string, IOperationNode> Children { get; }

            public OperationGroup(string name, List<IOperationNode> children, string alias = null)
            {
                Name = name;
                Children = children.ToDictionary(it => it.Name);
                Alias = alias ?? name;
            }
        }
    }
}
