using System;
using System.Collections.Generic;
using System.Text;

namespace FleetCommand.Framework
{
    public interface ISystem
    {
        void Tick(int currentTime);
    }
}
