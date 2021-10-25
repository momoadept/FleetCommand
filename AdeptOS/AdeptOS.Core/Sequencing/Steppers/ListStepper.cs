using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Presents a sequence from a list of operations. Sequence completes when the last step in the list is done.
        /// </summary>
        public class ListStepper : IStepper
        {
            protected IList<SequenceStep> Steps;
            protected int next;

            public ListStepper(IList<SequenceStep> steps)
            {
                Steps = steps;
            }

            public ListStepper(params SequenceStep[] steps)
            {
                Steps = steps;
            }


            public SequenceStep Next()
            {
                next++;

                if (next < Steps.Count)
                {
                    return Steps[next];
                }

                return null;
            }

            public bool IsComplete() => next >= Steps.Count;

            public void Reset() => next = 0;
            public IStepper New() => new ListStepper(Steps.Select(x => x).ToList());


            /*
             * >LST A
             * [....*]
             * |    >LST B
             * |    [....*]
             * |    |    >LST C
             * |    |    [..*X]
             * |    >LST B2
             * |    [....*X]
             */
            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">LST \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).AppendLine(Tracer.Steps(next - 1, IsComplete(), Steps.Count));

                return s.ToString();
            }

            public string Name { get; set; }
        }

    }
}