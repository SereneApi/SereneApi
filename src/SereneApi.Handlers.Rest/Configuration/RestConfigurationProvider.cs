using SereneApi.Core.Configuration;

namespace SereneApi.Handlers.Rest.Configuration
{
    internal class RestConfigurationProvider : ConfigurationProvider
    {
        public RestConfigurationProvider()
        {
            ResourcePath = "api";
            RetryCount = 0;
            Timeout = 30;

            //Dependencies.TryAddScoped<IQueryFactory>(() => new QueryFactory());
            //Dependencies.TryAddScoped<ISerializer>(() => new JsonSerializer());
            //Dependencies.TryAddScoped(() => ContentType.Json);
            //Dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
            //Dependencies.TryAddScoped<IRouteFactory>(p => new RouteFactory(p));
            //Dependencies.TryAddScoped<IClientFactory>(p => new ClientFactory(p));
            //Dependencies.TryAddScoped<IResponseHandler>(p => new ResponseHandler(p));
            //Dependencies.TryAddScoped<IFailedResponseHandler>(p => new FailedResponseHandler(p));
            //Dependencies.TryAddScoped<IRequestHandler>(p => new RetryingRequestHandler(p));
        }
    }
}
