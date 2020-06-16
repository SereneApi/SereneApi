using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptions{TApiHandler}"/>
    public class ApiHandlerOptions<TApiHandler>: ApiHandlerOptions, IApiHandlerOptions<TApiHandler> where TApiHandler : ApiHandler
    {
        public ApiHandlerOptions(IDependencyCollection dependencyCollection, Uri source, string resource, string resourcePath) : base(dependencyCollection, source, resource, resourcePath)
        {
        }
    }
}
