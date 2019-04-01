using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Tests.Storage
{
    class TestComplexObjectReadWrite
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("Test complex object read write");
            Console.WriteLine();

            var data = new TestModule(
                new DataObject(1, 2, "this is the string oh yes", new List<int>() { }),
                new DataObject(3, 4, "this is another one", new List<int>() {42}),
                new List<DataObject>()
                {
                    new DataObject(456, 3457, "i'm inside a collection!", new List<int>() {666, 666, 667}),
                    new DataObject(1337, 1337, "l33t", new List<int>() {1337}),
                }
            );
            data.UniqueName = "Name1";

            Console.WriteLine("initial object state");
            Console.WriteLine(data);
            Console.WriteLine();

            var storage = data.Stringify();

            Console.WriteLine("object string representation");
            Console.WriteLine(storage);
            Console.WriteLine();

            Console.WriteLine("restoring object from string");
            Console.WriteLine();

            var restored = new TestModule();
            restored.UniqueName = "Name1";
            restored.Restore(storage);

            Console.WriteLine("resulting object state");
            Console.WriteLine(restored);
            Console.WriteLine();
        }
    }
}
