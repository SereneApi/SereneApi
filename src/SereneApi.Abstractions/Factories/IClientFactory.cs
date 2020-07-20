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
        /// Builds an <see cref="HttpClient"/> asynchronously.
        /// </summary>
        Task<HttpClient> BuildClientAsync();
    }
}
