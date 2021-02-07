using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Factories
{
    /// <summary>
    /// Builds <see cref="HttpClient"/> to be used for making requests.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Builds an <see cref="HttpClient"/>. The returned <see cref="HttpClient"/> should not be disposed.
        /// </summary>
        Task<HttpClient> BuildClientAsync();

        HttpMessageHandler BuildHttpMessageHandler();
    }
}
