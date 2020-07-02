using System;
using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Handler.Options
{
    public interface IApiHandlerOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
