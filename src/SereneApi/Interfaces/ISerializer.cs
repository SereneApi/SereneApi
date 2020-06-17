using SereneApi.Interfaces.Requests;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Interfaces
{
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes the response content into the <see cref="TObject"/>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type"/> contained in the response content.</typeparam>
        /// <param name="content">The content to be deserializes.</param>
        TObject Deserialize<TObject>(HttpContent content);

        /// <summary>
        /// Deserializes the response content into the <see cref="TObject"/>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type"/> contained in the response content.</typeparam>
        /// <param name="content">The content to be deserializes.</param>
        Task<TObject> DeserializeAsync<TObject>(HttpContent content);

        IApiRequestContent Serialize<TObject>(TObject value);

        Task<IApiRequestContent> SerializeAsync<TObject>(TObject value);
    }
}
