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
        /// Combines two sequences, Main and Interrupter. Executes Main steps and completes when Main is complete.
        /// After each Main step executes one Interrupter step, if it's not complete, skips Interrupter otherwise. 
        /// </summary>
        public class InterruptingStepper : IStepper
        {
            IStepper _main;
            IStepper _interruptor;
            bool _interruptNow = true;
            int _stepped;

            public InterruptingStepper(IStepper main, IStepper interruptor)
            {
                _main = main;
                _interruptor = interruptor;
            }

            public SequenceStep Next()
            {
                if (_main.IsComplete() && _interruptNow)
                    return null;

                _interruptNow = !_interruptNow;

                _stepped++;
                return _interruptNow ? (_interruptor.Next() ?? _main.Next()) : _main.Next();
            }

            public bool IsComplete() => _main.IsComplete() && !_interruptNow;

            public void Reset()
            {
                _main.Reset();
                _interruptor.Reset();
                _stepped = 0;
                _interruptNow = true;
            }

            public IStepper New() => new InterruptingStepper(_main.New(), _interruptor.New());

            public string Trace(int depth = 0, string prefix = "")
            {
                var name = $">INT \"{Name}\"\n";

                var s = new StringBuilder();
                var tab = Tracer.Tab(depth);

                s.Append(tab).Append(name);
                s.Append(tab).Append(Tracer.Steps(_stepped - 1, IsComplete())).AppendLine(_interruptNow ? "(Main)" : "(Inter)");
                s.Append(_main.Trace(depth + 1));
                s.AppendLine();
                s.Append(_interruptor.Trace(depth + 1));

                return s.ToString();
            }

            public string Name { get; set; }
        }

    }
}