using System;

namespace IngameScript
{
    partial class Program
    {
        public abstract class OperationContract : IOperationNode
        {
            public string Name { get; protected set; }
            public string Alias { get; protected set; }

        }

        public class OperationContract<TArgument, TResult> : OperationContract
            where TArgument: IStringifiable, new ()
            where TResult: IStringifiable, new ()
        {
            public Func<TArgument, IPromise<TResult>> Invoke { get; }

            public OperationContract(string name, Func<TArgument, IPromise<TResult>> invoke, string alias = null)
            {
                Invoke = invoke;
                Name = name;
                Alias = alias ?? name;
            }
        }
    }
}
