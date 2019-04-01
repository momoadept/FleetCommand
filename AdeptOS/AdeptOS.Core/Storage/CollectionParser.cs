using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        public static class CollectionParser
        {
            public static IEnumerable<string> Parse(string values)
            {
                return new CollectionReader().Restore(values);
            }

            public static string Stringify(IEnumerable source)
            {
                var writer = new ValueWriter();
                var values = new List<string>();
                foreach (var value in source)
                {
                    values.Add(writer.StringifyValue(value));
                }

                var result = new StringBuilder();
                result.Append("{");

                result.Append(string.Join(",", values));

                result.Append("}");

                return result.ToString();
            }
        }
    }
}
