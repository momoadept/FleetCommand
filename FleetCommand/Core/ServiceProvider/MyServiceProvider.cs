using System;
using System.Collections.Generic;
using IngameScript.Core.Interfaces;
using IngameScript.Core.Logging;

namespace IngameScript.Core.ServiceProvider
{
        public class MyServiceProvider : IMyServiceProvider
        {
            protected Dictionary<Type, object> Config { get; } = new Dictionary<Type, object>();

            public T Get<T>() where T : class
            {
                var type = typeof(T);
                object service;

                if (Config.TryGetValue(type, out service))
                {
                    var result = service as T;
                    if (result != null)
                    {
                        return result;
                    }

                    throw new Exception($"Wrong service configured for {type.FullName}");
                }

                throw new Exception($"No service configured for {type.FullName}");
            }

            public void Use<T>(T service) where T : class
            {
                var type = typeof(T);

                if (Config.ContainsKey(type))
                {
                    Config[type] = service;
                }
                else
                {
                    Config.Add(type, service);
                }
            }

            public MyServiceProvider()
            {
                var log = new EmptyLog();
                Use<ILog>(log);
            }
        }
}