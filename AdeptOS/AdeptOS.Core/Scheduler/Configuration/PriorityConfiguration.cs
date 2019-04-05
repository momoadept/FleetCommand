namespace IngameScript
{
    partial class Program
    {
        public class PriorityConfiguration
        {
            public int ConditionCheckInterval(Priority priority)
            {
                return (int) priority;
            }

            public int JobCheckInterval(Priority priority)
            {
                return (int) priority;
            }
        }
    }
}
