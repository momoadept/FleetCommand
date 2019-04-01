namespace IngameScript
{
    partial class Program
    {
        public interface ILog
        {
            void Debug(params string[] items);
            void Info(params string[] items);
            void Warning(params string[] items);
            void Error(params string[] items);
            void Fatal(params string[] items);
        }
    }
}
