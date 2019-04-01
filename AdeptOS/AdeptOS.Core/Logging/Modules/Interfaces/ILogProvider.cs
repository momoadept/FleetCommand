namespace IngameScript
{
    partial class Program
    {
        public interface ILogProvider
        {
            void Log(LogSeverity severity, params string[] items);
        }
    }
}
