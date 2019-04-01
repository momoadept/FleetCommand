using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        public class ObjectParser<TSource>
        {
            private List<Property<TSource>> _mapping;

            public ObjectParser(List<Property<TSource>> mapping)
            {
                _mapping = mapping;
            }

            public string Stringify(TSource source)
            {
                var writer = new ValueWriter();
                var values = _mapping.Select(mapping => $"{mapping.Key}:{{{writer.StringifyValue(mapping.Getter(source))}}}");

                var result = new StringBuilder();
                result.Append("{");

                result.Append(string.Join(",", values));

                result.Append("}");

                return result.ToString();
            }

            public void Parse(TSource target, string values)
            {
                var reader = new ObjectReader<TSource>(_mapping);
                reader.Restore(target, values);
            }
        }
    }
}
