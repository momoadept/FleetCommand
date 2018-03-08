using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Logging;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public interface IBlockReferenceFactory
    {
        TagBlockReference<T> GetReference<T>(string tag) where T: IMyTerminalBlock;
    }
}
