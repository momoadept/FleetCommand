using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Logging;

namespace IngameScript.Core.ComponentModel
{
    public class BaseComponent: IComponent
    {
        public string ComponentId { get; }
        protected ILog Log { get; }

        public BaseComponent(string componentId)
        {
            ComponentId = componentId;
            Log = new LcdLog(componentId);
        }
        
        public virtual void OnAttached(App app)
        {
        }
    }
}
