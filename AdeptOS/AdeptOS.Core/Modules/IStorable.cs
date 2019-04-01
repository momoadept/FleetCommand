namespace IngameScript
{
    partial class Program
    {
        public interface IStorableModule
        {
            string GetState();

            void RestoreFromState(string state);
        }
    }
}
