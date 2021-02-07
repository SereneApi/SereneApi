using System;

namespace SereneApi.Extensions.Caching.Options
{
    public interface ICacheOptionsBuilder
    {
        void SetCacheLifeSpan(TimeSpan lifeSpan);
    }
}
