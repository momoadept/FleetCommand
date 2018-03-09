namespace FC.Core.Core.ComponentModel
{
    public interface IStatusReporter
    {
        string StatusEntityId { get; }

        int RefreshStatusDelay { get; }

        string GetStatus();
    }
}