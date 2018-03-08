using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.Core.ComponentModel
{
    public interface IMessageProcessor
    {
        bool ProcessMessage(ComponentMessage message);
    }
}
