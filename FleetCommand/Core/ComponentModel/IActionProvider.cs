using IngameScript.Core.FakeAsync.Promises;

namespace IngameScript.Core.ComponentModel
{
    public interface IActionProvider
    {
        Promise<string> Invoke(string action, string[] args);
        string ActionProviderId { get; }
    }
}
