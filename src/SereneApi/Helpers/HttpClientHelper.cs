using SereneApi.Interfaces;
using SereneApi.Types.Headers.Accept;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient CreateHttpClientFromDependencies(IDependencyCollection dependencies)
        {
            IConnectionInfo connection = dependencies.GetDependency<IConnectionInfo>();

            if(connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            if(!dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
            {
                ICredentials credentials = dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            HttpClient client = new HttpClient(messageHandler)
            {
                BaseAddress = connection.BaseAddress,
                Timeout = TimeSpan.FromSeconds(connection.Timeout)
            };

            client.DefaultRequestHeaders.Accept.Clear();

            if(dependencies.TryGetDependency(out IAuthentication authentication))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }

            if(dependencies.TryGetDependency(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
            }

            return client;
        }
    }
}
