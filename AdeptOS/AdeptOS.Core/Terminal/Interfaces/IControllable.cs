using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public interface IControllable
        {
            Dictionary<string, IActionContract> Actions { get; }
            string UniqueName { get; }
        }
    }
}
