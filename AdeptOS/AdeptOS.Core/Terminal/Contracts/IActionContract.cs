namespace IngameScript
{
    partial class Program
    {
        public interface IActionContract
        {
            string Name { get; }
            bool NoArgument { get; }
            IPromise<object> Do(object arg);
        }

        public interface IActionContract<TArgument, TResult> : IActionContract
            where TArgument : class, IStringifiable, new()
            where TResult : class, IStringifiable, new()
        {
            IPromise<TResult> Do(TArgument arg);
        }
    }
}
