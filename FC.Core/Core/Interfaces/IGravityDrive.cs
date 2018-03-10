using System;
using System.Collections.Generic;
using System.Text;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync.Promises;

namespace FC.Core.Core.Interfaces
{
    public interface IGravityDrive: IStatefull<OnOff>
    {
        Promise Enable();
        Promise Disable();
        Promise SetStrength();
    }
}
