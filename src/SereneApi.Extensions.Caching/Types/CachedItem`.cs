using System;
using System.Timers;

namespace SereneApi.Extensions.Caching.Types
{
    public class CachedItem<TValue> : IDisposable
    {
        private readonly Timer _expiryTimer = new();

        private readonly Action<CachedItem<TValue>> _onExpired;

        public TValue Value { get; }

        public CachedItem(TValue value, TimeSpan expiresAfter, Action<CachedItem<TValue>> onExpired)
        {
            _expiryTimer.Interval = expiresAfter.TotalMilliseconds;
            _expiryTimer.Elapsed += OnExpired;
            _expiryTimer.Start();

            _onExpired = onExpired;

            Value = value;
        }

        public void OnExpired(object sender, ElapsedEventArgs e)
        {
            _onExpired.Invoke(this);
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
                _expiryTimer.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
