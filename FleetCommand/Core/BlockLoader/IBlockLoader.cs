using System.Collections.Generic;
using IngameScript.Core.Interfaces;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockLoader
{
    public interface IBlockLoader
    {
        List<IMyTerminalBlock> Blocks { get; }
        List<IMyBlockGroup> Groups { get; }
    }
}