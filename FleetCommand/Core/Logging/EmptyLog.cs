using IngameScript.Core.Enums;
using IngameScript.Core.Interfaces;

namespace IngameScript.Core.Logging
{
    public class EmptyLog : ILog
    {
        public void Log(string entry, LogType logType = LogType.Info)
        {
        }

        public void Clear()
        {
        }
    }
}