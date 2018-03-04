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
        public List<LogType> MasterLogTypes { get; set; } = new List<LogType>()
        {
            LogType.Debug,
            LogType.Error,
            LogType.Info,
            LogType.Warning,
            LogType.Priority
        };
        public string MasterLogTag { get; set; } = App.BlockTag("MasterLog");
        public List<LogType> CommonLogTypes { get; set; } = new List<LogType>()
        {
            LogType.Debug,
            LogType.Error,
            LogType.Info,
            LogType.Warning,
            LogType.Priority
        };
    }
}
