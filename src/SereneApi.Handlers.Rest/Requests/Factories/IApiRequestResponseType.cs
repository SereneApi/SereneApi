using SereneApi.Core.Serialization;
using System;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestResponseType : IApiRequestPerformer
    {
        /// <summary>
        /// Specifies the <see cref="Type"/> that will be received in the content of the response.
        /// </summary>
        /// <remarks>Content is deserialized using the <see cref="ISerializer"/>.</remarks>
        IApiRequestPerformer<TContent> RespondsWith<TContent>();
    }
}
