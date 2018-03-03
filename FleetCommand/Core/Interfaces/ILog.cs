using IngameScript.Core.Enums;

namespace IngameScript.Core.Interfaces
{
    public interface ILog
    {
        void Log(string entry, LogType logType = LogType.Info);
        void Clear();
    }
}