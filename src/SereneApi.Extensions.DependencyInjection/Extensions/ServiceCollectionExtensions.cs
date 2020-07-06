using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Extensions.DependencyInjection.Types;
using System;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Handler.Extensions;
using SereneApi.Abstractions.Handler.Options;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Allows a registered <see cref="ApiHandler"/> to be extended upon.
        /// </summary>
        /// <typeparam name="TApiDefinition">The <see cref="ApiHandler"/> Definition.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the specified <see cref="ApiHandler"/> has not been registered.</exception>
        public static IApiHandlerExtensions ExtendApiHandler<TApiDefinition>(this IServiceCollection services) where TApiDefinition : class
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IDependencyCollection dependencies = GetDependencies(serviceProvider.GetService<IOptionsConfigurator<TApiDefinition>>());

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

            IDependencyCollection dependencies = GetDependencies(serviceProvider.GetService<IOptionsConfigurator<TApiDefinition>>());

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
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IOptionsConfigurator<TApiDefinition>> factory) where TApiDefinition : class where TApiImplementation : class, IApiHandler, TApiDefinition
        {
            services.TryAddSingleton(Configuration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApiDefinition, TApiImplementation>();

            if (services.Contains(service))
            {
                throw new ArgumentException();
            }

            services.Add(service);

            OptionsBuilder<TApiDefinition> builder = new OptionsBuilder<TApiDefinition>();

            services.Add(new ServiceDescriptor(typeof(IOptionsConfigurator<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(IOptions<TApiDefinition>), p => BuildApiHandlerOptions<TApiDefinition>(p, services), ServiceLifetime.Scoped));

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
            Action<IOptionsConfigurator<TApiDefinition>, IServiceProvider> factory)
            where TApiDefinition : class where TApiImplementation : class, IApiHandler, TApiDefinition
        {
            ServiceDescriptor service = ServiceDescriptor.Scoped<TApiDefinition, TApiImplementation>();

            if(services.Contains(service))
            {
                throw new ArgumentException();
            }

            services.Add(service);

            OptionsBuilder<TApiDefinition> builder = new OptionsBuilder<TApiDefinition>();

            services.TryAdd(new ServiceDescriptor(typeof(IOptionsConfigurator<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IOptions<TApiDefinition>),
                p => BuildApiHandlerOptions<TApiDefinition>(p, services), ServiceLifetime.Scoped));

            return new ApiHandlerExtensions(builder.Dependencies);
        }


        private static IOptionsConfigurator<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IOptionsConfigurator<TApiDefinition>> factory, OptionsBuilder<TApiDefinition> builder, IServiceCollection services) where TApiDefinition : class
        {
            factory.Invoke(builder);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
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

        private static IOptionsConfigurator<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IOptionsConfigurator<TApiDefinition>, IServiceProvider> factory, OptionsBuilder<TApiDefinition> builder, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class
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

        private static IOptions<TApiDefinition> BuildApiHandlerOptions<TApiDefinition>(IServiceProvider provider, IServiceCollection services) where TApiDefinition : class
        {
            OptionsBuilder<TApiDefinition> builder = (OptionsBuilder<TApiDefinition>)provider.GetRequiredService<IOptionsConfigurator<TApiDefinition>>();

            return builder.BuildOptions();
        }

        private static IDependencyCollection GetDependencies(IOptionsConfigurator configurator)
        {
            if(configurator is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(ICoreOptions)}");
        }
    }
}
