using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponsesBuilder
    {
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <param name="status">The <see cref="Status"/> to be returned by the mock response.</param>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        IMockResponseConfigurator AddMockResponse([NotNull] Status status);
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of the content.</typeparam>
        /// <param name="content">The content to be returned by the mock response.</param>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        IMockResponseConfigurator AddMockResponse<TContent>([NotNull] TContent content);
        /// <summary>
        /// Adds a <see cref="IMockResponse"/>.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of the content.</typeparam>
        /// <param name="content">The content to be returned by the mock response.</param>
        /// <param name="status">The <see cref="Status"/> to be returned by the mock response.</param>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        IMockResponseConfigurator AddMockResponse<TContent>([NotNull] TContent content, [NotNull] Status status);


    }
}
