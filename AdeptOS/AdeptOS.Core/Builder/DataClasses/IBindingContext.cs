using System.Collections.Generic;

namespace IngameScript
{
    public partial class Program
    {
        public interface IBindingContext
        {
            IEnumerable<TModule> Any<TModule>();
            IEnumerable<TModule> RequireAny<TModule>(IModule caller);

            TModule One<TModule>(IModule caller);
            TModule RequireOne<TModule>(IModule caller);
        }
    }
}
