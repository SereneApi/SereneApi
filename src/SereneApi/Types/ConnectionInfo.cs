using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public readonly struct ConnectionInfo: IConnectionInfo
    {
        public Uri Source { get; }

        public string Resource { get; }

        public string ResourcePath { get; }

        public int Timeout { get; }

        public int RetryAttempts { get; }

        private ConnectionInfo(Uri source, string resource, string resourcePath, int timeout = default, int attemptCount = default)
        {
            Source = source;

            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            if(attemptCount != default)
            {
                ApiHandlerOptionsRules.ValidateRetryAttempts(attemptCount);
            }

            if(timeout == default)
            {
                Timeout = ApiHandlerOptionDefaults.Timeout;
            }
            else
            {
                Timeout = timeout;
            }

            RetryAttempts = attemptCount;
        }

        public ConnectionInfo(string source, string resource, string resourcePath, int timeout = default, int attemptCount = default)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            source = SourceHelpers.EnsureSourceSlashTermination(source);
            Source = new Uri(source);

            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            if(timeout == default)
            {
                Timeout = ApiHandlerOptionDefaults.Timeout;
            }
            else
            {
                Timeout = timeout;
            }

            RetryAttempts = attemptCount;
        }

        public IConnectionInfo SetTimeout(int timeout)
        {
            return new ConnectionInfo(Source, Resource, ResourcePath, timeout, RetryAttempts);
        }

        public IConnectionInfo SetRetryAttempts(int attemptCount)
        {
            return new ConnectionInfo(Source, Resource, ResourcePath, Timeout, attemptCount);
        }
    }
}
