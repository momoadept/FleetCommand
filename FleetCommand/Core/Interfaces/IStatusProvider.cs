using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    partial class Program
    {
        public interface IStatusProvider
        {
            string StatusEntityId { get; }

            string GetStatus();
        }
    }
}
