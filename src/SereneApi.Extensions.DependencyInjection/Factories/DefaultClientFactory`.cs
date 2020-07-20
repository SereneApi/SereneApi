using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Authorisation.Authorizers;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Request.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Factories
{
    /// <summary>
    /// Configures the API to use <see cref="IHttpClientFactory"/>.
    /// </summary>
    /// <typeparam name="TApi">The API that will be using <see cref="IHttpClientFactory"/>.</typeparam>
    internal class DefaultClientFactory<TApi>: IClientFactory
    {
        private readonly IDependencyProvider _dependencies;

        private readonly string _handlerName;

        /// <summary>
        /// Specifies if the <see cref="DefaultClientFactory{TApi}"/> has been configured.
        /// </summary>
        public bool IsConfigured { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultClientFactory{TApi}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultClientFactory([NotNull] IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _handlerName = GenerateClientName();
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
        public Task<HttpClient> BuildClientAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                IServiceProvider provider = _dependencies.GetDependency<IServiceProvider>();

                IHttpClientFactory clientFactory = provider.GetRequiredService<IHttpClientFactory>();

                HttpClient client = clientFactory.CreateClient(_handlerName);

                return client;
            });
        }

        /// <summary>
        /// Configures the <see cref="DefaultClientFactory{TApi}"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when an invalid value is provided.</exception>
        public void Configure()
        {
            if(IsConfigured)
            {
                return;
            }

            IServiceCollection services = _dependencies.GetDependency<IServiceCollection>();

            services.AddHttpClient(_handlerName, client =>
            {
                IConnectionSettings connection = _dependencies.GetDependency<IConnectionSettings>();

                if(connection.Timeout == default || connection.Timeout < 0)
                {
                    throw new ArgumentException("The timeout value must be greater than 0 seconds.");
                }

                client.BaseAddress = connection.BaseAddress;
                client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
                client.DefaultRequestHeaders.Accept.Clear();

                if(_dependencies.TryGetDependency(out IAuthorizer authenticator))
                {
                    IAuthorization authorization = authenticator.AuthorizeAsync().GetAwaiter().GetResult();

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
                }
                else if(_dependencies.TryGetDependency(out IAuthorization authentication))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }

                if(_dependencies.TryGetDependency(out ContentType contentType))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if(_dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
                {
                    return messageHandler;
                }

                ICredentials credentials = _dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };

                return messageHandler;
            });

            IsConfigured = true;
        }

        /// <summary>
        /// Generates the <see cref="HttpClient"/> name.
        /// </summary>
        private static string GenerateClientName()
        {
            return $"SereneApi.{typeof(TApi).Name}.Client";
        }
    }
}
