using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public class Connection: IConnectionSettings
    {
        public Uri BaseAddress { get; }

        public string Source { get; }

        public string Resource { get; }

        public string ResourcePath { get; }

        public int Timeout { get; set; } = ApiHandlerOptionDefaults.Timeout;

        public int RetryAttempts { get; set; }

        public Connection(string baseAddress, string resource = default, string resourcePath = default)
        {
            ExceptionHelper.EnsureParameterIsNotNull(baseAddress, nameof(baseAddress));

            baseAddress = SourceHelpers.EnsureSourceSlashTermination(baseAddress);

            BaseAddress = new Uri(baseAddress);

            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            Source = $"{BaseAddress}{ResourcePath}{Resource}";
        }
    }
}
