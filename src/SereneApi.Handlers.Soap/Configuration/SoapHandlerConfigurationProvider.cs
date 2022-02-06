using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Responses.Handlers;
using SereneApi.Handlers.Soap.Routing;
using SereneApi.Handlers.Soap.Serialization;
using DeltaWare.Dependencies.Abstractions;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Configuration
{
    public class SoapHandlerConfigurationProvider : HandlerConfigurationProvider
    {
        public SoapHandlerConfigurationProvider()
        {
            Dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetContentType(ContentType.TextXml);
                c.SetResourcePath(string.Empty);
                c.SetRequestHeaders(new Dictionary<string, string>
                {
                    {"SOAPAction", string.Empty}
                });
            });

            Dependencies.AddScoped<IResponseHandler, ResponseHandler>();
            Dependencies.AddScoped<IFailedResponseHandler, FailedResponseHandler>();
            Dependencies.AddScoped<IRouteFactory, RouteFactory>();

            Dependencies.AddSingleton<ISerializer>(p => p.GetRequiredDependency<ISoapSerializer>());
            Dependencies.AddSingleton<ISoapSerializer, SoapSerializer>();
            Dependencies.AddSingleton<IEnvelopmentService, EnvelopmentService>();
            Dependencies.AddSingleton<ISoapSerializerSettings, SoapSerializerSettings>();
        }
    }
}