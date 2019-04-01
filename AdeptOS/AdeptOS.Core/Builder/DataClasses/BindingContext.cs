using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public class BindingContext: IBindingContext
        {
            private IEnumerable<IModule> _modules;

            public BindingContext(IEnumerable<IModule> modules)
            {
                _modules = modules;
            }

            public IEnumerable<TModule> Any<TModule>() where TModule : IModule
            {
                return Get<TModule>();
            }

            public IEnumerable<TModule> RequireAny<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if(!modules.Any())
                    throw new BindingException($"Module {caller.UniqueName} needs one or more implementations of {typeof(TModule).Name} but there aren't any in this package");

                return modules;
            }

            public TModule One<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if (modules.Count() > 1)
                    throw new BindingException($"Module {caller.UniqueName} needs one optional implementation of {typeof(TModule).Name} but there are {modules.Count()} in this package");

                return modules.FirstOrDefault();
            }

            public TModule RequireOne<TModule>(IModule caller) where TModule : IModule
            {
                var modules = Get<TModule>();

                if (!modules.Any())
                    throw new BindingException($"Module {caller.UniqueName} needs one implementation of {typeof(TModule).Name} but there aren't any in this package");

                if (modules.Count() > 1)
                    throw new BindingException($"Module {caller.UniqueName} needs one implementation of {typeof(TModule).Name} but there are {modules.Count()} in this package");

                return modules.First();
            }

            private IEnumerable<TModule> Get<TModule>()
            {
                return _modules.Where(it => it is TModule).Cast<TModule>();
            }
        }
    }
}
