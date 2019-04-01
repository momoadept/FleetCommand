using System;

namespace IngameScript
{
    partial class Program
    {
        public class ConditionTimeoutException : Exception
        {
            public ConditionTimeoutException()
                : base("Condition check timed out")
            {
            }

            public ConditionTimeoutException(string message) : base(message)
            {
            }
        }
    }
}
