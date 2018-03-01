using System;
using System.Collections.Generic;
using System.Text;

namespace FleetCommand.Framework.Async
{
    public class Async: ISystem
    {
        public List<IAsyncOperation> ScheduledTasks { get; } = new List<IAsyncOperation>();

        public void Tick
    }
}
