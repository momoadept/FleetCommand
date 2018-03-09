namespace FC.Core.Core.ComponentModel
{
    public interface IStateStrategy<in TState>
    {
        void ActivateState(TState previous, TState next);
        void DeactivateState(TState previous, TState next);
    }
}
