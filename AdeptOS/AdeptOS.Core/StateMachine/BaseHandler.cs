namespace IngameScript
{
    partial class Program
    {
        public abstract class BaseHandler<TState, TContext>: IStateMachineControlObject<TState, TContext>
        {
            protected IStateMachineController<TState, TContext> StateMachine;
            protected TContext Context => StateMachine.Context;

            public void RegisterStateMachine(IStateMachineController<TState, TContext> stateMachine) => StateMachine = stateMachine;

            public abstract void Enter();

            public abstract void Exit();
        }
    }
}
