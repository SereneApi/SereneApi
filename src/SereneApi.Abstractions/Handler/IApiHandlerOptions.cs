using DeltaWare.Dependencies;
using System;

namespace SereneApi.Abstractions.Handler
{
    public interface IApiHandlerOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
