using System;

namespace FC.Core.Core.Enums
{
    [Flags]
    public enum LogType
    {
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        Priority = 16,
        All = Debug + Info + Warning + Error + Priority,
        None = 0
    }
}