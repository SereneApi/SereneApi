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
        public static HttpClient BuildHttpClient(IDependencyCollection dependencies)
        {
            bool handlerFound = dependencies.TryGetDependency(out HttpMessageHandler messageHandler);

            if(!handlerFound)
            {
                messageHandler = BuildMessageHandler(dependencies);
            }

            // If a handle was found, the handler is not disposed of as the Dependency Collection has ownership.
            HttpClient client = new HttpClient(messageHandler, !handlerFound);

            ConfigureHttpClient(client, dependencies);
            BuildRequestHeaders(client.DefaultRequestHeaders, dependencies);

            return client;
        }

        public static void BuildRequestHeaders(HttpRequestHeaders requestHeaders, IDependencyCollection dependencies)
        {
            requestHeaders.Accept.Clear();

            if(dependencies.TryGetDependency(out IAuthenticator authenticator))
            {
                IAuthentication authentication = authenticator.GetAuthentication();

                requestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }
            else if(dependencies.TryGetDependency(out IAuthentication authentication))
            {
                requestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }

            if(dependencies.TryGetDependency(out ContentType contentType))
            {
                requestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
            }
        }

        public static HttpMessageHandler BuildMessageHandler(IDependencyCollection dependencies)
        {
            ICredentials credentials = dependencies.GetDependency<ICredentials>();

            HttpClientHandler messageHandler = new HttpClientHandler
            {
                Credentials = credentials
            };

            return messageHandler;
        }

        public static void ConfigureHttpClient(HttpClient client, IDependencyCollection dependencies)
        {
            IConnectionSettings connection = dependencies.GetDependency<IConnectionSettings>();

            if(connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
        }
    }
}
