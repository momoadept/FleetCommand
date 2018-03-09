using System.Collections.Generic;
using System.Linq;
using FC.Core.Core.FakeAsync.Promises;

namespace FC.Core.Core.ComponentModel
{
    public class StatefullComponent<TState, TStrategy>: BaseComponent, IStatefull<TState>
        where TStrategy : IStateStrategy<TState>
    {
        protected Dictionary<TState, TStrategy> Strategies = new Dictionary<TState, TStrategy>();
        private TState _state;

        protected TState State
        {
            get
            {
                if (_state == null)
                {
                    _state = Strategies.First().Key;
                }
                return _state;
            }
            set
            {
                OnStateChanging(value);
            }
        }

        protected TStrategy Strategy => Strategies[State];

        protected virtual void OnStateChanging(TState newState)
        {
            Strategy.DeactivateState(_state, newState);
            var oldState = _state;
            _state = newState;
            Strategy.ActivateState(oldState, newState);
        }

        protected void HasState(TState state, TStrategy strategy)
        {
            Strategies.Add(state, strategy);
        }

        protected void DefaultState(TState state)
        {
            _state = state;
        }

        public StatefullComponent(string componentId) : base(componentId)
        {
        }

        public Promise<TState> GetState()
        {
            return Promise.FromValue(State);
        }
    }
}
