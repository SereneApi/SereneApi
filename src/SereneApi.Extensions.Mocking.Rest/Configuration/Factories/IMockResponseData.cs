using SereneApi.Core.Http.Responses;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    public interface IMockResponseData
    {
        /// <summary>
        /// Specifies how the Mock Response will reply to a matching request.
        /// </summary>
        /// <param name="status">The <see cref="Status"/> to be returned.</param>
        IMockResponseDelay RespondsWith(Status status);

        /// <summary>
        /// Specifies how the Mock Response will reply to a matching request.
        /// </summary>
        /// <typeparam name="TContent">The content <see cref="Type"/>.</typeparam>
        /// <param name="content">The content to be sent in the body of the reply.</param>
        /// <param name="status">The <see cref="Status"/> to be returned.</param>
        /// <returns></returns>
        IMockResponseDelay RespondsWith<TContent>(TContent content, Status status = Status.Ok);
    }
}