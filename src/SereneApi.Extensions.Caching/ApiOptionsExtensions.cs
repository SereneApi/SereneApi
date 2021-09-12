using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Options.Factories;
using SereneApi.Extensions.Caching.Options;
using SereneApi.Extensions.Caching.Types;
using System;
using System.Net.Http;

namespace SereneApi.Extensions.Caching
{
    public static class ApiOptionsExtensions
    {
        public static IApiOptionsExtensions EnableCaching(this IApiOptionsExtensions extensions, Action<ICacheOptionsBuilder> optionsAction = null)
        {
            CacheOptionsBuilder cacheOptionsBuilder = new CacheOptionsBuilder();

            optionsAction?.Invoke(cacheOptionsBuilder);

            extensions.Dependencies.AddSingleton(() => cacheOptionsBuilder.BuildOptions());
            extensions.Dependencies.AddSingleton<Cache<Uri, ICachedResponse>>();

            extensions.Dependencies.AddScoped<HttpMessageHandler, CachedMessageHandler>();

            return extensions;
        }
    }
}