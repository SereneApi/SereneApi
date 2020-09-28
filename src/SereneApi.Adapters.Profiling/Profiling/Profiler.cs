using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Request.Events;
using SereneApi.Abstractions.Response.Events;
using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Adapters.Profiling.Profiling
{
    /// <inheritdoc cref="IProfiler"/>
    internal class Profiler: IProfiler
    {
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
            if(HasActiveSession)
            {
                // Seriously don't like this, but as it stands, this should never be thrown.    
                throw new MethodAccessException();
            }

            HasActiveSession = true;

            _requestProfiles.Clear();

            _eventRelay.Subscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.Subscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.Subscribe<ResponseEvent>(OnResponseEvent);
        }

        /// <inheritdoc cref="IProfiler.EndSession"/>
        public ISession EndSession()
        {
            if(!HasActiveSession)
            {
                throw new MethodAccessException("StartSession must be call first.");
            }

            _eventRelay.UnSubscribe<RetryEvent>(OnRetryEvent);
            _eventRelay.UnSubscribe<RequestEvent>(OnRequestEvent);
            _eventRelay.UnSubscribe<ResponseEvent>(OnResponseEvent);

            HasActiveSession = false;

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
