using System;

namespace FC.Core.Core.ServiceProvider
{
    public interface IService
    {
        Type[] Provides { get; }
    }
}