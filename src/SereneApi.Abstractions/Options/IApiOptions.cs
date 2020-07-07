using System;
using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
