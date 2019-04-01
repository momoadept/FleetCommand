namespace IngameScript
{
    partial class Program
    {
        public interface IModule: IStringifiable
        {
            void Bind(IBindingContext context);
            void Run();
            string UniqueName { get; }
        }
    }
}
