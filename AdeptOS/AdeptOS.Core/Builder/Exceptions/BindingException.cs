using System;

namespace IngameScript
{
    partial class Program
    {
        public class BindingException : Exception
        {
            public BindingException(string message) : base(message)
            {
            }

            public BindingException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}
