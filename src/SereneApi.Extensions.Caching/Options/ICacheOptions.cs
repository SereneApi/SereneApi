using System;

namespace SereneApi.Extensions.Caching.Options
{
    public interface ICacheOptions
    {
        TimeSpan LifeSpan { get; }
    }
}