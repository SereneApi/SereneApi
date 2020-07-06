using System;

namespace SereneApi.Abstractions.Factories
{
    /// <summary>
    /// Builds instances of the request <see cref="ApiHandler"/>.
    /// </summary>
    public interface IApiHandlerFactory: IDisposable
    {
        /// <summary>
        /// Creates a new instance of the requested <see cref="ApiHandler"/> type.
        /// </summary>
        TApiDefinition Build<TApiDefinition>() where TApiDefinition : class;
    }
}
