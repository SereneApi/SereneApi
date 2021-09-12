using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Content;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Responses.Handlers;
using SereneApi.Handlers.Soap.Routing;
using SereneApi.Handlers.Soap.Serialization;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Configuration
{
    public class SoapConfigurationFactory : ConfigurationFactory
    {
        public SoapConfigurationFactory()
        {
            Configuration.Add("ApiType", "SoapApi");
            Configuration.Add("ResourcePath", string.Empty);

            Configuration.Add("RequestHeaders", new Dictionary<string, string>
            {
                {"SOAPAction", string.Empty}
            });

            Dependencies.TryAddScoped<IResponseHandler, ResponseHandler>();
            Dependencies.TryAddScoped<IFailedResponseHandler, FailedResponseHandler>();
            Dependencies.TryAddScoped<ISoapSerializer, SoapSerializer>();
            Dependencies.TryAddScoped<ISerializer>(p => p.GetDependency<ISoapSerializer>());
            Dependencies.TryAddScoped<IRouteFactory, RouteFactory>();
            Dependencies.TryAddScoped(() => ContentType.TextXml);

            Dependencies.TryAddSingleton<IEnvelopmentService, EnvelopmentService>();
            Dependencies.TryAddSingleton<ISoapSerializerSettings, SoapSerializerSettings>();
        }
    }
}