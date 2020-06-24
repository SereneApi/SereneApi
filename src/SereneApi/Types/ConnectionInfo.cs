using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public readonly struct ConnectionInfo: IConnectionInfo
    {
        public Uri BaseAddress { get; }

        public string Source { get; }

        public string Resource { get; }

        public string ResourcePath { get; }

        public int Timeout { get; }

        public int RetryAttempts { get; }

        public ConnectionInfo(Uri baseAddress, string resource = default, string resourcePath = default, int timeout = default, int attemptCount = default)
        {
            BaseAddress = baseAddress;

            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            Source = $"{BaseAddress}{ResourcePath}{Resource}";

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

        public ConnectionInfo(string baseAddress, string resource = default, string resourcePath = default, int timeout = default, int attemptCount = default)
        {
            ExceptionHelper.EnsureParameterIsNotNull(baseAddress, nameof(baseAddress));

            baseAddress = SourceHelpers.EnsureSourceSlashTermination(baseAddress);
            BaseAddress = new Uri(baseAddress);

            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            Source = $"{BaseAddress}{ResourcePath}{Resource}";

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
            return new ConnectionInfo(BaseAddress, Resource, ResourcePath, timeout, RetryAttempts);
        }

        public IConnectionInfo SetRetryAttempts(int attemptCount)
        {
            return new ConnectionInfo(BaseAddress, Resource, ResourcePath, Timeout, attemptCount);
        }
    }
}
