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
        protected IBlockLoader BlockLoader { get; }
        protected ILog Log { get; }
        protected string Tag { get; }
        protected string AppTag { get; }
        public List<BlockAccessor<T>> Accessors { get; } = new List<BlockAccessor<T>>();

        public TagBlockReference(IBlockLoader blockLoader, ILog log, string tag, string appTag)
        {
            BlockLoader = blockLoader;
            Log = log;
            Tag = tag;
            AppTag = appTag;
            BlockLoader.Updated += loader => RefreshAccessors();
            RefreshAccessors();
        }

        public void RefreshAccessors()
        {
            var searchString = $"[{AppTag} {Tag}";

            Accessors.Clear();
            Accessors.AddRange(BlockLoader.Blocks
                .Where(block => block.CustomName.StartsWith(searchString) && block is T)
                .Cast<T>()
                .Select(block => new BlockAccessor<T>(block, GetArguments(block.CustomName))));
        }

        public int ForEach(Action<T> action, Func<BlockAccessor<T>, bool> filter = null)
        {
            filter = filter ?? (x => true);
            var filtered = Accessors.Where(filter);
            foreach (var blockAccessor in Accessors.Where(filter))
            {
                action(blockAccessor.Block);
            }

            return filtered.Count();
        }

        protected List<string> GetArguments(string name)
        {
            try
            {
                var start = name.IndexOf("[");
                var end = name.IndexOf("]");

                return name
                    .Substring(start + 1, end - start - 1)
                    .Replace(AppTag, "")
                    .Replace(Tag, "")
                    .Split(' ')
                    .ToList();
            }
            catch (Exception e)
            {
                Log.Error($"Cannot parse the block name: {name}");
                throw;
            }
        }
    }
}
