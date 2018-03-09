using IngameScript.Core.FakeAsync.Promises;

namespace IngameScript.Core.ComponentModel
{
    public interface IStatefull<TState>
    {
        Promise<TState> GetState();
    }
}
