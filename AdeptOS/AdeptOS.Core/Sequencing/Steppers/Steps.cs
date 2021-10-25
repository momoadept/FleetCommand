using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Components.BankingAndCurrency;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public static class Tracer
        {
            public static string Tab(int d)
            {
                var s = new StringBuilder();
                for (int i = 0; i < d; i++)
                {
                    s.Append("|   >");
                }

                return s.ToString();
            }

            public static string Steps(int current, bool terminated, int max = -1, bool error = false)
            {
                if (max < 0)
                    max = current;

                var s = new StringBuilder("[");

                //for (int i = 0; i < max; i++)
                //{
                //    s.Append(i == current - 1 ? ";" : ".");
                //}

                s.Append($"{current}/{max}]");

                if (error)
                    s.Append("E");
                else if (terminated)
                    s.Append("X");

                s.Append("]");

                return s.ToString();
            }
        }

        public class SequenceStep
        {
            public Func<IPromise<Void>> PromiseGenerator;
            public string StepTag;

            public static implicit operator SequenceStep(Func<IPromise<Void>> v) => new SequenceStep()
            {
                PromiseGenerator = v,
                StepTag = string.Empty,
            };

            public static implicit operator SequenceStep(Action v) => new SequenceStep()
            {
                PromiseGenerator = () =>
                {
                    v();
                    return Void.Promise();
                },
                StepTag = string.Empty,
            };
        }

        public interface IStepper
        {
            SequenceStep Next();

            bool IsComplete();

            void Reset();

            IStepper New();

            string Trace(int depth = 0, string prefix = "");

            string Name { get; set; }
        }
    }
}
