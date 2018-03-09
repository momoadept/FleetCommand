using System;
using System.Collections.Generic;

namespace IngameScript.Core.ServiceProvider
{
    public class MyServiceProvider : IMyServiceProvider
    {
        public MyServiceProvider()
        {

        }

        protected Dictionary<Type, Func<object>> Config { get; } = new Dictionary<Type, Func<object>>();

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            Func<object> service;

            if (Config.TryGetValue(type, out service))
            {
                var result = service() as T;
                if (result != null) return result;

                throw new Exception($"Wrong service configured for {type.FullName}");
            }

            throw new Exception($"No service configured for {type.FullName}");
        }

        public void Use<T>(T service) where T : class
        {
            var type = typeof(T);

            if (Config.ContainsKey(type))
                Config[type] = () => service;
            else
                Config.Add(type, () => service);
        }

        public void Use<T>(Func<T> factory) where T : class
        {
            var type = typeof(T);

            if (Config.ContainsKey(type))
                Config[type] = factory;
            else
                Config.Add(type, factory);
        }

        public void Use<T>(T service, Type providedType) where T : class
        {
            if (Config.ContainsKey(providedType))
                Config[providedType] = () => service;
            else
                Config.Add(providedType, () => service);
        }
    }
}