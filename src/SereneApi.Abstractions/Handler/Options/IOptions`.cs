using System;

namespace SereneApi.Abstractions.Handler.Options
{
    /// <summary>
    /// The options used to create the <see cref="ApiHandler"/>.
    /// </summary>
    /// <remarks>This is required for <see cref="ApiHandler"/>s that will be instantiated with Dependency Injection.</remarks>
    public interface IOptions<TApiDefinition>: IOptions where TApiDefinition : class
    {
        /// <summary>
        /// The specific handler the <see cref="IOptions"/> are for.
        /// </summary>
        Type HandlerType { get; }
    }
}
