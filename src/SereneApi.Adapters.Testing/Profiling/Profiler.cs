using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Request.Events;
using SereneApi.Abstractions.Response.Events;
using SereneApi.Adapters.Testing.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace SereneApi.Adapters.Testing.Profiling
{
    /// <inheritdoc cref="IProfiler"/>
    internal class Profiler: IProfiler
    {
        private bool _isLocked;

        private readonly object _lock = new object();

        private readonly IEventRelay _eventRelay;

        private readonly Dictionary<Guid, RequestProfile> _requestProfiles = new Dictionary<Guid, RequestProfile>();

        /// <inheritdoc cref="IProfiler.HasActiveSession"/>
        public bool HasActiveSession => _isLocked;

        public Profiler([NotNull] IEventRelay eventRelay)
        {
            _eventRelay = eventRelay ?? throw new ArgumentNullException(nameof(eventRelay));
        }

        /// <inheritdoc cref="IProfiler.StartSession"/>
        public void StartSession()
        {
            Monitor.Enter(_lock, ref _isLocked);

            _requestProfiles.Clear();

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

            return new Session(_requestProfiles.Values.ToList());
        }

        private void OnRetryEvent(RetryEvent retryEvent)
        {
            _requestProfiles[retryEvent.Value].RetryAttempts++;
        }

        private void OnRequestEvent(RequestEvent requestEvent)
        {
            RequestProfile request = new RequestProfile(requestEvent.Value, requestEvent.Reference.GetType())
            {
                Sent = requestEvent.EventTime
            };

            _requestProfiles.Add(request.Identity, request);
        }

        private void OnResponseEvent(ResponseEvent responseEvent)
        {
            RequestProfile profile = _requestProfiles[responseEvent.Value.Request.Identity];

            profile.Response = responseEvent.Value;
            profile.Received = responseEvent.EventTime;
        }
    }
}
