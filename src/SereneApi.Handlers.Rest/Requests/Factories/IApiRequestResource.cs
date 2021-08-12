using System;
using SereneApi.Core.Handler;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestResource : IApiRequestVersion
    {
        /// <summary>
        /// Specifies the Resource to be used for the request.
        /// </summary>
        /// <remarks>This overrides the default value provided by the <see cref="IApiHandler"/>.</remarks>
        /// <param name="resource">The resource to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when a string is either empty or only contains null spaces.</exception>
        IApiRequestVersion AgainstResource(string resource);
    }
}
