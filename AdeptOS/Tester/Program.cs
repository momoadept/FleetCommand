using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Tester.Tests.Storage;
using VRage.Game.ModAPI.Ingame;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            TestObjectReadWrite.Run();
            TestComplexObjectReadWrite.Run();

            Console.ReadLine();
        }
    }
}
