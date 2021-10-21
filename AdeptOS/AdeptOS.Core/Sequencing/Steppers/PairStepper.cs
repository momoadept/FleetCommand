﻿using Sandbox.Game.EntityComponents;
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
        /// Combines two sequences. Executes first sequence steps in order, then second sequence steps in order, then completes.
        /// </summary>
        public class PairStepper : IStepper
        {
            private IStepper _a;
            private IStepper _b;
            private ILog _log;
            private IStepper _current;
            private int _stepped;

            public PairStepper(IStepper a, IStepper b, ILog log = null)
            {
                _a = a;
                _b = b;
                _log = log;
                _current = a;
            }

            public SequenceStep Next()
            {
                if (!_current.IsComplete())
                {
                    _stepped++;
                    return _current.Next();
                }

                if (_current == _a)
                {
                    _current = _b;
                    _stepped++;
                    return Next();
                }

                return null;
            }

            public bool IsComplete() => _current == _b && _b.IsComplete();

            public void Reset()
            {
                _a.Reset();
                _b.Reset();
                _current = _a;
                _stepped = 0;
            }

            public IStepper Clone() => new PairStepper(_a.Clone(), _b.Clone());

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">TWO \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).Append(Tracer.Steps(_stepped - 1, IsComplete())).AppendLine(_current == _a ? "(A)" : "(B)");
                s.Append(_a.Trace(depth + 1));
                s.AppendLine();
                s.Append(_b.Trace(depth + 1));

                return s.ToString();
            }

            public string Name { get; set; }
        }

    }
}