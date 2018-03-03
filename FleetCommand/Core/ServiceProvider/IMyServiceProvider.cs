using System;

namespace IngameScript.Core.ServiceProvider
{
    public interface IMyServiceProvider
    {
        T Get<T>() where T : class;

        void Use<T>(T service) where T : class;

        void Use<T>(Func<T> factory) where T : class;

        void Use<T>(T service, Type providedType) where T : class;
    }
}