using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public interface IBlockReferenceFactory
    {
        TagBlockReference<T> GetReference<T>(string tag) where T: IMyTerminalBlock;
    }
}
