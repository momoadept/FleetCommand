namespace IngameScript
{
    partial class Program
    {
        public class PriorityConfiguration
        {
            public int ConditionCheckInterval(Priority priority) => (int)priority;

            public int JobCheckInterval(Priority priority) => (int)priority;
        }
    }
}
