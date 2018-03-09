using Sandbox.ModAPI.Ingame;

namespace FC.Core.Core.BlockReferences
{
    public interface IBlockReferenceFactory
    {
        TagBlockReference<T> GetReference<T>(string tag) where T: IMyTerminalBlock;
    }
}
