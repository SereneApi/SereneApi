using DeltaWare.Dependencies;
using Newtonsoft.Json;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Newtonsoft.Serializers;
using System;

namespace SereneApi.Extensions.Newtonsoft
{
    public static class SereneApiExtensionsExtensions
    {
        public static ISereneApiExtensions AddNewtonsoft(this ISereneApiExtensions extensions, Action<JsonSerializerSettings> factory = null)
        {
            if(factory == null)
            {
                extensions.ExtendDependencyFactory(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer()));

                return extensions;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            factory.Invoke(settings);

            extensions.ExtendDependencyFactory(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer(settings)));

            return extensions;
        }
    }
}
