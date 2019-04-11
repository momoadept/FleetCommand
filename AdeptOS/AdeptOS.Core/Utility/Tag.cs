using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IngameScript
{
    partial class Program
    {
        public class Tag: IStringifiable
        {
            public string Unwrapped { get; private set; }
            public string Wrapped => Wrap(Unwrapped);

            public Tag(string tag)
            {
                Unwrapped = tag.StripSquareBraces().Trim();
            }

            public string Stringify() => Unwrapped;

            public void Restore(string value)
            {
                Unwrapped = value.StripCurlyBraces().StripSquareBraces();
            }

            public bool InName(string name) => name.Contains(Wrapped);

            public string AddToName(string name)
            {
                if (InName(name))
                    return name;

                return $"{Wrapped}{name}";
            }

            public override int GetHashCode() => Unwrapped.GetHashCode();

            public override bool Equals(object obj) => GetHashCode() == obj?.GetHashCode();

            static string Wrap(string tag) => tag.Trim().StartsWith("[") ? tag.Trim() : $"[{tag.Trim()}]";
            static Regex _tagMatcher = new Regex(" /\\[([^]]+)\\]/;");

            public static HashSet<Tag> FromName(string name)
            {
                var matches = _tagMatcher.Matches(name).Cast<Match>().Select(it => it.Value);
                return new HashSet<Tag>(matches.Select(it => new Tag(it)));
            }
        }
    }
}
