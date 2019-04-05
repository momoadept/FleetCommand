using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        public interface ITimedQueue<TValue>
        {
            void Push(DateTime time, TValue value);

            bool AnyLessThan(DateTime time);

            IEnumerable<TValue> PopLessThan(DateTime time);

            TValue PopNext();

            void Clear();

            int Count { get; }
        }
    }
}
