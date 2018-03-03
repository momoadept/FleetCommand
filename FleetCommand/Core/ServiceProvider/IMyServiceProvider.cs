namespace IngameScript.Core.ServiceProvider
{
        public interface IMyServiceProvider
        {
            T Get<T>() where T : class;

            void Use<T>(T service) where T : class;
        }
}
