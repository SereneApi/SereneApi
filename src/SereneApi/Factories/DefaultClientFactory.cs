using SereneApi.Helpers;
using SereneApi.Interfaces;
using System.Net.Http;

namespace SereneApi.Factories
{
    internal sealed class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyCollection _dependencyCollection;

        public DefaultClientFactory(IDependencyCollection dependencyCollection)
        {
            _dependencyCollection = dependencyCollection;
        }

        public HttpClient BuildClient()
        {
            return HttpClientHelper.CreateHttpClientFromDependencies(_dependencyCollection);
        }
    }
}
