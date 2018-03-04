using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.FakeAsync;

namespace IngameScript.Core.Delegates
{
    public static class Event
    {
        public delegate void Handler<in T>(T arg);
    }
}
