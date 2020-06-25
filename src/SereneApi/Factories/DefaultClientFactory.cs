using SereneApi.Helpers;
using SereneApi.Interfaces;
using System.Net.Http;

namespace SereneApi.Factories
{
    public class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyCollection _dependencies;

        public DefaultClientFactory(IDependencyCollection dependencies)
        {
            _dependencies = dependencies;

        }

        public HttpClient BuildClient()
        {
            return HttpClientHelper.BuildHttpClient(_dependencies);
        }
    }
}
