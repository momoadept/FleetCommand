using FC.Core.Core.FakeAsync.Promises;

namespace FC.Core.Core.ComponentModel
{
    public interface IStatefull<TState>
    {
        Promise<TState> GetState();
    }
}
