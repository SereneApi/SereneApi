using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponsesBuilder
    {
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <param name="status">The <see cref="Status"/> to be returned by the mock response.</param>
        /// <param name="message">The message to be returned by the mock response. Optional</param>
        IMockResponseExtensions AddMockResponse(Status status, string message = null);
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of the content.</typeparam>
        /// <param name="content">The content to be returned by the mock response.</param>
        IMockResponseExtensions AddMockResponse<TContent>(TContent content);
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of the content.</typeparam>
        /// <param name="content">The content to be returned by the mock response.</param>
        /// <param name="status">The <see cref="Status"/> to be returned by the mock response.</param>
        IMockResponseExtensions AddMockResponse<TContent>(TContent content, Status status);


    }
}
