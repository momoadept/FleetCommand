using System;

namespace IngameScript
{
    partial class Program
    {
        public class ActionContract<TArgument, TResult> : IActionContract<TArgument, TResult>
            where TArgument : class, IStringifiable, new() 
            where TResult : class, IStringifiable, new()
        {
            public string Name { get; }

            public bool NoArgument { get; }

            private Func<TArgument, IPromise<TResult>> _invoke;

            public ActionContract(string name, Func<TArgument, IPromise<TResult>> invoke, bool noArgument = true)
            {
                _invoke = invoke;
                Name = name;
                NoArgument = noArgument;
            }

            public IPromise<TResult> Do(TArgument arg) => (IPromise<TResult>)Do((object)arg);

            public IPromise<object> Do(object arg)
            {
                if (NoArgument)
                    return _invoke(null);

                var argument = arg as TArgument;
                if (argument == null)
                {
                    if (!(arg is string))
                    {
                        throw new Exception($"Cannot invoke {Name} - bad argument {arg}");
                    }

                    argument = new TArgument();
                    argument.Restore((string) arg);
                }

                var result = _invoke(argument);
                return result;
            }
        }
    }
}
