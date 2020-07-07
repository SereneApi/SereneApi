using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
