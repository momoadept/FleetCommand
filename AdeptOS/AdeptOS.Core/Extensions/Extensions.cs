using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using System.Text.RegularExpressions;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    static class Extensions
    {
        public static string StripSquareBraces(this string src) => src.Replace("[", "").Replace("]", "");
        public static string StripCurlyBraces(this string src) => src.Replace("{", "").Replace("}", "");

        public static Program.Primitive<string> AsPrimitive(this string src) => new Program.Primitive<string>(src);
        public static Program.Primitive<bool> AsPrimitive(this bool src) => new Program.Primitive<bool>(src);
        public static Program.Primitive<int> AsPrimitive(this int src) => new Program.Primitive<int>(src);
        public static Program.Primitive<double> AsPrimitive(this double src) => new Program.Primitive<double>(src);

        static Regex _nameMatcher = new Regex(" /(\\([^)]+\\))/;");
        public static void UpdateName(this IMyProgrammableBlock target, string name)
        {
            var builder = new StringBuilder(target.CustomName);

            if (target.CustomName.Contains($"({name})"))
                return;

            var existingName = _nameMatcher.Match(target.CustomName);
            builder.Replace(existingName.Value, "");
            builder.Append($"({name})");
            target.CustomName = builder.ToString();
        }
    }
}
