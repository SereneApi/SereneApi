using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Responses.Handlers;
using SereneApi.Handlers.Soap.Routing;
using SereneApi.Handlers.Soap.Serialization;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Configuration
{
    public class SoapHandlerConfigurationProvider : HandlerConfigurationProvider
    {
        protected override void Configure(IDependencyCollection dependencies)
        {
            dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetContentType(ContentType.TextXml);
                c.SetResourcePath(string.Empty);
                c.SetRequestHeaders(new Dictionary<string, string>
                {
                    {"SOAPAction", string.Empty}
                });
            });

            dependencies.AddScoped<IResponseHandler, ResponseHandler>();
            dependencies.AddScoped<IFailedResponseHandler, FailedResponseHandler>();
            dependencies.AddScoped<IRouteFactory, RouteFactory>();

            dependencies.AddSingleton<ISerializer>(p => p.GetRequiredDependency<ISoapSerializer>());
            dependencies.AddSingleton<ISoapSerializer, SoapSerializer>();
            dependencies.AddSingleton<IEnvelopmentService, EnvelopmentService>();
            dependencies.AddSingleton<ISoapSerializerSettings, SoapSerializerSettings>();
        }
    }
}