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
        /// Presents a repeating sequence. Executes underlying sequence steps, then resets it if condition is true.
        /// Completes when condition is false, either at the end of current sequence or straight away.
        /// </summary>
        public class CycleStepper : IStepper
        {
            private IStepper _body;
            private Func<bool> _while;
            private bool _checkEveryStep;
            private bool _first = true;
            private int _stepped = 0;

            public CycleStepper(IStepper body, Func<bool> @while, bool checkEveryStep = false)
            {
                _body = body;
                _while = @while;
                _checkEveryStep = checkEveryStep;
            }

            public SequenceStep Next()
            {
                if (IsComplete())
                    return null;

                if (_body.IsComplete())
                {
                    if (_while())
                    {
                        _body.Reset();
                        _stepped++;
                        return _body.Next();
                    }

                    return null;
                }

                _first = false;
                _stepped++;
                return _body.Next();
            }

            public bool IsComplete() => ((_first || _checkEveryStep) && !_while()) || (_body.IsComplete() && !_while());

            public void Reset()
            {
                _first = true;
                _body.Reset();
                _stepped = 0;
            }

            public IStepper Clone() => new CycleStepper(_body.Clone(), _while, _checkEveryStep);

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">CYC \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).Append(Tracer.Steps(_stepped - 1, IsComplete())).AppendLine(_while() ? " (Y)" : " (N)");
                s.Append(_body.Trace(depth + 1));

                return s.ToString();
            }

            public string Name { get; set; }
        }
    }
}