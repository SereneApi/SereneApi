using System;

namespace SereneApi.Extensions.Caching.Options
{
    public class CacheOptions : ICacheOptions
    {
        public TimeSpan LifeSpan { get; set; } = TimeSpan.FromSeconds(15);
    }
}
