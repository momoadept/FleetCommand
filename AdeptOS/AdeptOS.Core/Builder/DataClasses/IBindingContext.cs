using System.Collections.Generic;

namespace IngameScript
{
    public partial class Program
    {
        #region mdk preserve
        public interface IBindingContext
        {
            #endregion
            IEnumerable<TModule> Any<TModule>();
            IEnumerable<TModule> RequireAny<TModule>(IModule caller);

            TModule One<TModule>(IModule caller);
            TModule RequireOne<TModule>(IModule caller);
        }
    }
}
