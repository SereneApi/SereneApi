using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Types;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <summary>
    /// Used internally so Dependency Injection can find the correct <see cref="ApiHandlerExtensions"/> for the specified <see cref="TApiDefinition"/>.
    /// </summary>
    internal class ApiHandlerExtensions<TApiDefinition> : ApiHandlerExtensions, IApiHandlerExtensions<TApiDefinition> where TApiDefinition : class
    {
    }
}
