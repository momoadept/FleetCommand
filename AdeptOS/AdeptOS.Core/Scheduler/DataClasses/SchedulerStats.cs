using System;

namespace IngameScript
{
    partial class Program
    {
        public class SchedulerStats
        {
            int _totalSteps;
            int _totalAllowedSteps;
            public int Ticks { get; private set; }
            int _actions;
            DateTime _timeStart = DateTime.Now;
            IGameContext _context;
            float _maxLoad;
            float _maxLoadSteps;

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
                _maxLoadSteps = Math.Max(_totalSteps, _maxLoadSteps);
                var report =
                    $"{load:F1}% ({_maxLoad:F1}% max)\n{_totalSteps} / {_totalAllowedSteps} ({_maxLoadSteps}) Load\n{_actions} microtasks\nLast {elapsedSeconds:F2} seconds, {Ticks} ticks";

                _actions = _totalSteps = _totalAllowedSteps = Ticks = 0;
                _timeStart = DateTime.Now;

                return report;
            }
        }
    }
}
