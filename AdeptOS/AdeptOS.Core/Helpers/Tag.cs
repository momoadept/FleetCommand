using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IngameScript
{
    partial class Program
    {
        public static class Tag
        {
            public static string Wrap(string tag) => tag.StartsWith("[") ? tag : $"[{tag}]";
            public static bool TaggedWith(string name, string tag) => name.Contains(Wrap(tag));

            private static Regex _tagMatcher = new Regex(" /\\[([^]]+)\\]/;");

            public static HashSet<string> Tags(string name)
            {
                var matches = _tagMatcher.Matches(name).Cast<Match>().Select(it => it.Value);
                return new HashSet<string>(matches);
            }
        }
    }
}
