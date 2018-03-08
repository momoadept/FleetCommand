using System.Collections.Generic;
using IngameScript.Core.Delegates;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public interface IBlockLoader
    {
        List<IMyTerminalBlock> Blocks { get; }
        List<IMyBlockGroup> Groups { get; }

        event Event.Handler<IBlockLoader> Updated;
    }
}