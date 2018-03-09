using FC.Core.Core.ComponentModel;

namespace FC.Core.Core.Logging
{
    public interface ILogFactory
    {
        ILog GetLog(IComponent target);
    }
}
