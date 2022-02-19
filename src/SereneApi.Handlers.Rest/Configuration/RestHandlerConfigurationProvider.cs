using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses.Handlers;
using SereneApi.Handlers.Rest.Routing;

namespace SereneApi.Handlers.Rest.Configuration
{
    public class RestHandlerConfigurationProvider : HandlerConfigurationProvider
    {
        public RestHandlerConfigurationProvider()
        {
            Dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetContentType(ContentType.Json);
                c.SetResourcePath("api");
            });

            Dependencies.AddTransient<IApiRequestFactory, RestRequestFactory>();

            Dependencies.AddScoped<IQueryFactory, QueryFactory>();
            Dependencies.AddScoped<ISerializer>(() => new JsonSerializer());
            Dependencies.AddScoped<IRouteFactory, RouteFactory>();
            Dependencies.AddScoped<IResponseHandler, ResponseHandler>();
            Dependencies.AddScoped<IFailedResponseHandler, FailedResponseHandler>();
        }
    }
}