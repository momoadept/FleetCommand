namespace IngameScript
{
    partial class Program
    {
        public interface IStringifiable
        {
            /// <summary>
            /// No {} wrapping expected
            /// </summary>
            /// <returns></returns>
            string Stringify();
            void Restore(string value);
        }
    }
}
