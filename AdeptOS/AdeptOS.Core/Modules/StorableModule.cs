using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public abstract class StorableModule<TType>: IModule, IStringifiable where TType: class
        {
            public abstract string UniqueName { get; }
            public abstract string Alias { get; }

            private List<Property<TType>> _mapping;
            private ObjectParser<TType> _parser;

            protected StorableModule(List<Property<TType>> mapping)
            {
                _mapping = mapping;
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

            public abstract void Bind(IBindingContext context);

            public abstract void Run();

            public virtual void OnSaving()
            {

            }
        }
    }
}
