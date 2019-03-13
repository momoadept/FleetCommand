using System;
using System.Collections.Generic;
using System.Text;

namespace AdeptOS.Core.Utility
{
    public abstract class BaseTrackable
    {
        public long Id { get; private set; }

        private static long _nextId = 0;

        private static object globalLock = new object();

        protected BaseTrackable()
        {
            lock (globalLock)
            {
                Id = ++_nextId;
            }
        }
    }
}
