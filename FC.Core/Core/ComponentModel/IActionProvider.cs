using FC.Core.Core.FakeAsync.Promises;

namespace FC.Core.Core.ComponentModel
{
    public interface IActionProvider
    {
        Promise<string> Invoke(string action, string[] args);
        bool CanInvoke(string action);
        string ActionProviderId { get; }
    }
}
