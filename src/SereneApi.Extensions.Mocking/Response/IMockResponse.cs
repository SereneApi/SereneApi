using System;
using System.Threading;
using System.Threading.Tasks;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Mocking.Dependencies;

namespace SereneApi.Extensions.Mocking.Response
{
    /// <summary>
    /// Responds to mock API request.
    /// </summary>
    public interface IMockResponse: IWhitelist, IDisposable
    {
        /// <summary>
        /// The <see cref="ISerializer"/> used by the <see cref="IMockResponse"/>.
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// The <see cref="Status"/> the <see cref="IMockResponse"/> will respond with.
        /// </summary>
        Status Status { get; }

        /// <summary>
        /// The message the <see cref="IMockResponse"/> will respond with.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the <see cref="IApiRequestContent"/> that the <see cref="IMockResponse"/> will responds with.
        /// </summary>
        /// <remarks>The <see cref="CancellationToken"/> is required for the timeout to function.</remarks>
        /// <param name="cancellationToken">The cancellation token used in conjunction with the <see cref="DelayedResponseDependency"/>.</param>
        Task<IApiRequestContent> GetResponseContentAsync(CancellationToken cancellationToken = default);
    }
}
