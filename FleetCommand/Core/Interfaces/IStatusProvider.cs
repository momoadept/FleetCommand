namespace IngameScript.Core.Interfaces
{
        public interface IStatusProvider
        {
            string StatusEntityId { get; }

            string GetStatus();
        }
}
