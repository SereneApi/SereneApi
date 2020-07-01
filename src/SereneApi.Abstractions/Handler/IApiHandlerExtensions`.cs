using System;

namespace SereneApi.Abstractions.Handler
{
    /// <summary>
    /// Used internally so Dependency Injection can find the correct <see cref="IApiHandlerExtensions"/> for the specified <see cref="TApiDefinition"/>.
    /// </summary>
    public interface IApiHandlerExtensions<TApiDefinition>: IApiHandlerExtensions where TApiDefinition : class
    {
        /// <summary>
        /// The specific handler the <see cref="IApiHandlerExtensions"/> are for.
        /// </summary>
        Type HandlerType { get; }
    }
}
