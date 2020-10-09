using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response;
using SereneApi.Extensions.Mocking.Dependencies;
using SereneApi.Extensions.Mocking.Dependencies.Whitelist;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Response
{
    /// <summary>
    /// Responds to mock API request.
    /// </summary>
    public interface IMockResponse: IWhitelist, IDisposable
    {
        /// <summary>
        /// The <see cref="Status"/> the <see cref="IMockResponse"/> will respond with.
        /// </summary>
        Status Status { get; }

        /// <summary>
        /// Gets the <see cref="IApiRequestContent"/> that the <see cref="IMockResponse"/> will responds with.
        /// </summary>
        /// <remarks>The <see cref="CancellationToken"/> is required for the timeout to function.</remarks>
        /// <param name="cancellationToken">The cancellation token used in conjunction with the <see cref="DelayedResponseDependency"/>.</param>
        /// <exception cref="TaskCanceledException">Thrown when a <see cref="CancellationToken"/> is received.</exception>
        Task<IApiRequestContent> GetResponseContentAsync(CancellationToken cancellationToken = default);
    }
}
