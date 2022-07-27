using SereneApi.Core.Serialization;
using System;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestBody : IApiRequestResponseType
    {
        /// <summary>
        /// Specifies the content be sent in the request.
        /// </summary>
        /// <param name="content">The content to be serialized into JSON data.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>Content is serialized using the <see cref="ISerializer"/>.</remarks>
        [Obsolete("This has been replaced with WithInBodyContent")]
        IApiRequestResponseType AddInBodyContent<TContent>(TContent content);

        /// <summary>
        /// Specifies the content be sent in the request.
        /// </summary>
        /// <param name="content">The content to be serialized into JSON data.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>Content is serialized using the <see cref="ISerializer"/>.</remarks>
        IApiRequestResponseType WithInBodyContent<TContent>(TContent content);
    }
}