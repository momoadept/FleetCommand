using System.Collections.Generic;
using FC.Core.Core.Delegates;
using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core.BlockLoader
{
    public interface IBlockLoader
    {
        List<IMyTerminalBlock> Blocks { get; }
        List<IMyBlockGroup> Groups { get; }

        event Event.Handler<IBlockLoader> Updated;
    }
}