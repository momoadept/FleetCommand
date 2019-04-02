using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public abstract class OperationContract : IOperationNode
        {
            public string Name { get; protected set; }
            public string Alias { get; protected set; }
            public string FullPath => $"{Parent?.FullPath ?? ""}.{Name}";
            public OperationGroup Parent { get; set; }
           
            public OperationContract Find(string fullPath)
            {
                if (fullPath.Equals(FullPath))
                    return this;

                return null;
            }

            public Dictionary<string, OperationContract> OperationsByPath(string prefix)
            {
                return new Dictionary<string, OperationContract>()
                {
                    {$"{prefix}.{Name}", this}
                };
            }

            public abstract IPromise<object> Invoke(object argument = null);
            public bool NoArgument { get; protected set; }
        }

        public class OperationContract<TArgument, TResult> : OperationContract
            where TArgument: class, IStringifiable, new ()
            where TResult: class, IStringifiable, new ()
        {
            private Func<TArgument, IPromise<TResult>> _invoke;

            public OperationContract(string name, Func<TArgument, IPromise<TResult>> invoke, bool noArgument = true, string alias = null)
            {
                _invoke = invoke;
                Name = name;
                Alias = alias ?? name;
                NoArgument = noArgument;
            }

            public override IPromise<object> Invoke(object argument = null)
            {
                if (NoArgument)
                    return _invoke(null);

                var arg = argument as TArgument;
                if (arg == null)
                {
                    if (!(argument is string))
                    {
                        throw new Exception($"Cannot invoke {Name} - bad argument {argument}");
                    }

                    arg = new TArgument();
                    arg.Restore(argument as string);
                }

                var result = _invoke(arg);
                return result;
            }

            public IPromise<TResult> Invoke(TArgument argument = null)
            {
                return (IPromise<TResult>) Invoke((object) argument);
            }
        }
    }
}
