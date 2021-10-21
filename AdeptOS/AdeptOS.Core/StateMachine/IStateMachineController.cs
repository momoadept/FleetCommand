namespace IngameScript
{
    partial class Program
    {
        public interface IStateMachineController<TState, TSynchContext>
        {
            void SwitchState(TState next);
            TSynchContext Context { get; }

            TState Current { get; }
        }
    }
}
