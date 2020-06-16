using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using SereneApi.Interfaces;
using SereneApi.Interfaces.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    /// <summary>
    /// Responds to mock API request.
    /// </summary>
    public interface IMockResponse: IWhitelist
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
