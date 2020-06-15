using SereneApi.Interfaces;

namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    internal interface IApiHandlerExtensions<TApiDefinition> : IApiHandlerExtensions where TApiDefinition : class
    {
    }
}
