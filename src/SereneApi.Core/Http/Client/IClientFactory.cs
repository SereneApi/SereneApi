using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Client
{
    /// <summary>
    /// Builds <see cref="HttpClient"/> to be used for making requests.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Builds an <see cref="HttpClient"/>. The returned <see cref="HttpClient"/> should not be disposed.
        /// </summary>
        Task<HttpClient> BuildClientAsync(out bool disposeClient);
    }
}