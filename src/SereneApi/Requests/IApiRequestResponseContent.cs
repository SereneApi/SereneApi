using System;
using SereneApi.Abstractions.Serialization;

namespace SereneApi.Requests
{
    public interface IApiRequestResponseContent : IApiRequestPerformer
    {
        /// <summary>
        /// Specifies the content to be received by the request.
        /// </summary>
        /// <remarks>Content is deserialized using the <see cref="ISerializer"/>.</remarks>
        IApiRequestPerformer<TContent> RespondsWithContent<TContent>();
    }
}
