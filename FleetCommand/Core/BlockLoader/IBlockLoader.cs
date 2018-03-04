using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public interface IBlockLoader
    {
        List<IMyTerminalBlock> Blocks { get; }
        List<IMyBlockGroup> Groups { get; }
    }
}