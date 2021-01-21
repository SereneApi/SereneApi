using System;

namespace SereneApi.Abstractions.Response
{
    /// <inheritdoc cref="IApiResponse"/>
    /// <typeparam name="TEntity">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
    public interface IApiResponse<out TEntity>: IApiResponse
    {
        /// <summary>
        /// The deserialized data received from the API.
        /// </summary>
        TEntity Data { get; }
    }
}
