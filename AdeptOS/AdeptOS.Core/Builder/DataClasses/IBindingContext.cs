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
            IEnumerable<TModule> RequireAny<TModule>();

            TModule One<TModule>();
            TModule RequireOne<TModule>();
        }
    }
}
