using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.Delegates;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.FakeAsync
{
    public class Waiter: IAsyncTask
    {
        public int Created { get; }
        public int Delay { get; private set; }
        public bool IsCompleted { get; private set; }

        public event Event.Handler<IAsyncTask> Completed;

        protected Func<bool> Condition { get; }
        public Waiter(Func<bool> condition, int delay = 0)
        {
            Delay = delay;
            Created = App.Time.Now;
            Condition = condition;
        }

        public void Tick()
        {
            if (Condition())
            {
                IsCompleted = true;
                Completed?.Invoke(this);
            }
            else
            {
                Delay++;
            }
        }

    }
}
