using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Content;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses.Handlers;
using SereneApi.Handlers.Rest.Routing;

namespace SereneApi.Handlers.Rest.Configuration
{
    public class RestConfigurationFactory : ConfigurationFactory
    {
        public RestConfigurationFactory()
        {
            Configuration.Add("ApiType", "RestApi");
            Configuration.Add("ResourcePath", "api");

            Dependencies.TryAddTransient<IApiRequestFactory, RestRequestFactory>();

            Dependencies.TryAddScoped<IQueryFactory, QueryFactory>();
            Dependencies.TryAddScoped<ISerializer>(() => new JsonSerializer());
            Dependencies.TryAddScoped(() => ContentType.Json);
            Dependencies.TryAddScoped<IRouteFactory, RouteFactory>();
            Dependencies.TryAddScoped<IResponseHandler, ResponseHandler>();
            Dependencies.TryAddScoped<IFailedResponseHandler, FailedResponseHandler>();
        }
    }
}