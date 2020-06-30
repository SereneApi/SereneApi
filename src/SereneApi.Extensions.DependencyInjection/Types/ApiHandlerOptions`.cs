using DeltaWare.Dependencies;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptions{TApiHandler}"/>
    public class ApiHandlerOptions<TApiHandler>: ApiHandlerOptions, IApiHandlerOptions<TApiHandler> where TApiHandler : ApiHandler
    {
        public ApiHandlerOptions(IDependencyProvider dependencies, IConnectionSettings connection) : base(dependencies, connection)
        {
        }
    }
}
