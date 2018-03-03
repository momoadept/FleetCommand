namespace IngameScript.Core.Interfaces
{
        interface ILogProvider
        {
            string LogEntityId { get; }

            ILog Log { get; }
        }
}
