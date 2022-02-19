using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Http.Client.Handler;
using SereneApi.Extensions.Caching.Options;
using SereneApi.Extensions.Caching.Types;
using System;

namespace SereneApi.Extensions.Caching
{
    public static class ApiOptionsExtensions
    {
        public static void EnableCaching(this IApiConfiguration configuration, Action<ICacheOptionsBuilder> optionsAction = null)
        {
            CacheOptionsBuilder cacheOptionsBuilder = new();

            optionsAction?.Invoke(cacheOptionsBuilder);

            configuration.Dependencies.AddSingleton(() => cacheOptionsBuilder.BuildOptions());
            configuration.Dependencies.AddSingleton<Cache<Uri, ICachedResponse>>();

            configuration.Dependencies.Configure<IHandlerFactory>((p, f) =>
            {
                p.TryGetDependency(out ILogger logger);

                f.AddHandler(new CachedMessageHandler(p.GetRequiredDependency<Cache<Uri, ICachedResponse>>(), logger));
            });
        }
    }
}