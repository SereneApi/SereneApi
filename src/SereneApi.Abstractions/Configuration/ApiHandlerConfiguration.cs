using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using System;
using SereneApi.Abstractions.Handler.Options;

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

        public IOptionsBuilder GetOptionsBuilder()
        {
            OptionsBuilder optionsBuilder = new OptionsBuilder();

            ConfigureDefaultDependencies(optionsBuilder.Dependencies);

            return optionsBuilder;
        }

        public IOptionsBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            ConfigureDefaultDependencies(builder.Dependencies);

            return builder;
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
