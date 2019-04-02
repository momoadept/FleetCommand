using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Tests.Storage
{
    public class TestModule: IngameScript.Program.BaseDataObject<TestModule>, IngameScript.Program.IModule
    {
        public DataObject object1;
        public DataObject object2;
        public IEnumerable<DataObject> objectCollection;

        public string Alias { get; }

        public void Bind(IngameScript.Program.IBindingContext context)
        {
        }

        public void Run()
        {
        }

        public void OnSaving()
        {
        }

        public string UniqueName { get; set; }

        private static List<IngameScript.Program.Property<TestModule>> mapping = new List<IngameScript.Program.Property<TestModule>>()
        {
            new IngameScript.Program.Property<TestModule>("object1", it => it.object1, (it, obj1) => it.object1 = DataObject.FromString(obj1)),
            new IngameScript.Program.Property<TestModule>("object2", it => it.object2, (it, obj2) => it.object2 = DataObject.FromString(obj2)),
            new IngameScript.Program.Property<TestModule>("objectCollection", 
                it => it.objectCollection, 
                (it, coll) => it.objectCollection = IngameScript.Program.CollectionParser.Parse(coll).Select(obj => DataObject.FromString(obj)))
        };

        public TestModule() : base(mapping)
        {
        }

        public TestModule(DataObject object1, DataObject object2, IEnumerable<DataObject> objectCollection) : base(mapping)
        {
            this.object1 = object1;
            this.object2 = object2;
            this.objectCollection = objectCollection;
        }

        public override string ToString()
        {
            return $@"
{UniqueName} BEGIN
(obj1)
{object1}

(obj2)
{object2}

(objCollection)
{string.Join("\n", objectCollection)}
{UniqueName} END
";
        }
    }
}
