using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Serialization.Types;
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

                serializer.Transformers.Add(new DateTimeTransformer());
            });

            dependencies.Register<RestRequestFactory>()
                .DefineAs<IApiRequestFactory>()
                .AsTransient();

            dependencies.Register<QuerySerializer>()
                .DefineAs<IQuerySerializer>()
                .AsScoped();

            dependencies.Register(() => new JsonSerializer())
                .DefineAs<ISerializer>()
                .AsScoped();

            dependencies.Register<RouteFactory>()
                .DefineAs<IRouteFactory>()
                .AsScoped();

            dependencies.Register<ResponseHandler>()
                .DefineAs<IResponseHandler>()
                .AsScoped();

            dependencies.Register<FailedResponseHandler>()
                .DefineAs<IFailedResponseHandler>()
                .AsScoped();
        }
    }
}