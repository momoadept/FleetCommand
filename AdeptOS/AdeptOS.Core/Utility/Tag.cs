using Sandbox.Game.AI;
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

            public bool Configurable { get; private set; }
            System.Text.RegularExpressions.Regex _configMatcher;

            public Tag(string tag, bool configurable = false)
            {
                Configurable = configurable;
                Unwrapped = tag.StripSquareBraces().Trim();
                if (Configurable)
                    _configMatcher = new System.Text.RegularExpressions.Regex(_matchTagStartingWithPattern.Replace("$", Unwrapped));
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

            public string GetOptions(string name) => Configurable ? _configMatcher.Match(name).Value.StripSquareBraces().Replace(Unwrapped, "") : "";

            public override int GetHashCode() => Unwrapped.GetHashCode();

            public override bool Equals(object obj) => GetHashCode() == obj?.GetHashCode();

            static string Wrap(string tag) => tag.Trim().StartsWith("[") ? tag.Trim() : $"[{tag.Trim()}]";
            static System.Text.RegularExpressions.Regex _tagMatcher = new System.Text.RegularExpressions.Regex("\\[([^]]+)\\]");
            static string _matchTagStartingWithPattern = "\\[($\\]|$[^\\w\\]\\[][^\\[\\]]*)\\]?";

            public static HashSet<Tag> FromName(string name)
            {
                var matches = _tagMatcher.Matches(name);
                var result = new List<System.Text.RegularExpressions.Match>();
                for (int i = 0; i < matches.Count; i++)
                {
                    result.Add(matches[i]);
                }

                var processed = result.Select(x => x.Value)
                    .Select(x =>
                    {
                        var separator = x.FirstOrDefault(ch => !char.IsLetterOrDigit(ch) && !"[]_-+".Contains(ch));

                        if (separator == default(char))
                            return x;

                        var index = x.IndexOf(separator);
                        return x.Substring(0, index);
                    });

                return new HashSet<Tag>(processed.Select(it => new Tag(it)));
            }

            public bool NameMatches(string name) => Configurable ? _configMatcher.Match(name).Success : name.Contains(Wrapped);

            public static List<TBlock> FindAllByTag<TBlock>(Tag tag, IMyGridTerminalSystem grid,
                List<IMyTerminalBlock> bbuffer = null,
                List<IMyBlockGroup> gbuffer = null)
                where TBlock : class =>
                FindGroupByTag<TBlock>(tag, grid, bbuffer, gbuffer)
                    .Concat(FindBlockByTag<TBlock>(tag, grid, bbuffer))
                    .Distinct()
                    .ToList();

            public static List<TBlock> FindGroupByTag<TBlock>(Tag tag, IMyGridTerminalSystem grid, List<IMyTerminalBlock> bbuffer = null,
                List<IMyBlockGroup> gbuffer = null)
                where TBlock : class
            {
                bbuffer = bbuffer ?? new List<IMyTerminalBlock>();
                gbuffer = gbuffer ?? new List<IMyBlockGroup>();

                var result = new List<TBlock>();

                grid.GetBlockGroups(gbuffer, group => tag.NameMatches(group.Name));
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

                grid.GetBlocksOfType<TBlock>(bbuffer, x => tag.NameMatches(x.CustomName));
                return bbuffer.Cast<TBlock>().ToList();
            }
        }
    }
}
