using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.ComponentModel;

namespace IngameScript.Core.Logging
{
    public interface ILogFactory
    {
        ILog GetLog(IComponent target);
    }
}
