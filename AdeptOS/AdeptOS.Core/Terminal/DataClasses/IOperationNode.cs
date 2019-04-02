using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public interface IOperationNode
        {
            string Name { get; }
            string Alias { get; }
            string FullPath { get; }
            OperationContract Find(string fullPath);
            Dictionary<string, OperationContract> OperationsByPath(string prefix = "");
            OperationGroup Parent { get; }
        }
    }
}
