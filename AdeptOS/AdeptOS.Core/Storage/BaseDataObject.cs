using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public abstract class BaseDataObject<TType>: IStringifiable where TType: class, new ()
        {
            ObjectParser<TType> _parser;

            protected BaseDataObject(List<Property<TType>> mapping)
            {
                _parser = new ObjectParser<TType>(mapping);
            }

            public string Stringify()
            {
                return _parser.Stringify(this as TType);
            }

            public void Restore(string value)
            {
                _parser.Parse(this as TType, value);
            }

            public static TType FromString(string value)
            {
                var obj = new TType() as BaseDataObject<TType>;
                obj.Restore(value);
                return obj as TType;
            }
        }
    }
}
