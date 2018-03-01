using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    partial class Program
    {
        public interface IMyServiceProvider
        {
            T Get<T>() where T : class;

            void Use<T>(T service) where T : class;
        }
    }
}
