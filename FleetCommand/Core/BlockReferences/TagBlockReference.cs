using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.BlockLoader;
using IngameScript.Core.FakeAsync;
using IngameScript.Core.Logging;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public class TagBlockReference<T> where T: IMyTerminalBlock
    {
        protected IBlockLoader Blocks;
        protected string Tag;
        protected ILog Log;
        protected IAsyncTask ReadyWaiter;
        public bool Ready { get; private set; } = false;

        public string FullTag => App.BlockTag(Tag);

        public TagBlockReference(string tag)
        {
            Blocks = App.ServiceProvider.Get<IBlockLoader>();
            Tag = tag;
        }

        public List<T> GetMyBlocks()
        {
            if (WaitingIfNotReady())
            {
                return new List<T>();
            }

            Ready = true;

            var blocks = Blocks.Blocks
                .Where(b => b is T && b.CustomName != null && b.CustomName.Contains(FullTag))
                .Cast<T>();

            return blocks.ToList();
        }

        private bool WaitingIfNotReady()
        {
            if (ReadyWaiter != null && !ReadyWaiter.IsCompleted)
            {
                return true;
            }
            if (Blocks == null)
            {
                ReadyWaiter = App.ServiceProvider.Get<Async>()
                    .When(() => App.ServiceProvider.Get<IBlockLoader>() != null && App.ServiceProvider.Get<IBlockLoader>().Blocks.Any())
                    .Then(e =>
                    {
                        Blocks = App.ServiceProvider.Get<IBlockLoader>();
                        Ready = true;
                    })
                    .Pick();
                return true;
            }

            return false;
        }
    }
}
