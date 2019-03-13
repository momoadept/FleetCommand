using System;
using System.Collections.Generic;
using System.Text;

namespace AdeptOS.Core.Async
{
    public interface IAsyncQueue
    {
        AsyncResult<TResult> Do<TResult>(Func<TResult> task, AsyncPriority priority);
    }
}
