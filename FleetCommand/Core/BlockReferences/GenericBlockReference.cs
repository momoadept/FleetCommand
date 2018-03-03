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

        public TagBlockReference(IBlockLoader blocks, string tag)
        {
            Blocks = blocks;
            Tag = tag;
        }

        protected List<T> GetMyBlocks()
        {
            var blocks = Blocks.Blocks
                .Where(b => b is T && b.Name.Contains(App.BlockTag(Tag)))
                .Cast<T>();

            return blocks.ToList();
        }
    }
}
