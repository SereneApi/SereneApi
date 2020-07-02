using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using System;

namespace SereneApi.Abstractions.Configuration
{
    public class ApiHandlerConfiguration: IApiHandlerConfiguration
    {
        private readonly Action<IDependencyCollection> _defaultBuilder;

        public void ConfigureDefaultDependencies(IDependencyCollection dependencies)
        {
            _defaultBuilder.Invoke(dependencies);
        }

        public ApiHandlerConfiguration(Action<IDependencyCollection> defaultBuilder)
        {
            _defaultBuilder = defaultBuilder;
        }

        public static IApiHandlerConfiguration Default { get; } = new ApiHandlerConfiguration(d =>
        {
            d.AddScoped(() => Defaults.Factories.QueryFactory);
            d.AddScoped(() => Defaults.Serializer);
            d.AddScoped(() => Defaults.ContentType);
            d.AddScoped(() => Defaults.Handler.Credentials);
            d.AddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
            d.AddScoped<IClientFactory>(p => new DefaultClientFactory(p));
        });
    }
}
