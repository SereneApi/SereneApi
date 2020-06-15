using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Types;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    internal class ApiHandlerExtensions<TApiDefinition> : ApiHandlerExtensions, IApiHandlerExtensions<TApiDefinition> where TApiDefinition : class
    {
        public ApiHandlerExtensions()
        {
        }

        public ApiHandlerExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
        }
    }
}
