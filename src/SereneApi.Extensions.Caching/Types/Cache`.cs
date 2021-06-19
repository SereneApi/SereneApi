using SereneApi.Extensions.Caching.Options;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.Caching.Types
{
    public class Cache<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, CachedItem<TValue>> _cachedItems = new Dictionary<TKey, CachedItem<TValue>>();

        private readonly ICacheOptions _options;

        public int Count => _cachedItems.Count;

        public Cache(ICacheOptions options)
        {
            _options = options;
        }

        public TValue Get(TKey key)
        {
            if (_cachedItems.TryGetValue(key, out CachedItem<TValue> cachedItem))
            {
                return cachedItem.Value;
            }

            return default;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            bool result = _cachedItems.TryGetValue(key, out CachedItem<TValue> item);

            value = item != null ? item.Value : default;

            return result;
        }

        public void Store(TKey key, TValue value)
        {
            _cachedItems[key] = new CachedItem<TValue>(value, _options.LifeSpan, i => OnExpired(key, i));
        }

        public void OnExpired(TKey key, CachedItem<TValue> caller)
        {
            _cachedItems.Remove(key);

            caller.Dispose();
        }

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Disposes the current instance of the <see cref="CachedItem{TValue}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to implement <see cref="CachedItem{TValue}"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (CachedItem<TValue> cachedItem in _cachedItems.Values)
                {
                    cachedItem.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
