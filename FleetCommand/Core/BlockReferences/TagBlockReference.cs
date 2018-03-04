using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.BlockLoader;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences
{
    public class TagBlockReference<T> where T: IMyTerminalBlock
    {
        protected IBlockLoader Blocks;
        protected string Tag;

        public string FullTag => App.BlockTag(Tag);

        public TagBlockReference(string tag)
        {
            Blocks = App.ServiceProvider.Get<IBlockLoader>();
            Tag = tag;
        }

        public List<T> GetMyBlocks()
        {
            var blocks = Blocks.Blocks
                .Where(b => b is T && b.CustomName != null && b.CustomName.Contains(FullTag))
                .Cast<T>();

            return blocks.ToList();
        }
    }
}
