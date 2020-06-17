using SereneApi.Abstraction.Enums;
using SereneApi.Interfaces;
using SereneApi.Interfaces.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponsesBuilder
    {
        /// <summary>
        /// Adds a <see cref="ISerializer"/> to be used for serializing and deserializing the <see cref="IApiRequestContent"/>.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown if an <see cref="ISerializer"/> has already been provided.</exception>
        void UseSerializer(ISerializer serializer);

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
