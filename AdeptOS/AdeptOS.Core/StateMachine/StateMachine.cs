using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class StateMachine<THandler, TState, TActions, TSynchContext>: IStateMachineController<TState, TSynchContext> 
            where THandler: class, TActions, IStateMachineControlObject<TState, TSynchContext>
        {
            public TState CurrentState { get; private set; }
            public THandler Actions => _controlObjects.ContainsKey(CurrentState) ? _controlObjects[CurrentState] : null;
            public TSynchContext Context { get; }
            public TState Current => CurrentState;

            Dictionary<TState, THandler> _controlObjects;

            public StateMachine(Dictionary<TState, THandler> controlObjects, TState initialState, TSynchContext context)
            {
                CurrentState = initialState;
                Context = context;
                _controlObjects = controlObjects;

                foreach (var controlObject in controlObjects)
                    controlObject.Value.RegisterStateMachine(this);

                if(_controlObjects.ContainsKey(CurrentState))
                    _controlObjects[CurrentState].Enter();
            }

            public virtual void SwitchState(TState next)
            {
                if (_controlObjects.ContainsKey(CurrentState))
                    _controlObjects[CurrentState].Exit();

                CurrentState = next;

                if (_controlObjects.ContainsKey(CurrentState))
                    _controlObjects[CurrentState].Enter();
            }
        }
    }
}
