namespace IngameScript.Core.Interfaces
{
        public interface IStatusReporter
        {
            string StatusEntityId { get; }

            string GetStatus();

            int RefreshStatusDelay { get; }
        }
}
