namespace IngameScript
{
    partial class Program
    {
        public abstract class BaseHandler<TState, TContext>: IStateMachineControlObject<TState, TContext>
        {
            protected IStateMachineController<TState, TContext> _stateMachine;

            public void RegisterStateMachine(IStateMachineController<TState, TContext> stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public abstract void Enter();

            public abstract void Exit();
        }
    }
}
