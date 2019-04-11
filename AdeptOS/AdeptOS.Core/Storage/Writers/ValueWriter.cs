using System.Collections;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        public class ValueWriter
        {
            public string StringifyValue(object value)
            {
                var stringifiable = value as IStringifiable;
                if (stringifiable != null)
                    return stringifiable.Stringify();

                var enumerable = value as IEnumerable;
                if (enumerable != null && !(enumerable is string))
                    return StringifyCollection(enumerable);

                return value.ToString();
            }

            public string StringifyCollection(IEnumerable enumerable)
            {
                var result = new StringBuilder();
                var index = 0;
                foreach (var value in enumerable)
                    result.Append($"{index++}:{{{StringifyValue(value)}}},");

                if (result.Length > 0)
                    result.Remove(result.Length - 1, 1);

                return result.ToString();
            }
        }
    }
}
