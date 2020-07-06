using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Handler.Options
{
    public interface IOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
