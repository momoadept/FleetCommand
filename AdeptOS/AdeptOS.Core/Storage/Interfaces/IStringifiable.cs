namespace IngameScript
{
    partial class Program
    {
        public interface IStringifiable
        {
            string Stringify();
            void Restore(string value);
        }
    }
}
