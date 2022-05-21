using SereneApi.Core.Http.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Handler
{
    /// <summary>
    /// The result returned from an <see cref="MockRestApiHandlerBase"/>.
    /// </summary>
    /// <remarks>This is intended to imitate IActionResult.</remarks>
    public interface IMockResult
    {
        /// <summary>
        /// The <see cref="Status"/> of the result.
        /// </summary>
        Status Status { get; }
    }
}
