namespace IngameScript
{
    partial class Program
    {
        public interface IModule
        {
            string UniqueName { get; }
            string Alias { get; }
            void Bind(IBindingContext context);
            void Run();
            void OnSaving();
        }
    }
}
