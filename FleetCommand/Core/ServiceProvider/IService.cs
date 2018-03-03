using System;

namespace IngameScript.Core.ServiceProvider
{
    public interface IService
    {
        Type[] Provides { get; }
    }
}