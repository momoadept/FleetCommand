using System;

namespace IngameScript
{
    partial class Program
    {
        public class Property<TSource>
        {
            public string Key;

            public Func<TSource, object> Getter;

            public Action<TSource, string> Setter;

            public Property(string key, Func<TSource, object> getter, Action<TSource, string> setter)
            {
                Key = key;
                Getter = getter;
                Setter = setter;
            }
        }
    }
}
