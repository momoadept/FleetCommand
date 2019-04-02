namespace IngameScript
{
    partial class Program
    {
        public enum Priority
        {
            /// <summary>
            /// Dependant on real-time execution
            /// </summary>
            Critical = 10,

            /// <summary>
            /// Needs to be done but not time-sensitive
            /// </summary>
            Routine = 1000,

            /// <summary>
            /// Not crucial to ship operation
            /// </summary>
            Unimportant = 30000
        }
    }
}
