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
            public string FullPath => $"{Parent?.FullPath ?? ""}.{Name}";
            public OperationGroup Parent { get; set; }
            protected Dictionary<string, IOperationNode> Children { get; }

            public OperationGroup(string name, List<IOperationNode> children, string alias = null)
            {
                Name = name;
                Children = children.ToDictionary(it => it.Name);
                Alias = alias ?? name;
            }

            public OperationContract Find(string fullPath)
            {
                return Children.
            }

            public Dictionary<string, OperationContract> OperationsByPath(string prefix = "")
            {
                var newPrefix = $"{prefix}.{Name}";
                var result = new List<KeyValuePair<string, OperationContract>>();

                foreach (var operationNode in Children.Values)
                {
                    var paths = operationNode.OperationsByPath(newPrefix);
                    result.AddRange(paths);
                }

                return result.ToDictionary(it => it.Key, it => it.Value);
            }
        }
    }
}
