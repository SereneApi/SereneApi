using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Factories
{
    public interface IClientFactory
    {
        Task<HttpClient> BuildClientAsync();
    }
}
