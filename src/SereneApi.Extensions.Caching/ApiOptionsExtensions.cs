using SereneApi.Abstractions.Options;
using SereneApi.Extensions.Caching.Options;
using SereneApi.Extensions.Caching.Types;
using DeltaWare.Dependencies.Abstractions;
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

            if(!(extensions is ICoreOptions options))
            {
                throw new InvalidCastException($"Base type must inherit {nameof(ICoreOptions)}");
            }

            options.Dependencies.AddSingleton(() => new Cache<Uri, ICachedResponse>(cacheOptions));
            options.Dependencies.AddScoped<HttpMessageHandler>(p => new CachedMessageHandler(p));

            return extensions;
        }
    }
}
