using SereneApi.Core.Handler;
using SereneApi.Core.Versioning;
using System;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestVersion : IApiRequestEndpoint
    {
        /// <summary>
        /// Specifies the Version to be used for the request.
        /// </summary>
        /// <remarks>This overrides the default value provided by the <see cref="IApiHandler"/>.</remarks>
        /// <param name="version">The version to be used.</param>
        /// <exception cref="ArgumentException">Thrown when a null value is provided.</exception>
        /// <exception cref="IApiHandler">
        /// Thrown when a string is either empty or only contains null spaces.
        /// </exception>
        IApiRequestEndpoint AgainstVersion(string version);

        /// <summary>
        /// Specifies the <see cref="IApiHandler"/> to be used for the request.
        /// </summary>
        /// <remarks>This overrides the default value provided by the <see cref="IApiHandler"/>.</remarks>
        /// <param name="version">The version to be used.</param>
        /// <exception cref="IApiVersion">Thrown when a null value is provided.</exception>
        IApiRequestEndpoint AgainstVersion(IApiVersion version);
    }
}