using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Request.Events;
using SereneApi.Abstractions.Response.Events;
using System;
using System.Threading;

namespace SereneApi.Adapters.Testing.Profiling
{
    /// <inheritdoc cref="IProfiler"/>
    internal class Profiler: IProfiler
    {
        private bool _isLocked;

        private readonly object _lock = new object();

        private readonly IEventRelay _eventRelay;

        private Session _currentSession;

        /// <inheritdoc cref="IProfiler.IsActive"/>
        public bool IsActive => _isLocked;

        public Profiler(IEventRelay eventRelay)
        {
            _eventRelay = eventRelay ?? throw new ArgumentNullException(nameof(eventRelay));
        }

        /// <inheritdoc cref="IProfiler.StartSession"/>
        public void StartSession()
        {
            Monitor.Enter(_lock, ref _isLocked);

            _currentSession = new Session();

            _eventRelay.Subscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.Subscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.Subscribe<ResponseEvent>(OnResponseEvent);
        }

        /// <inheritdoc cref="IProfiler.EndSession"/>
        public ISession EndSession()
        {
            if(!_isLocked)
            {
                throw new MethodAccessException("StartSession must be call first.");
            }

            _eventRelay.UnSubscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.UnSubscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.UnSubscribe<ResponseEvent>(OnResponseEvent);

            Monitor.Exit(_lock);

            return _currentSession;
        }

        private void OnRetryEvent(RetryEvent retryEvent)
        {

        }

        private void OnRequestEvent(RequestEvent requestEvent)
        {

        }

        private void OnResponseEvent(ResponseEvent responseEvent)
        {

        }
    }
}
