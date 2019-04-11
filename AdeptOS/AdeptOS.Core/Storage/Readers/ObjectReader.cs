using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class ObjectReader<TSource>
        {
            List<Property<TSource>> _mapping;

            public ObjectReader(List<Property<TSource>> mapping)
            {
                _mapping = mapping;
            }

            public TSource Restore(TSource target, string values)
            {
                var reader = new KeyValueReader();
                var keyValues = reader.Parse(values);

                foreach (var property in _mapping)
                    property.Setter(target, keyValues[property.Key]);

                return target;
            }
        }
    }
}
