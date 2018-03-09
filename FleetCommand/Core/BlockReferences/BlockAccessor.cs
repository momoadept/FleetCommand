using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public class BlockAccessor<T> where T: IMyTerminalBlock
    {
        public BlockAccessor(T block, List<string> arguments)
        {
            Block = block;
            Arguments = arguments;
        }

        public T Block { get; }
        public List<string> Arguments { get; }
    }
}
