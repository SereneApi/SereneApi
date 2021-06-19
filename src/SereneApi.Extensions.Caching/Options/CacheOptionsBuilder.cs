using System;

namespace SereneApi.Extensions.Caching.Options
{
    public class CacheOptionsBuilder : ICacheOptionsBuilder
    {
        private TimeSpan _lifeSpan;

        public void SetCacheLifeSpan(TimeSpan lifeSpan)
        {
            if (lifeSpan.TotalMilliseconds <= 0)
            {
                throw new ArgumentException("Expiration time must be greater than 0 seconds");
            }

            _lifeSpan = lifeSpan;
        }

        public ICacheOptions BuildOptions()
        {
            return new CacheOptions
            {
                LifeSpan = _lifeSpan
            };
        }
    }
}
