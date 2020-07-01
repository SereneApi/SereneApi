using DeltaWare.Dependencies;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;
using System;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerExtensions{TApiDefinition}"/>
    internal class ApiHandlerExtensions<TApiDefinition>: IApiHandlerExtensions<TApiDefinition>, ICoreOptions where TApiDefinition : class
    {
        public IDependencyCollection Dependencies { get; }

        public Type HandlerType => typeof(TApiDefinition);

        public ApiHandlerExtensions(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
