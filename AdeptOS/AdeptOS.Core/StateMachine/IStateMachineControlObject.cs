namespace IngameScript
{
    partial class Program
    {
        public interface IStateMachineControlObject<TState, TSyncContext>
        {
            void RegisterStateMachine(IStateMachineController<TState, TSyncContext> stateMachine);
            void Enter();
            void Exit();
        }
    }
}
