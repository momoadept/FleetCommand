using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync;

namespace IngameScript.Core.Testing
{
    public class Ticker: BaseComponent
    {
        public Ticker() : base("Ticker")
        {
        }

        public override void OnAttached(App app)
        {
        }
    }
}
