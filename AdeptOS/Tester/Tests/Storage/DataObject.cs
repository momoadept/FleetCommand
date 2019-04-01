using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript;
using Property = IngameScript.Program.Property<Tester.Tests.Storage.DataObject>;
using CollectionParser = IngameScript.Program.CollectionParser;

namespace Tester.Tests.Storage
{
    public class DataObject: IngameScript.Program.BaseDataObject<DataObject>
    {
        public int x;
        public int y;
        public string text;
        public IEnumerable<int> members;

        private static List<Property> mapping = new List<Property>()
        {
            new Property("x", it => it.x, (it, x) => it.x = int.Parse(x)),
            new Property("y", it => it.y, (it, y) => it.y = int.Parse(y)),
            new Property("text", it => it.text, (it, t) => it.text = t),
            new Property("members", it => it.members, (it, members) => 
                it.members = CollectionParser.Parse(members).Select(int.Parse))
        };

        public DataObject(int x, int y, string text, IEnumerable<int> members) : base(mapping)
        {
            this.x = x;
            this.y = y;
            this.text = text;
            this.members = members;
        }

        public DataObject() : base(mapping)
        {
        }

        public override string ToString()
        {
            return $@"
x = {x}
y = {y}
text = {text}
members = {string.Join(", ", members)}
";
        }
    }
}
