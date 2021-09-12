using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Events;
using SereneApi.Core.Factories;
using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Transformation;
using SereneApi.Core.Transformation.Transformers;
using System;
using System.Net;

namespace SereneApi.Core.Configuration
{
    public abstract class ConfigurationFactory : IHandlerConfigurationBuilder, IHandlerConfigurationFactory, IDisposable
    {
        private readonly IEventManager _eventManager = new EventManager();

        public Configuration Configuration { get; } = new();

        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        protected ConfigurationFactory()
        {
            Configuration.Add("RetryAttempts", 0);
            Configuration.Add("Timeout", 30);
            Configuration.Add("ThrowExceptions", false);

            Dependencies.AddSingleton<IConfiguration>(() => Configuration);
            Dependencies.AddSingleton(() => _eventManager, Binding.Unbound);

            Dependencies.AddScoped<ITransformationService, TransformationService>();
            Dependencies.AddScoped<IObjectToStringTransformer, BasicObjectToStringTransformer>();
            Dependencies.AddScoped<IStringToObjectTransformer, BasicStringToObjectTransformer>();

            Dependencies.AddScoped(() => CredentialCache.DefaultCredentials);

            Dependencies.AddScoped<IRequestHandler, RetryingRequestHandler>();

            Dependencies.AddSingleton<IClientFactory, ClientFactory>();
        }

        public ApiOptionsFactory<TApiHandler> BuildOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler
        {
            DependencyCollection dependencies = (DependencyCollection)Dependencies;

            return new ApiOptionsFactory<TApiHandler>((IDependencyCollection)dependencies.Clone());
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}