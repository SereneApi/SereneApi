using System;
using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <inheritdoc cref="IApiOptions{TApiDefinition}"/>
    internal class ApiOptions<TApiDefinition>: ApiOptions, IApiOptions<TApiDefinition> where TApiDefinition : class
    {
        public Type HandlerType => typeof(TApiDefinition);

        public ApiOptions(IDependencyProvider dependencies, IConnectionSettings connection) : base(dependencies,
            connection)
        {
        }
    }
}
