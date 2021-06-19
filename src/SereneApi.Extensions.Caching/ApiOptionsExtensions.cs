using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Options;
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

            ICacheOptions cacheOptions = cacheOptionsBuilder.BuildOptions();

            extensions.Dependencies.AddSingleton(() => new Cache<Uri, ICachedResponse>(cacheOptions));
            extensions.Dependencies.AddScoped<HttpMessageHandler>(p => new CachedMessageHandler(p));

            return extensions;
        }
    }
}
