using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    partial class Program
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
}
