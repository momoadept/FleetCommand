using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Tests.Storage
{
    class TestObjectReadWrite
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("Test object read write");
            Console.WriteLine();

            var data = new DataObject(1, 2, "some_text", new List<int>() {1337, 42, 69});

            Console.WriteLine("initial object state");
            Console.WriteLine(data);
            Console.WriteLine();

            var storage = data.Stringify();

            Console.WriteLine("object string representation");
            Console.WriteLine(storage);
            Console.WriteLine();

            Console.WriteLine("restoring object from string");
            Console.WriteLine();

            var restored = new DataObject();
            restored.Restore(storage);

            Console.WriteLine("resulting object state");
            Console.WriteLine(restored);
            Console.WriteLine();
        }
    }
}
