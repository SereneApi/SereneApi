using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Extensions;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Options;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ISereneApiExtensions ConfigureSereneApi(this IServiceCollection services, ISereneApiConfiguration configuration = null)
        {
            if(configuration == null)
            {
                configuration = SereneApiConfiguration.Default;
            }

            ServiceDescriptor service = ServiceDescriptor.Singleton(configuration);

            if(!services.Contains(service))
            {
                throw new ArgumentException();
            }

            services.AddSingleton(configuration);

            if(configuration is SereneApiConfiguration handlerConfiguration)
            {
                return handlerConfiguration.GetExtensions();
            }

            throw new ArgumentException();
        }

        public static ISereneApiExtensions ConfigureSereneApi(this IServiceCollection services, Action<ISereneApiConfigurationBuilder> factory)
        {
            SereneApiConfiguration configuration = (SereneApiConfiguration)SereneApiConfiguration.Default;

            factory.Invoke(configuration);

            return services.ConfigureSereneApi(configuration);
        }

        /// <summary>
        /// Allows a registered <see cref="ApiHandler"/> to be extended upon.
        /// </summary>
        /// <typeparam name="TApiDefinition">The <see cref="ApiHandler"/> Definition.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the specified <see cref="ApiHandler"/> has not been registered.</exception>
        public static IApiHandlerExtensions ExtendApiHandler<TApiDefinition>(this IServiceCollection services) where TApiDefinition : class
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IDependencyCollection dependencies = GetDependencies(serviceProvider.GetService<IApiOptionsConfigurator<TApiDefinition>>());

            return new ApiHandlerExtensions(dependencies);
        }

        /// <summary>
        /// Allows a registered <see cref="ApiHandler"/> to be extended upon.
        /// </summary>
        /// <typeparam name="TApiDefinition">The <see cref="ApiHandler"/> Definition.</typeparam>
        /// /// <exception cref="ArgumentException">Thrown if the specified <see cref="ApiHandler"/> has not been registered.</exception>
        public static void ExtendApiHandler<TApiDefinition>(this IServiceCollection services, Action<IApiHandlerExtensions> factory) where TApiDefinition : class
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IDependencyCollection dependencies = GetDependencies(serviceProvider.GetService<IApiOptionsConfigurator<TApiDefinition>>());

            IApiHandlerExtensions extensions = new ApiHandlerExtensions(dependencies);

            factory.Invoke(extensions);
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="factory">An action to configure the <see cref="ApiHandler"/></param>
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiOptionsConfigurator<TApiDefinition>> factory) where TApiDefinition : class where TApiImplementation : class, IApiHandler, TApiDefinition
        {
            services.TryAddSingleton(SereneApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApiDefinition, TApiImplementation>();

            if(services.Contains(service))
            {
                throw new ArgumentException();
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ISereneApiConfiguration configuration = provider.GetRequiredService<ISereneApiConfiguration>();

            ApiApiOptionsBuilder<TApiDefinition> builder = configuration.GetOptionsBuilder<ApiApiOptionsBuilder<TApiDefinition>>();

            services.Add(new ServiceDescriptor(typeof(IApiOptionsConfigurator<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(IApiOptions<TApiDefinition>), BuildApiHandlerOptions<TApiDefinition>, ServiceLifetime.Scoped));

            return new ApiHandlerExtensions(builder.Dependencies);
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="factory">An action to configure the <see cref="ApiHandler"/> with an <see cref="IServiceProvider"/> to get already registered services</param>
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(
            this IServiceCollection services,
            Action<IApiOptionsConfigurator<TApiDefinition>, IServiceProvider> factory)
            where TApiDefinition : class where TApiImplementation : class, IApiHandler, TApiDefinition
        {
            services.TryAddSingleton(SereneApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApiDefinition, TApiImplementation>();

            if(services.Contains(service))
            {
                throw new ArgumentException();
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ISereneApiConfiguration configuration = provider.GetRequiredService<ISereneApiConfiguration>();

            ApiApiOptionsBuilder<TApiDefinition> builder = configuration.GetOptionsBuilder<ApiApiOptionsBuilder<TApiDefinition>>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptionsConfigurator<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptions<TApiDefinition>),
                BuildApiHandlerOptions<TApiDefinition>, ServiceLifetime.Scoped));

            return new ApiHandlerExtensions(builder.Dependencies);
        }

        private static IApiOptionsConfigurator<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IApiOptionsConfigurator<TApiDefinition>> factory, ApiApiOptionsBuilder<TApiDefinition> builder, IServiceCollection services) where TApiDefinition : class
        {
            factory.Invoke(builder);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddScoped<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());
            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiDefinition> clientFactory = new ClientFactory<TApiDefinition>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiOptionsConfigurator<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IApiOptionsConfigurator<TApiDefinition>, IServiceProvider> factory, ApiApiOptionsBuilder<TApiDefinition> builder, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class
        {
            factory.Invoke(builder, provider);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddScoped<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());
            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiDefinition> clientFactory = new ClientFactory<TApiDefinition>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiOptions<TApiDefinition> BuildApiHandlerOptions<TApiDefinition>(IServiceProvider provider) where TApiDefinition : class
        {
            ApiApiOptionsBuilder<TApiDefinition> builder = (ApiApiOptionsBuilder<TApiDefinition>)provider.GetRequiredService<IApiOptionsConfigurator<TApiDefinition>>();

            return builder.BuildOptions();
        }

        private static IDependencyCollection GetDependencies(IApiOptionsConfigurator configurator)
        {
            if(configurator is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(ICoreOptions)}");
        }
    }
}
