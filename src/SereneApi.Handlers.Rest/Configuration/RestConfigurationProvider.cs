using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Content;
using SereneApi.Core.Factories;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Core.Transformation;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Requests.Handlers;
using SereneApi.Handlers.Rest.Routing;
using System.Net;

namespace SereneApi.Handlers.Rest.Configuration
{
    public class RestConfigurationProvider : ConfigurationProvider
    {
        public RestConfigurationProvider()
        {
            ResourcePath = "api";
            RetryCount = 0;
            Timeout = 30;

            Dependencies += d => d.TryAddScoped<ITransformationService>(() => new TransformationService());
            Dependencies += d => d.TryAddScoped<IQueryFactory>(p => new QueryFactory(p));
            Dependencies += d => d.TryAddScoped<ISerializer>(() => new JsonSerializer());
            Dependencies += d => d.TryAddScoped(() => ContentType.Json);
            Dependencies += d => d.TryAddScoped(() => CredentialCache.DefaultCredentials);
            Dependencies += d => d.TryAddScoped<IRouteFactory>(p => new RouteFactory(p));
            Dependencies += d => d.TryAddScoped<IClientFactory>(p => new ClientFactory(p));
            Dependencies += d => d.TryAddScoped<IResponseHandler>(p => new ResponseHandler(p));
            Dependencies += d => d.TryAddScoped<IFailedResponseHandler>(p => new FailedResponseHandler(p));
            Dependencies += d => d.TryAddScoped<IRequestHandler>(p => new RetryingRequestHandler(p));
        }
    }
}
