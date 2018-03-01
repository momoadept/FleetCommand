namespace IngameScript
{
    partial class Program
    {
        public interface ILog
        {
            void Log(string entry, LogType logType = LogType.Info);
            void Clear();
        }
    }
}