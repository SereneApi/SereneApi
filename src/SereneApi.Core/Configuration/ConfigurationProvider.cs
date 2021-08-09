using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factory;
using System;

namespace SereneApi.Core.Configuration
{
    public abstract class ConfigurationProvider : IHandlerConfigurationBuilder, IHandlerConfigurationFactory
    {
        protected Action<IDependencyCollection> Dependencies { get; set; }

        public string ResourcePath { get; set; }

        public int Timeout { get; set; }

        public int RetryCount { get; set; }

        public void AddDependency<TDependency>(Func<TDependency> dependency, Lifetime lifetime, Binding binding = Binding.Bound)
        {
            Dependencies += d => d.AddDependency(dependency, lifetime, binding);
        }

        protected ConfigurationProvider()
        {
            Dependencies += d => d.AddSingleton(() => this);
        }

        public ApiOptionsFactory<TApiHandler> BuildOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler
        {
            IDependencyCollection dependencies = new DependencyCollection();

            Dependencies.Invoke(dependencies);

            return new ApiOptionsFactory<TApiHandler>(dependencies);
        }
    }
}
