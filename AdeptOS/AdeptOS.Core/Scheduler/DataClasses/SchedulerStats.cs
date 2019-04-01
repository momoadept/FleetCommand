using System;

namespace IngameScript
{
    partial class Program
    {
        public class SchedulerStats
        {
            private int _totalSteps = 0;
            private int _totalAllowedSteps = 0;
            public int Ticks { get; private set; } = 0;
            private int _actions = 0;
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

            public void IncActions()
            {
                ++_actions;
            }

            public string Snapshot()
            {
                var elapsedSeconds = DateTime.Now.Subtract(_timeStart).TotalMilliseconds / 1000;
                var report =
                    $"{_totalSteps / _totalAllowedSteps * 100:F1}% Load\n{_actions} microtasks\nLast {elapsedSeconds:F2} seconds, {Ticks} ticks";

                _actions = _totalSteps = _totalAllowedSteps = Ticks = 0;
                _timeStart = DateTime.Now;

                return report;
            }
        }
    }
}
