using SereneApi.Interfaces;

namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    /// <summary>
    /// Used internally so Dependency Injection can find the correct <see cref="IApiHandlerExtensions"/> for the specified <see cref="TApiDefinition"/>.
    /// </summary>
    internal interface IApiHandlerExtensions<TApiDefinition> : IApiHandlerExtensions where TApiDefinition : class
    {
    }
}
