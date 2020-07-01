
using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    /// <inheritdoc cref="IApiResponse"/>
    /// <typeparam name="TEntity">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
    public interface IApiResponse<out TEntity>: IApiResponse
    {
        /// <summary>
        /// The deserialized value received from the API.
        /// </summary>
        TEntity Result { get; }
    }
}
