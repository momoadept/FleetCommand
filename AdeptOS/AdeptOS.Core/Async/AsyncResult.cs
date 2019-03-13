using System;
using System.Collections.Generic;
using System.Text;
using AdeptOS.Core.Utility;

namespace AdeptOS.Core.Async
{
    public class AsyncResult<TResult>: BaseTrackable
    {
        public AsyncState State { get; set; }

        public AsyncResult<TResult> Then(Action<TResult> handler)
        {
            return this;
        }

        public AsyncResult<TResult> Catch(Action<Exception> handler)
        {
            return this;
        }

        public AsyncResult<TResult> Finally(Action handler)
        {
            return this;
        }
    }
}
