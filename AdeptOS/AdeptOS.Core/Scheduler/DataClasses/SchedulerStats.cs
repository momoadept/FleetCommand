using System;

namespace IngameScript
{
    partial class Program
    {
        public class SchedulerStats
        {
            int _totalSteps = 0;
            int _totalAllowedSteps = 0;
            public int Ticks { get; private set; } = 0;
            int _actions = 0;
            DateTime _timeStart = DateTime.Now;
            IGameContext _context;
            private float _maxLoad = 0;

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

            public void IncActions() => ++_actions;

            public string Snapshot()
            {
                var elapsedSeconds = DateTime.Now.Subtract(_timeStart).TotalMilliseconds / 1000;
                var load = _totalSteps / _totalAllowedSteps * 100;
                _maxLoad = Math.Max(load, _maxLoad);
                var report =
                    $"{load:F1}% ({_maxLoad:F1}% max) Load\n{_actions} microtasks\nLast {elapsedSeconds:F2} seconds, {Ticks} ticks";

                _actions = _totalSteps = _totalAllowedSteps = Ticks = 0;
                _timeStart = DateTime.Now;

                return report;
            }
        }
    }
}
