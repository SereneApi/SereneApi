using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Events.Types;
using SereneApi.Adapters.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Adapters.Profiling
{
    /// <inheritdoc cref="IProfiler"/>
    internal class Profiler : IProfiler
    {
        private readonly Stopwatch _sessionDuration = new Stopwatch();

        private readonly IEventRelay _eventRelay;

        private readonly Dictionary<Guid, RequestProfile> _requestProfiles = new Dictionary<Guid, RequestProfile>();

        /// <inheritdoc cref="IProfiler.HasActiveSession"/>
        public bool HasActiveSession { get; private set; }

        public Profiler([NotNull] IEventRelay eventRelay)
        {
            _eventRelay = eventRelay ?? throw new ArgumentNullException(nameof(eventRelay));
        }

        /// <inheritdoc cref="IProfiler.StartSession"/>
        public void StartSession()
        {
            if (HasActiveSession)
            {
                // Seriously don't like this, but as it stands, this should never be thrown.    
                throw new MethodAccessException();
            }

            HasActiveSession = true;

            _sessionDuration.Reset();
            _sessionDuration.Start();

            _requestProfiles.Clear();

            _eventRelay.Subscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.Subscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.Subscribe<ResponseEvent>(OnResponseEvent);
        }

        /// <inheritdoc cref="IProfiler.EndSession"/>
        public ISession EndSession()
        {
            if (!HasActiveSession)
            {
                throw new MethodAccessException("StartSession must be call first.");
            }

            _eventRelay.UnSubscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.UnSubscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.UnSubscribe<ResponseEvent>(OnResponseEvent);

            HasActiveSession = false;

            _sessionDuration.Stop();

            return new Session(_requestProfiles.Values.ToList(), _sessionDuration.Elapsed);
        }

        private void OnRetryEvent(RetryEvent retryEvent)
        {
            _requestProfiles[retryEvent.Value.Identity].RetryAttempts++;
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
