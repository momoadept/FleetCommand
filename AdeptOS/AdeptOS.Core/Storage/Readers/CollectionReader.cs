using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class CollectionReader
        {
            public IEnumerable<string> Restore(string values)
            {
                var keyValues = new KeyValueReader().Parse(values);

                return keyValues
                    .OrderBy(it => int.Parse(it.Key))
                    .Select(it => it.Value);
            }
        }
    }
}
