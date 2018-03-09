using IngameScript.Core.ComponentModel;

namespace IngameScript.Core.Logging
{
    public interface ILogFactory
    {
        ILog GetLog(IComponent target);
    }
}
