using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
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

        static System.Text.RegularExpressions.Regex _nameMatcher = new System.Text.RegularExpressions.Regex(" /(\\([^)]+\\))/;");
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

        public static Dictionary<string, Program.IActionContract> Add<TArgument, TResult>(
            this Dictionary<string, Program.IActionContract> actions,
            string name,
            Func<TArgument, Program.IPromise<TResult>> impl,
            bool noArgument = false)
            where TResult : class, Program.IStringifiable, new()
            where TArgument : class, Program.IStringifiable, new()
        {
            var contract = new Program.ActionContract<TArgument, TResult>(name, impl, noArgument);
            actions.Add(name, contract);
            return actions;
        }
    }
}
