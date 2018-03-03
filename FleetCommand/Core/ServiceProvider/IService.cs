using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.Core.ServiceProvider
{
    public interface IService
    {
        Type[] Provides { get; }
    }
}
