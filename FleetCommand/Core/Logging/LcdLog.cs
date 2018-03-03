using IngameScript.Core.BlockLoader;
using IngameScript.Core.Enums;
using IngameScript.Core.Interfaces;
using IngameScript.Core.ServiceProvider;

namespace IngameScript.Core.Logging
{
        public class LcdLog: ILog
        {
            private readonly IBlockLoader blocks;

            public LcdLog(IMyServiceProvider services)
            {
                blocks = services.Get<IBlockLoader>();
            }

            public void Log(string entry, LogType logType = LogType.Info)
            {
            }

            public void Clear()
            {
            }
        }
}
