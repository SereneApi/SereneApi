using System;
using SereneApi.Abstractions.Serialization;

namespace SereneApi.Abstractions.Requests.Builder
{
    public interface IApiRequestResponseType : IApiRequestPerformer
    {
        /// <summary>
        /// Specifies the <see cref="Type"/> that will be received in the content of the response.
        /// </summary>
        /// <remarks>Content is deserialized using the <see cref="ISerializer"/>.</remarks>
        IApiRequestPerformer<TContent> RespondsWithType<TContent>();
    }
}
