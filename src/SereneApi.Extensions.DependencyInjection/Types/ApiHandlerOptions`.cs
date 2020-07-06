using DeltaWare.Dependencies;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;
using System;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Handler.Options;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IOptions{TApiDefinition}"/>
    internal class Options<TApiDefinition>: Options, IOptions<TApiDefinition> where TApiDefinition : class
    {
        public Type HandlerType => typeof(TApiDefinition);

        public Options(IDependencyProvider dependencies, IConnectionSettings connection) : base(dependencies,
            connection)
        {
        }
    }
}
