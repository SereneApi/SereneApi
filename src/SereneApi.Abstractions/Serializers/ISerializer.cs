using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Serializers
{
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes the response content into <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The object that the content will be deserialized into.</typeparam>
        /// <param name="content">The content to be deserialized.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        T Deserialize<T>([NotNull] IApiResponseContent content);

        /// <summary>
        /// Deserializes the response content asynchronously into <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The object that the content will be deserialized into.</typeparam>
        /// <param name="content">The content to be deserialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<T> DeserializeAsync<T>([NotNull] IApiResponseContent content);

        /// <summary>
        /// Serializes the response content asynchronously into <see cref="IApiRequestContent"/>.
        /// </summary>
        /// <typeparam name="T">The object to be serialized.</typeparam>
        /// <param name="value">The value to be Serialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IApiRequestContent Serialize<T>([NotNull] T value);

        /// <summary>
        /// Serializes the value asynchronously into <see cref="IApiRequestContent"/>.
        /// </summary>
        /// <typeparam name="T">The object to be serialized.</typeparam>
        /// <param name="value">The value to be Serialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiRequestContent> SerializeAsync<T>([NotNull] T value);
    }
}
