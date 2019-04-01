using System.Collections.Generic;

namespace IngameScript
{
    public partial class Program
    {
        public interface IBindingContext
        {
            IEnumerable<TModule> Any<TModule>() where TModule : IModule;
            IEnumerable<TModule> RequireAny<TModule>(IModule caller) where TModule : IModule;

            TModule One<TModule>(IModule caller) where TModule : IModule;
            TModule RequireOne<TModule>(IModule caller) where TModule : IModule;
        }
    }
}
