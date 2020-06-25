using System.Net.Http;

namespace SereneApi.Interfaces
{
    public interface IClientFactory
    {
        HttpClient BuildClient();
    }
}
