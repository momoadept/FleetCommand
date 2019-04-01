using System;

namespace IngameScript
{
    partial class Program
    {
        public interface IResolvablePromise<TResult>: IPromise<TResult>
        {
            void Resolve(TResult result);

            void Fail(Exception error);
        }
    }
}
