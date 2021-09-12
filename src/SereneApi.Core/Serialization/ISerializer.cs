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
        /// ///
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<T> DeserializeAsync<T>(IResponseContent content);

        IRequestContent Serialize(object value);
    }
}