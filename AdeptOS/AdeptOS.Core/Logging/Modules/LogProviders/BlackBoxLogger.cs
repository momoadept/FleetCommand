namespace IngameScript
{
    partial class Program
    {
        public class BlackBoxLogger: IModule, ILogProvider
        {
            public string UniqueName => "BBL";
            public string Alias => "Black Box Logger";

            public void Bind(IBindingContext context)
            {
            }

            public void Run()
            {
            }
            
            public void Log(LogSeverity severity, params string[] items)
            {
            }
        }
    }
}
