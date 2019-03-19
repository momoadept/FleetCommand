using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class SchedulerStats
        {
            private int _totalSteps = 0;
            private int _totalAllowedSteps = 0;
            public int Ticks { get; private set; } = 0;
            private DateTime _timeStart = DateTime.Now;
            private IGameContext _context;

            public SchedulerStats(IGameContext context)
            {
                _context = context;
            }

            public void Tick()
            {
                ++Ticks;
                _totalSteps += _context.CurrentSteps;
                _totalAllowedSteps += _context.MaxSteps;
            }

            public string Snapshot()
            {
                var elapsedSeconds = DateTime.Now.Subtract(_timeStart).TotalMilliseconds / 1000;
                var report =
                    $"{_totalSteps / _totalAllowedSteps * 100:F1}% Load ({elapsedSeconds:F2} seconds, {Ticks} ticks)";

                _totalSteps = _totalAllowedSteps = Ticks = 0;
                _timeStart = DateTime.Now;

                return report;
            }
        }
    }
}
