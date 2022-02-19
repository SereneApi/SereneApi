using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Core.Configuration
{
    internal sealed class ApiOnInitialization : IApiOnInitialization, IDisposable
    {
        public IDependencyProvider Dependencies { get; }

        public ApiOnInitialization(IDependencyProvider dependencies)
        {
            Dependencies = dependencies;
        }

        public void Dispose()
        {
            Dependencies?.Dispose();
        }
    }
}