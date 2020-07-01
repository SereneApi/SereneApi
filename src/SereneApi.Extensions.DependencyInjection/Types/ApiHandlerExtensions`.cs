using DeltaWare.Dependencies;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Types;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerExtensions{TApiDefinition}"/>
    internal class ApiHandlerExtensions<TApiDefinition>: ApiHandlerExtensions, IApiHandlerExtensions<TApiDefinition> where TApiDefinition : class
    {
        public ApiHandlerExtensions(IDependencyCollection dependencies) : base(dependencies)
        {

        }
    }
}
