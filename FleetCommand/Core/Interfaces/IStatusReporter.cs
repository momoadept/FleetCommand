namespace IngameScript.Core.Interfaces
{
    public interface IStatusReporter
    {
        string StatusEntityId { get; }

        int RefreshStatusDelay { get; }

        string GetStatus();
    }
}