using System;
using SereneApi.Abstractions.Serialization;

namespace SereneApi.Requests
{
    public interface IApiRequestContent : IApiRequestResponseContent
    {
        /// <summary>
        /// Specifies the content be sent in the request.
        /// </summary>
        /// <param name="content">The content to be serialized into JSON data.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>Content is serialized using the <see cref="ISerializer"/>.</remarks>
        IApiRequestResponseContent AddInBodyContent<TContent>(TContent content);
    }
}
