using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Core.Serialization;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Rest.Configuration.Transformers;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses.Handlers;
using SereneApi.Handlers.Rest.Routing;

namespace SereneApi.Handlers.Rest.Configuration
{
    public class RestHandlerConfigurationProvider : HandlerConfigurationProvider
    {
        protected override void Configure(IDependencyCollection dependencies)
        {
            dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetContentType(ContentType.Json);
                c.SetResourcePath("api");
            });

            dependencies.Configure<IObjectSerializer>(s =>
            {
                ObjectSerializer serializer = (ObjectSerializer)s;

                serializer.Transformers.AddTransformer(new DateTimeTransformer());
            });

            dependencies.AddTransient<IApiRequestFactory, RestRequestFactory>();

            dependencies.AddScoped<IQuerySerializer, QuerySerializer>();
            dependencies.AddScoped<ISerializer>(() => new JsonSerializer());
            dependencies.AddScoped<IRouteFactory, RouteFactory>();
            dependencies.AddScoped<IResponseHandler, ResponseHandler>();
            dependencies.AddScoped<IFailedResponseHandler, FailedResponseHandler>();
        }
    }
}