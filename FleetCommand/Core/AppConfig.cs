using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Enums;

namespace IngameScript.Core
{
    public class AppConfig
    {
        // Logging
        public bool EnableMasterLog { get; set; } = false;
        public LogType MasterLogTypes { get; set; } = LogType.All;
        public LogType CommonLogTypes { get; set; } = LogType.All;
    }
}
