using DeltaWare.Dependencies;
using System;

namespace SereneApi.Abstractions.Configuration
{
    public interface ISereneApiConfigurationBuilder
    {
        int Timeout { get; set; }

        string ResourcePath { get; set; }

        int RetryCount { get; set; }

        void SetDependencies(Action<IDependencyCollection> factory);

        void AddDependencies(Action<IDependencyCollection> factory);
    }
}
