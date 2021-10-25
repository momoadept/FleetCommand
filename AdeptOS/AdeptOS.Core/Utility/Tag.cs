using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Sandbox.ModAPI.Ingame;

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

            public void Restore(string value) => Unwrapped = value.StripCurlyBraces().StripSquareBraces();

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
            static System.Text.RegularExpressions.Regex _tagMatcher = new System.Text.RegularExpressions.Regex(" /\\[([^]]+)\\]/");

            public static HashSet<Tag> FromName(string name)
            {
                var matches = _tagMatcher.Matches(name);
                var result = new List<System.Text.RegularExpressions.Match>();
                for (int i = 0; i < matches.Count; i++)
                {
                    result.Add(matches[i]);
                }

                return new HashSet<Tag>(result.Select(it => new Tag(it.Value)));
            }

            public static List<TBlock> FindGroupByTag<TBlock>(Tag tag, IMyGridTerminalSystem grid, List<IMyTerminalBlock> bbuffer = null,
                List<IMyBlockGroup> gbuffer = null)
                where TBlock : class
            {
                bbuffer = bbuffer ?? new List<IMyTerminalBlock>();
                gbuffer = gbuffer ?? new List<IMyBlockGroup>();

                var result = new List<TBlock>();

                grid.GetBlockGroups(gbuffer, group => group.Name.Contains(tag.Wrapped));
                foreach (var blockGroup in gbuffer)
                {
                    blockGroup.GetBlocksOfType<TBlock>(bbuffer);
                    result.AddRange(bbuffer.Cast<TBlock>());
                }

                return result;
            }

            public static List<TBlock> FindBlockByTag<TBlock>(Tag tag, IMyGridTerminalSystem grid, List<IMyTerminalBlock> bbuffer = null)
                where TBlock : class
            {
                bbuffer = bbuffer ?? new List<IMyTerminalBlock>();

                grid.SearchBlocksOfName(tag.Wrapped, bbuffer, x => x is TBlock);
                return bbuffer.Cast<TBlock>().ToList();
            }
        }
    }
}
