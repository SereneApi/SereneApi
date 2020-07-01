using DeltaWare.Dependencies;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptions{TApiHandler}"/>
    internal class ApiHandlerOptions<TApiHandler>: ApiHandlerOptions, IApiHandlerOptions<TApiHandler> where TApiHandler : class
    {
        public ApiHandlerOptions(IDependencyProvider dependencies, IConnectionSettings connection) : base(dependencies, connection)
        {
        }
    }
}
