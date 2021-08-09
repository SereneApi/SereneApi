using SereneApi.Core.Content;
using System;
using System.Threading.Tasks;

namespace SereneApi.Core.Serialization
{
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes the response content asynchronously into <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The object that the content will be deserialized into.</typeparam>
        /// <param name="content">The content to be deserialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<T> DeserializeAsync<T>(IResponseContent content);

        /// <summary>
        /// Serializes the response content asynchronously into <see cref="IRequestContent"/>.
        /// </summary>
        /// <typeparam name="T">The object to be serialized.</typeparam>
        /// <param name="value">The value to be Serialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestContent Serialize<T>(T value);

        /// <summary>
        /// Serializes the value asynchronously into <see cref="IRequestContent"/>.
        /// </summary>
        /// <typeparam name="T">The object to be serialized.</typeparam>
        /// <param name="value">The value to be Serialized.</param>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IRequestContent> SerializeAsync<T>(T value);
    }
}
