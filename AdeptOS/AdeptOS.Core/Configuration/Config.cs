namespace IngameScript
{
    partial class Program
    {
        public class Config
        {
            public PriorityConfiguration Priorities = new PriorityConfiguration();
            public SchedulerPerformanceConfiguration SchedulerPerformance = new SchedulerPerformanceConfiguration();

            public bool IsDev = true;
        }
    }
}
