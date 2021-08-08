using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Core.Configuration
{
    public interface IHandlerConfigurationBuilder : IHandlerConfiguration
    {
        new string ResourcePath { get; set; }

        new int Timeout { get; set; }

        new int RetryCount { get; set; }

        void AddDependency<TDependency>(Func<TDependency> dependency, Lifetime lifetime, Binding binding = Binding.Bound);
    }
}
